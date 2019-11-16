namespace DeserializationChallenge.Models
{
    public class Visitor
    {
        public string name { get; set; }
        public string visiting { get; set; }
        public Company company { get; set; }
    }

    public class Company
    {
        public string name { get; set; }
    }
}