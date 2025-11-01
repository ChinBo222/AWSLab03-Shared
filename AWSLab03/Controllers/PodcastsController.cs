using AWSLab03.Data;
using AWSLab03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AWSLab03.Controllers
{
    public class PodcastsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PodcastsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Podcasts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Podcasts.ToListAsync());
        }

        // GET: Podcasts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Podcasts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description")] Podcast podcast)
        {
            // Assign CreatorID and CreatedDate before ModelState validation
            podcast.CreatorID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            podcast.CreatedDate = System.DateTime.Now;

            if (!ModelState.IsValid)
            {
                // Log ModelState errors to debug
                foreach (var entry in ModelState)
                    foreach (var error in entry.Value.Errors)
                        System.Diagnostics.Debug.WriteLine($"Field: {entry.Key}, Error: {error.ErrorMessage}");

                // Display errors in the view
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View(podcast);
            }

            _context.Add(podcast);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Podcasts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var podcast = await _context.Podcasts.FindAsync(id);
            if (podcast == null) return NotFound();

            return View(podcast);
        }

        // POST: Podcasts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PodcastID,Title,Description")] Podcast podcast)
        {
            if (id != podcast.PodcastID) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Podcasts.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Title = podcast.Title;
                existing.Description = podcast.Description;

                _context.Update(existing);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(podcast);
        }

        // GET: Podcasts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var podcast = await _context.Podcasts
                .FirstOrDefaultAsync(m => m.PodcastID == id);
            if (podcast == null) return NotFound();

            return View(podcast);
        }

        // POST: Podcasts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var podcast = await _context.Podcasts.FindAsync(id);
            if (podcast != null)
            {
                _context.Podcasts.Remove(podcast);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PodcastExists(int id)
        {
            return _context.Podcasts.Any(e => e.PodcastID == id);
        }


        // GET: Podcasts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var podcast = await _context.Podcasts
                .Include(p => p.Subscriptions) // <-- make sure subscriptions are loaded
                .FirstOrDefaultAsync(p => p.PodcastID == id);

            if (podcast == null)
                return NotFound();

            return View(podcast);
        }

    }
}
