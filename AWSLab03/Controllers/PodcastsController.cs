using AWSLab03.Data;
using AWSLab03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace AWSLab03.Controllers
{
    [Authorize] // requires the user to be logged in for all actions
    public class PodcastsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PodcastsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Only Podcasters or Admins can create new podcasts
        [Authorize(Roles = "Podcaster,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        //Restrict create post as well
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Podcaster,Admin")]
        public async Task<IActionResult> Create([Bind("Title,Description")] Podcast podcast)
        {
            podcast.CreatorID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            podcast.CreatedDate = System.DateTime.Now;

            if (!ModelState.IsValid)
                return View(podcast);

            _context.Add(podcast);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Only Podcaster or Admin can edit their podcast
        [Authorize(Roles = "Podcaster,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var podcast = await _context.Podcasts.FindAsync(id);
            if (podcast == null) return NotFound();
            return View(podcast);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Podcaster,Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("PodcastID,Title,Description")] Podcast podcast)
        {
            if (id != podcast.PodcastID) return NotFound();
            if (!ModelState.IsValid) return View(podcast);

            var existing = await _context.Podcasts.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = podcast.Title;
            existing.Description = podcast.Description;

            _context.Update(existing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Only Admin can delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var podcast = await _context.Podcasts.FirstOrDefaultAsync(m => m.PodcastID == id);
            if (podcast == null) return NotFound();
            return View(podcast);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        //Anyone can view details (no restriction)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var podcast = await _context.Podcasts
                .Include(p => p.Subscriptions)
                .FirstOrDefaultAsync(p => p.PodcastID == id);

            if (podcast == null)
                return NotFound();

            return View(podcast);
        }

        //Anyone can see the list of podcasts
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Podcasts.ToListAsync());
        }

        private bool PodcastExists(int id) =>
            _context.Podcasts.Any(e => e.PodcastID == id);
    }

}
