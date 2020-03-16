using EventScraper.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EventScraper.ViewModels
{
    public class EventModelViewModel
    {
        public List<EventModel> EventModels { get; set; }
        public string LocationQuery { get; set; }
        public string TypeQuery { get; set; }
    }
}