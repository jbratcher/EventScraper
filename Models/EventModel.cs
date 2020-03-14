using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EventScraper.Models
{
    public class EventModel
    {
        public int Id { get; set; }

        [Index(IsUnique = true)]
        public string EventModelDateTime { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Price { get; set; }
    }
}
