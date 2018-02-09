using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioServicesWeb.Controllers
{
    public class ResponseCreator
    {
        public static Response<Boolean> CreateBoolean(bool resp, string message = "")
        {
            return new Response<bool>(resp)
            {
                Result = resp,
                Message = message
            };
        }
        public static Response<T> Create<T>(T data, string message = "")
        {
            return new Response<T>(data)
            {
                Result = data != null ? true : false,
                Message = message
            };
        }
        
    }
    public class Response<T>
    {
        public long Ticks { get; }
        public bool Result { get; set; } = true;
        public string Message { get; set; }
        public T Data { get; set; }

        public Response()
        {
            Ticks = DateTime.Now.Ticks;
        }
        public Response(T data) : this()
        {
            Data = data;
        }
    }
}
