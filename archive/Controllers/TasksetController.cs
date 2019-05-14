using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using archive.Data;
using archive.Data.Entities;
using archive.Models.Taskset;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using archive.Data.Enums;
using archive.Services;

namespace archive.Controllers
{
    public class TasksetController : ArchiveController
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;

        public TasksetController(ILogger<TasksetController> logger, IRepository repository,
            IUserActivityService activityService) : base(activityService)
        {
            _logger = logger;
            _repository = repository;
        }

        [Authorize]
        public async Task<IActionResult> ShowTaskset(int id)
        {
            var taskset = await _repository.Tasksets
                .Include(t => t.Course)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taskset == null)
            {
                return new StatusCodeResult(404);
            }

            if (taskset.Course.Archive == true || !ModelState.IsValid)
            {
                return new StatusCodeResult(403);
            }

            var tasks = await _repository.Tasks.Where(s => s.TasksetId == id).ToListAsync();

            var listOfSolutions = new Dictionary<int, List<Solution>>();

            foreach (var task in tasks) {
                var solutions = await _repository.Solutions.Where(s => s.TaskId == task.Id).ToListAsync();
                listOfSolutions.Add(task.Id, solutions);
            }

            var model = new TasksetViewModel {Taskset = taskset, Tasks = tasks, ListOfSolutions = listOfSolutions};
            // We need full path (see Index(id))
            return View("/Views/Taskset/ShowTaskset.cshtml", model);
        }

        [Authorize]
        public async Task<IActionResult> Index(int id)
        {
            _logger.LogDebug($"Requested taskset for course id={id}");

            var course = await _repository.Courses.FindAsync(id);
            if (course == null)
            {
                _logger.LogDebug($"Cannot find course with id={id}");
                return new StatusCodeResult(404);
            }

            if (course.Archive == true)
            {
                return new StatusCodeResult(403);
            }
            
            var tasksets = await _repository.Tasksets
                .Where(s => s.CourseId == id)
                .OrderByDescending(s => s.Year)
                .ToListAsync();

            var model = new IndexViewModel {Tasksets = tasksets, Course = course};
            // This is called also from HomeController.Shortcut and becouse of this we need full path to view file
            return View("/Views/Taskset/Index.cshtml", model);
        }

        [Authorize(Roles = UserRoles.TRUSTED_USER)]
        public async Task<IActionResult> Create(int? forCourseId)
        {
            
            var course = await _repository.Courses
                .Where(c => c.Id == forCourseId)
                .FirstOrDefaultAsync();

            if (course != null)
            {
                return View(new CreateTasksetViewModel(course));
            }

            if (course.Archive == true)
            {
                return new StatusCodeResult(403);
            }
            
            var courses = await _repository.Courses.ToListAsync();
            return View(new CreateTasksetViewModel(courses));
        }

        [Authorize(Roles = UserRoles.TRUSTED_USER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTasksetViewModel taskset)
        {
            _logger.LogDebug($"Requested to create taskset:" + taskset);
            var same = await _repository.Tasksets
                .Where(t => t.Name == taskset.Name && t.Year == taskset.Year)
                .FirstOrDefaultAsync();

            if (same != null || !ModelState.IsValid)
            {
                _logger.LogDebug($"Cannot add:" + taskset);
                return new StatusCodeResult(400);
            }
            
            _repository.Tasksets
                .Add(new Taskset
                {
                    Type = taskset.TypeAsEnum, 
                    Name = taskset.Name,
                    Year = taskset.Year,
                    CourseId = taskset.CourseId
                });
            
            await _repository.SaveChangesAsync();
            return await Index(taskset.CourseId);
        }

        [Authorize(Roles = UserRoles.MODERATOR)]
        public async Task<IActionResult> Delete(int id)
        {
            var taskset = await _repository.Tasksets.FindAsync(id);
            if (taskset == null)
            {
                _logger.LogDebug($"Task(Id={id}) not found");
                return new StatusCodeResult(404);
            }

            // We can only delete empty tasksets
            int tasksCount = await _repository.Tasks.Where(t => t.TasksetId == taskset.Id).CountAsync();
            if (tasksCount > 0)
            {
                _logger.LogDebug($"Taskset(Id={id}) it nonempty and cannot be deleted");
                return new StatusCodeResult(400);
            }

            _repository.Tasksets.Remove(taskset);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index",  new { id = taskset.CourseId });
        }
    }
}