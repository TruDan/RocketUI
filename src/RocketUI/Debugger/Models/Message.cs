namespace RocketUI.Debugger.Models
{
    public class Message
    {
        public int      Id        { get; set; }
        public string   Command   { get; set; }
        public string[] Arguments { get; set; }
    }

    public class Response
    {
        public int    Id   { get; set; }
        public object Data { get; set; }
    }
}