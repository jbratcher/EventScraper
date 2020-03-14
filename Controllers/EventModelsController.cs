using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AngleSharp;
using AngleSharp.Html.Parser;
using EventScraper.Data;
using EventScraper.Models;

namespace EventScraper.Controllers
{
    public class EventModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly String websiteUrl = "https://www.eventbrite.com/d/ky--louisville/tech-conference/";


        // GET: EventModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.EventModels.ToListAsync());
        }

        // GET: EventModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.EventModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // GET: EventModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EventModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventModelDateTime,Url,Title,Location,Price")] EventModel eventModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventModel);
        }

        // GET: EventModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.EventModels.FindAsync(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }

        // POST: EventModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventModelDateTime,Url,Title,Location,Price")] EventModel eventModel)
        {
            if (id != eventModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventModelExists(eventModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(eventModel);
        }

        // GET: EventModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventModel = await _context.EventModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventModel == null)
            {
                return NotFound();
            }

            return View(eventModel);
        }

        // POST: EventModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventModel = await _context.EventModels.FindAsync(id);
            _context.EventModels.Remove(eventModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventModelExists(int id)
        {
            return _context.EventModels.Any(e => e.Id == id);
        }

        private bool EventModelExistsByObject(EventModel eventModel)
        {
            bool result = _context.EventModels.Any(e => e.EventModelDateTime == eventModel.EventModelDateTime && e.Title == eventModel.Title);
            return result;
        }

        private async Task<IActionResult> GetPageData(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            var eventCards = document.QuerySelectorAll(".eds-media-card-content__content__principal");

            List<dynamic> EventModelList = new List<dynamic>();

            foreach (var row in eventCards)
            {
                // Create a container object
                EventModel newEvent = new EventModel();

                // event datetime
                newEvent.EventModelDateTime = row.Children[0].Children[0].TextContent;

                // event Url
                newEvent.Url = row.Children[0].Children[1].GetAttribute("Href");

                // event title
                newEvent.Title = row.Children[0].Children[1].TextContent;

                // event location
                newEvent.Location = row.Children[1].Children[0].TextContent;

                // event price
                newEvent.Price = row.LastChild.LastChild.TextContent;

                if (!EventModelExistsByObject(newEvent))
                {
                    EventModelList.Add(newEvent);
                }
                else
                {
                    continue;
                }
            }

            // Check if a next page link is present
            //string nextPageUrl = "";
            //var nextPageLink = document.QuerySelector("button[data-spec='page-next']");
            //if (nextPageLink != null)
            //{
            //    nextPageUrl = websiteUrl + nextPageLink.GetAttribute("Href");
            //}

            // If next page link is present recursively call the function again with the new url
            //if (!String.IsNullOrEmpty(nextPageUrl))
            //{
            //    return await GetPageData(nextPageUrl);
            //}

            IEnumerable<dynamic> DistinctEvents = EventModelList.Distinct();

            _context.AddRange(DistinctEvents);

            await _context.SaveChangesAsync();

            return Redirect("/EventModels");

        }

        public async Task<IActionResult> UpdateEvents()
        {
            await GetPageData(websiteUrl);

            return Redirect("/EventModels");
        }

    }
}
