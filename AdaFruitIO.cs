using IoTProyecto.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace IoTProyecto
{
    class AdaFruitIO
    {
        private readonly HttpClient Client;
        private readonly Autentification Auth;
        public AdaFruitIO(Autentification auth)
        {
            Client = new HttpClient();
            Auth = auth;
            Client.DefaultRequestHeaders.Add("X-AIO-Key", Auth.ADAFRUIT_IO_KEY);
            Client.BaseAddress = new Uri(" https://io.adafruit.com");
        }

        public async Task PostFeed(FeedData data) => await Client.PostAsync($"/api/v2/{Auth.ADAFRUIT_IO_USERNAME}/feeds/{data.feed_key}/data", data.ToContent());
                
    }
}
