using AWSLab03.Data;
using AWSLab03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLab03.Controllers
{
    [Authorize] // only logged-in users can subscribe
    public class SubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SubscriptionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int podcastId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Check for duplicates
            var exists = await _context.Subscriptions
                .AnyAsync(s => s.PodcastID == podcastId && s.UserID == user.Id);

            if (!exists)
            {
                var subscription = new Subscription
                {
                    PodcastID = podcastId,
                    UserID = user.Id,
                    SubscribedDate = DateTime.Now
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Podcasts", new { id = podcastId });
        }


        // Optional: view all subscriptions
        public async Task<IActionResult> List()
        {
            var subs = await _context.Subscriptions.Include(s => s.Podcast).ToListAsync();
            return View(subs);
        }
    
    }
}
