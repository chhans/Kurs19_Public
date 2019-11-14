using System;

namespace DeserializationChallenge.Models
{
    public class Secret
    {
        public Secret()
        {
            this.flag = Environment.GetEnvironmentVariable("Flag");
        }
        
        public string name { get; set; }
        public string flag { get; set; }
    }


}