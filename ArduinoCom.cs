using IoTProyecto.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IoTProyecto
{
    class ArduinoCom
    {
        private readonly SerialPort SerialPort;
        private readonly AdaFruitIO Ada;
        public Dictionary<string, FeedData> DataSet { get; set; }
        public ArduinoCom(Autentification auth)
        {
            Ada = new AdaFruitIO(auth);
            SerialPort = new SerialPort("COM1", 9600);
            //SerialPort.BaudRate = 9600;
            //SerialPort.PortName = "COM1";
            SerialPort.DtrEnable = true;
            SerialPort.RtsEnable = true;
            //SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            SerialPort.Open();
            Console.WriteLine("Arduino Com Init");
            DataSet = new Dictionary<string, FeedData>();
        }

        public async Task OnMessageAsync()
        {
            while (true)
            {
                try
                {
                    string inData = SerialPort.ReadLine();
                    Console.WriteLine(inData);
                    if (inData.StartsWith("{") && inData.EndsWith("}"))
                    {
                        var data = JsonSerializer.Deserialize<FeedData>(inData);
                        DataSet[data.feed_key] = data;
                        await Ada.PostFeed(data);
                    }
                    else Console.WriteLine(inData);
                }
                catch (TimeoutException) { }
            }
        }
        public void SendMessage(string text)
        {
            SerialPort.Write(text);
        }
    }
}
