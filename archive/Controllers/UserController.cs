using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using archive.Data;
using archive.Migrations;
using archive.Models.Taskset;
using archive.Models.User;
using archive.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace archive.Controllers
{
    public class UserController : ArchiveController
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly IUserActivityService _activityService;

        public UserController(IRepository repository, ILogger<UserController> logger,
            IUserActivityService activityService) : base(activityService)
        {
            _repository = repository;
            _logger = logger;
            _activityService = activityService;
        }

        public async Task<IActionResult> ShowProfile(string name)
        {
            _logger.LogDebug($"Zażądano profilu użytkownika o nazwie={name}");
            var user = await _repository.Users
                .Include(t => t.Avatar)
                .FirstOrDefaultAsync(t => t.UserName == name);

            if (user == null)
            {
                _logger.LogDebug($"Nie znaleziono użytkownika o nazwie={name}");
                return new StatusCodeResult(404);
            }

            var lastActive = await _activityService.GetLastActionTimeAsync(name);
            
            return View("/Views/User/ShowProfile.cshtml", new ProfileViewModel
            {
                UserName = user.UserName,
                AvatarImage = user.Avatar?.Image,
                Email = user.Email,
                HomePage = user.HomePage,
                Phone = user.PhoneNumber,
                LastActive = lastActive
            });
        }
    }
}