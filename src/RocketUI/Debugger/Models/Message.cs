using System;

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

        public Response(int id)
        {
            Id = id;
        }
        
        public Response(int id, object data) : this(id)
        {
            Data = data;
        }
    }


    public class ErrorResponse : Response
    {
        public string Error { get; set; }

        public ErrorResponse(int id, Exception exception) : base(id)
        {
            Error = exception.Message;
        }
    }
}