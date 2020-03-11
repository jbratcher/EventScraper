using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventScraper.Models
{
    public class EventModel
    {
        public int Id { get; set; }
        public string EventModelDateTime { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Price { get; set; }
    }
}
