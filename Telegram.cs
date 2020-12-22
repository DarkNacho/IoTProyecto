using IoTProyecto.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using System.Linq;

namespace IoTProyecto
{
    class Telegram
    {
        private readonly ITelegramBotClient botClient;
        private HashSet<String> Users;
        public ArduinoCom Arduino;
        private string[] Commands;
        public Telegram(string token, HashSet<String> users)
        {
            Users = users;
            Commands = new string[] { "\\brightness", "\\humidity", "\\pressure", "\\rain", "\\temp", "\\wind"};
            botClient = new TelegramBotClient(token);
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Console.WriteLine("Telegram Init");
        }

        async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                await botClient.SendTextMessageAsync(
                  chatId: e.Message.Chat,
                  text: "You said:\n" + e.Message.Text
                );
            }

            if (e.Message == null) return;
            else if (!e.Message.Text.StartsWith("\\")) return;
            else if (e.Message.Text == "\\motor" && Arduino != null)
                Arduino.SendMessage("1");
            else if(e.Message.Text == "\\register")
            {
                Users.Add(e.Message.Chat.ToString());
                var json = JsonSerializer.Serialize(Users);
                await File.WriteAllTextAsync("users.json", json);
            }
            else if ( Commands.Any(x => x == e.Message.Text) && Arduino.DataSet.Count == 6) 
                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: Arduino.DataSet[e.Message.Text.Substring(1) + "feed"].ToString());
            
            //else registrarlo...         
        }

        async public void SendAlert()
        {
            while (Arduino.DataSet.Count < 6) ;
            while (Arduino.DataSet.Count == 6)
            {
                bool storm = false;
                var msg = new StringBuilder();
                var data = Arduino.DataSet["humidityfeed"];
              
                if (data.value >= 79) msg.AppendLine($"Alta Humedad Detectada a las {data.created_at} con un valor de {data.value}");
                if (data.value <= 30) msg.AppendLine($"Baja Humedad Detectada a las {data.created_at} con un valor de {data.value}");
                int wind = Arduino.DataSet["windfeed"].value;
                int pressure = Arduino.DataSet["pressurefeed"].value;
                int rain = Arduino.DataSet["rainfeed"].value;
                if (wind >= 700 && rain >= 700 && pressure >= 200) storm = true;
                if (storm)
                    Users.AsParallel().ForAll(async id => await botClient.SendPhotoAsync(chatId: id,
                    photo: "https://ih1.redbubble.net/image.4496802.1983/flat,1000x1000,075,f.jpg",
                    caption: msg.ToString() + "Tormenta!!!"));

                if (msg.Length > 0)
                    Users.AsParallel().ForAll(async id => await botClient.SendTextMessageAsync(chatId: id, text: msg.ToString()));
                if(msg.Length > 0 || storm) Thread.Sleep(3600000); // solo para que no exista flood espera una hora...
                msg.Clear();
                Thread.Sleep(20500);

            }


        }


    }
}
