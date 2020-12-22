using System;
namespace IoTProyecto.Models
{
    class FeedData
    {
        public string feed_key { get; set; }
        public int value { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now.AddSeconds(-60);
        public override string ToString() => $"{value} at {created_at}";
    }
}
