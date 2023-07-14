using System;
namespace ProServ.Client.Data
{
    public class LongRunData
    {
        public double Distance { get; set; }
        public bool PaceOrTotal { get; set; }
        public int PaceMinutes { get; set; }
        public int PaceSeconds { get; set; }
    }

    public class CustomData
    {
        public string Comments { get; set; }
        public TimeSpan Time { get; set; }
        public double Distance  { get; set; }

    }
}

