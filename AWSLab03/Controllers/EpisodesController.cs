using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AWSLab03.Data;
using AWSLab03.Models;

namespace AWSLab03.Controllers
{
    public class EpisodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EpisodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Episodes
       
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            var episodes = from e in _context.Episodes.Include(e => e.Podcast)
                           select e;

            // Search by title or podcast name
            if (!String.IsNullOrEmpty(searchString))
            {
                episodes = episodes.Where(e => e.Title.Contains(searchString) ||
                                               e.Podcast.Title.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "popular":
                    episodes = episodes.OrderByDescending(e => e.PlayCount);
                    break;
                case "recent":
                    episodes = episodes.OrderByDescending(e => e.ReleaseDate);
                    break;
                default:
                    episodes = episodes.OrderBy(e => e.Title);
                    break;
            }

            return View(await episodes.ToListAsync());
        }



        // GET: Episodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes
                .Include(e => e.Podcast)
                .FirstOrDefaultAsync(m => m.EpisodeID == id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // GET: Episodes/Create
       
        public IActionResult Create()
        {
            ViewData["PodcastID"] = new SelectList(_context.Podcasts, "PodcastID", "Title");
            return View();
        }



        // POST: Episodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EpisodeID,PodcastID,Title,Description,ReleaseDate,Duration,PlayCount,AudioFileURL")] Episode episode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(episode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PodcastID"] = new SelectList(_context.Podcasts, "PodcastID", "Title", episode.PodcastID);
            return View(episode);
        }

        // GET: Episodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }
            ViewData["PodcastID"] = new SelectList(_context.Podcasts, "PodcastID", "Title", episode.PodcastID);
            return View(episode);
        }

        // POST: Episodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EpisodeID,PodcastID,Title,Description,ReleaseDate,Duration,PlayCount,AudioFileURL")] Episode episode)
        {
            if (id != episode.EpisodeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(episode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpisodeExists(episode.EpisodeID))
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
            ViewData["PodcastID"] = new SelectList(_context.Podcasts, "PodcastID", "Title", episode.PodcastID);
            return View(episode);
        }

        // GET: Episodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes
                .Include(e => e.Podcast)
                .FirstOrDefaultAsync(m => m.EpisodeID == id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // POST: Episodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode != null)
            {
                _context.Episodes.Remove(episode);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EpisodeExists(int id)
        {
            return _context.Episodes.Any(e => e.EpisodeID == id);
        }


        public async Task<IActionResult> Play(int id)
        {
            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null) return NotFound();

            episode.PlayCount++;
            await _context.SaveChangesAsync();

            return View(episode);
        }

    }
}
