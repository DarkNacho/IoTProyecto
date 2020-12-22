using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace IoTProyecto
{
    public static class Extention
    {
        public static string ToJson(this object data) => JsonSerializer.Serialize(data);
        public static StringContent JsonToConent(this string data) => new StringContent(data, Encoding.UTF8, "application/json");
        public static StringContent ToContent(this object data) => data.ToJson().JsonToConent();
        public static int ToInt(this object data) => Convert.ToInt32(data.ToString().Split("\"")[0]);
    }
}
