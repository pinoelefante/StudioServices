using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StudioServicesWeb.Controllers
{
    [Route("api/news")]
    public class NewsController : MyController
    {
        [Route("all")]
        [HttpGet("{time}")]
        public string GetAllNews(long time = 0)
        {
            return $"GetAllNews ({time})";
        }

        [HttpGet("{id}")]
        public string GetNews(int id)
        {
            // Imposta la news come letta
            return $"News {id}";
        }

        [HttpPost]
        public string PostNews(string text, bool is_private, int person_id, bool is_marked, bool expire, int e_year, int e_month, int e_day)
        {
            if(!_isAdmin())
            {
                // TODO log request
                return "admin required";
            }
            return "post news";
        }

        [HttpDelete("{id}")]
        public string DeleteNews(int id)
        {
            return "delete news";
        }
    }
}
