using IoTProyecto.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.IO.Ports;
using System.Threading.Tasks;

namespace IoTProyecto
{
    class Program
    {
        static ArduinoCom Arduino;
        static Telegram Telegram;
        static void Main(string[] args)
        {
            HashSet<String> Users = new HashSet<string>();
            
            var aut = JsonSerializer.Deserialize<Autentification>(File.ReadAllText("settings.json"));
            var users= JsonSerializer.Deserialize<HashSet<String>> (File.ReadAllText("users.json"));

            Arduino = new ArduinoCom(aut);
            var ArduinoOnMessageTask = Task.Run(() => Arduino.OnMessageAsync());
            Telegram = new Telegram(aut.TELEGRAM_TOKEN, users);
            Telegram.Arduino = Arduino;
            var Alert = Task.Run(() => Telegram.SendAlert());
            ArduinoOnMessageTask.Wait();
            Alert.Wait();
        }
    }
}
