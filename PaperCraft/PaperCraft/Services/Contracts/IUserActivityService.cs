using PaperCraft.Models;
using PaperCraft.Models.PaperCraft.Models;

namespace PaperCraft.Services
{
    public interface IUserActivityService
    {
        Task LogActivityAsync(string userId, string action, string description,
            string ipAddress = null, string userAgent = null, ActivityType type = ActivityType.Other);
        Task<List<UserActivity>> GetUserActivitiesAsync(string userId, int take = 10);
    }
}