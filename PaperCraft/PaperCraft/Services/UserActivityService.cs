using Microsoft.EntityFrameworkCore;
using PaperCraft.Data;
using PaperCraft.Models;
using PaperCraft.Models.PaperCraft.Models;

namespace PaperCraft.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly AppDbContext _context;

        public UserActivityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(string userId, string action, string description,
            string ipAddress = null, string userAgent = null, ActivityType type = ActivityType.Other)
        {
            var activity = new UserActivity
            {
                UserId = userId,
                Action = action,
                Description = description,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserActivities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserActivity>> GetUserActivitiesAsync(string userId, int take = 10)
        {
            return await _context.UserActivities
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .ToListAsync();
        }
    }
}