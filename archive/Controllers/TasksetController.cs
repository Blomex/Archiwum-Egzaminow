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
using Microsoft.AspNetCore.Http;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Task = System.Threading.Tasks.Task;

namespace archive.Controllers
{
    public class TasksetController : AbstractArchiveController
    {
        private readonly ILogger _logger;
        private readonly IRepository _repository;
        private readonly IStorageService _storageService;
        public const int AttachmentRequestLimit = 20_000_000; // in bytes

        public TasksetController(ILogger<TasksetController> logger, IRepository repository,
            IUserActivityService activityService, IStorageService storageService) : base(activityService)
        {
            _logger = logger;
            _repository = repository;
            _storageService = storageService;
        }

        [Authorize]
        public async Task<IActionResult> ShowTaskset(int id)
        {
            var taskset = await _repository.Tasksets
                .Include(t => t.Course)
                
                .Include(t => t.Attachments)
                .ThenInclude(a => a.File)
                
                .Include(t => t.Tasks)
                .ThenInclude(t => t.Tags)
                
                .Include(t => t.Tasks)
                .ThenInclude(t => t.Solutions)
                .ThenInclude(t => t.Author)
                
                .Include(t => t.Tasks)
                .ThenInclude(t => t.Attachments)
                .ThenInclude(a => a.File)
                
                .FirstOrDefaultAsync(t => t.Id == id);

            if (taskset == null)
            {
                return new StatusCodeResult(404);
            }

            if (taskset.Course.Archive == true || !ModelState.IsValid)
            {
                return new StatusCodeResult(403);
            }

            // We need full path (see Index(id))
            return View("/Views/Taskset/ShowTaskset.cshtml", taskset);
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
                .Where(t => t.CourseId == id)
                .OrderByDescending(t => t.Year)
                .ToListAsync();

            var model = new IndexViewModel {Tasksets = tasksets, Course = course};
            // This is called also from HomeController.Shortcut and becouse of this we need full path to view file
            return View("/Views/Taskset/Index.cshtml", model);
        }

        [Authorize]
        public async Task<IActionResult> IndexFilter(int id, IndexViewModel model)
        {
            _logger.LogDebug($"Requested taskset for course id={id}");

            var course = await _repository.Courses.FindAsync(id);
            if (course == null)
            {
                _logger.LogDebug($"Cannot find course with id={id}");
                return new StatusCodeResult(404);
            }
            var tasksets = new List<archive.Data.Entities.Taskset>();

            if (model.haveSolutions)
            {
                tasksets = await _repository.Solutions
                    .Where(e => e.Task.Taskset.CourseId == id
                                && e.Task.Taskset.Year >= model.yearFrom
                                && e.Task.Taskset.Year <= model.yearTo
                                && ((!model.haveTasks) || e.Task.Taskset.Tasks.Any())) //Not sure if needed, solution shouldn't exist without task
                    .Select(s => s.Task.Taskset).Distinct()
                    .ToListAsync();
            }
            else
            {
                tasksets = await _repository.Tasksets
                    .Where(s => s.CourseId == id
                                && s.Year >= model.yearFrom
                                && s.Year <= model.yearTo
                                && ((!model.haveTasks) || s.Tasks.Any()))
                    .OrderByDescending(s => s.Year)
                    .ToListAsync();
            }
            
            model.Tasksets = tasksets;    
            model.Course = course;
            // This is called also from HomeController.Shortcut and becouse of this we need full path to view file
            return View("/Views/Taskset/Index.cshtml", model);
        }

        [Authorize]
        public async Task<IActionResult> AllFilterTasks(AllFilterTasksViewModel model)
        {
            var correctTasks = new List<int>();
            
            var courses = await _repository.Courses.Where(c => c.Archive == false).ToListAsync();
            model.AddCourseList(courses);           

            if(model.minRating > 0 || model.minRatingNumber > 0)
            {
                model.haveSolutions = true;
            }
        
            var tasksToShow = new List<archive.Data.Entities.Task>();

            var listOfSolutions = new Dictionary<int, List<Solution>>();

            var tagNames = model.Tags?.Split(", ").ToList<string>();
            var listOfTaggedTasks = new Dictionary<string, List<int>>();
            if (tagNames != null)
            {
                foreach (var name in tagNames)
                {
                    var tags = await _repository.Tags.Where(t => name == t.Name).
                        Select(t => t.TaskId).ToListAsync();
                    listOfTaggedTasks.Add(name, new List<int>(tags));
                }

                if (model.allTags)
                {
                    correctTasks = await _repository.Tags.
                        Where(t => t.Name == tagNames[0]).Select(t => t.TaskId).ToListAsync();
                    //Wszystkie tagi
                    foreach (var key in tagNames)
                    {
                        correctTasks = correctTasks?.Intersect(listOfTaggedTasks[key]).ToList();
                    }
                }
                else
                {
                    correctTasks = new List<int>();
                    foreach (var key in tagNames)
                    {
                        correctTasks = correctTasks.Concat(listOfTaggedTasks[key]).ToList();
                    }
                }

            }

               var tasks = await _repository.Tasks.
                   Include(t => t.Solutions).Where(t =>
                   (t.Taskset.CourseId == model.courseId || model.courseId == 0)
                   && t.Taskset.Year >= model.yearFrom
                   && t.Taskset.Year <= model.yearTo &&
                   (correctTasks.Contains(t.Id) || tagNames == null)
                   ).ToListAsync();
               
                   _logger.LogDebug($"Przed SQL");

                   var query = (from t in _repository.Tasks
                       join sol in _repository.Solutions on t.Id equals sol.TaskId
                       where (from res in _repository.Ratings
                                 where (res.Value == true && res.IdSolution == sol.Id)
                                 select res.Id).Count() >= model.minRating &&
                             (from res2 in _repository.Ratings
                                 where (res2.IdSolution == sol.Id)
                                 select res2.Id).Count() >= model.minRatingNumber &&
                             (t.Taskset.CourseId == model.courseId || model.courseId == 0) &&
                            t.Taskset.Year >= model.yearFrom &&
                            t.Taskset.Year <= model.yearTo &&
                             tasks.Contains(t)
                                select new
                       {
                           taskId = t.Id,
                           solutionId = sol.Id,
                           solutionAuthorId = sol.AuthorId,
                           solutionCachedContent = sol.CachedContent,
                           taskName = t.Name,
                           taskContent = t.Content,
                           taskTasksetId = t.TasksetId,
                       }).ToList();
                   _logger.LogDebug($"Halo zrobiłem SQLa");
                   foreach (var result in query)
                   {
                       if (!listOfSolutions.ContainsKey(result.taskId))
                       {
                           tasksToShow.Add(new archive.Data.Entities.Task
                           {
                               Id = result.taskId,
                               Name = result.taskName,
                               Content = result.taskContent,
                               TasksetId = result.taskTasksetId
                           });
                           listOfSolutions.Add(result.taskId, new List<Solution>());
                       }
                   }

                   foreach (var task in tasks)
                   {
                       if (!listOfSolutions.ContainsKey(task.Id))
                       {
                            listOfSolutions.Add(task.Id, new List<Solution>());
                            tasksToShow.Add(task);
                       }
                   }
                   foreach (var result in query)
                   {
                       listOfSolutions[result.taskId].Add(new archive.Data.Entities.Solution
                       {
                           Id = result.solutionId,
                           CachedContent = result.solutionCachedContent,
                           AuthorId = result.solutionAuthorId,
                           TaskId = result.taskId
                       });
                   }

                   var returnTasks = new List<archive.Data.Entities.Task>();
                   foreach (var p in tasksToShow)
                   {
                       returnTasks.Add(p);
                   }

                   if (model.haveSolutions)
                   {
                       foreach (var p in listOfSolutions)
                       {
                           if (tasksToShow.Any(e => e.Id == p.Key && !p.Value.Any()))
                           {
                               returnTasks.Remove(tasksToShow.FirstOrDefault(e => e.Id == p.Key));
                           }
                       }
                   }

                   model.Tasks = returnTasks;
            model.ListOfSolutions = listOfSolutions;    
            // This is called also from HomeController.Shortcut and becouse of this we need full path to view file
            return View("/Views/Taskset/AllFilterTasks.cshtml", model);
        }

        [Authorize(Roles = UserRoles.TRUSTED_USER)]
        public async Task<IActionResult> RemoveAttachment(int tasksetId, string fileId)
        {
            _logger.LogDebug($"Requested to remove attachment={fileId} from taskset={tasksetId}");
            var taskset = await _repository.Tasksets
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == tasksetId);

            if (taskset == null)
            {
                _logger.LogDebug($"Cannot find taskset with id={tasksetId}");
                return new StatusCodeResult(404);
            }

            var toRemove = taskset.Attachments
                .FirstOrDefault(a => a.FileId.ToString() == fileId);

            if (toRemove != null)
            {
                // "Detach" attachment, remove file
                taskset.Attachments.Remove(toRemove);
                await _storageService.Delete(toRemove.FileId);
            }
            else
            {
                _logger.LogDebug($"Cannot find file with id={fileId}");
                return new StatusCodeResult(404);
            }

            return await AddAttachmentsView(tasksetId);
        }

        private async Task StoreAttachments(Taskset entity, List<IFormFile> files)
        {
            if (files == null) return;

            // Store files attaching them to taskset
            foreach (var file in files)
            {
                var fileEntity = await _storageService.Store(file.FileName, file.OpenReadStream());
                entity.Attachments.Add(new TasksetsFiles() {TasksetId = entity.Id, FileId = fileEntity.Id});
            }

            await _repository.SaveChangesAsync();
        }


        [Authorize(Roles = UserRoles.TRUSTED_USER)]
        [RequestSizeLimit(AttachmentRequestLimit)]
        public async Task<IActionResult> AddAttachments(AddAttachmentsModel add)
        {
            _logger.LogDebug($"Requested to add attachments to taskset={add.EntityId}");
            var taskset = await _repository.Tasksets
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == add.EntityId);

            if (taskset == null)
            {
                _logger.LogDebug($"Cannot find taskset with id={add.EntityId}");
                return new StatusCodeResult(404);
            }

            await StoreAttachments(taskset, add.Attachments);
            return await AddAttachmentsView(add.EntityId);
        }

        [Authorize(Roles = UserRoles.TRUSTED_USER)]
        public async Task<IActionResult> Create(int? forCourseId)
        {
            var course = await _repository.Courses
                .Where(c => c.Id == forCourseId
                            && c.Archive == false)
                .FirstOrDefaultAsync();

            if (course != null)
            {
                return View(new CreateTasksetViewModel(course));
            }

            //if course was not selected, user will be allowed to choose course
            var courses = await _repository
                .Courses.Where(e => e.Archive == false)
                .ToListAsync();
            return View(new CreateTasksetViewModel(courses));
        }

        [Authorize(Roles = UserRoles.TRUSTED_USER)]
        [RequestSizeLimit(AttachmentRequestLimit)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTasksetViewModel taskset)
        {
            // Validate request
            _logger.LogDebug($"Requested to create taskset:" + taskset);
            var same = await _repository.Tasksets
                .Where(t => t.Name == taskset.Name && t.Year == taskset.Year)
                .FirstOrDefaultAsync();

            if (same != null || !ModelState.IsValid)
            {
                _logger.LogDebug($"Cannot add:" + taskset);
                return new StatusCodeResult(400);
            }

            // Save taskset
            var entity = new Taskset
            {
                Type = taskset.TypeAsEnum,
                Name = taskset.Name,
                Year = taskset.Year,
                CourseId = taskset.CourseId,
            };

            _repository.Tasksets.Add(entity);
            await _repository.SaveChangesAsync();

            entity.Attachments = new List<TasksetsFiles>();
            await StoreAttachments(entity, taskset.Attachments);
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
            return RedirectToAction("Index", new {id = taskset.CourseId});
        }

        public async Task<IActionResult> AddAttachmentsView(int tasksetId)
        {
            var taskset = await _repository.Tasksets
                .Include(t => t.Course)
                .Include(t => t.Attachments)
                .ThenInclude(a => a.File)
                .FirstOrDefaultAsync(t => t.Id == tasksetId);

            if (taskset == null)
            {
                _logger.LogDebug($"Cannot find taskset with id={tasksetId}");
                return new StatusCodeResult(404);
            }

            return View("AddAttachments", new AddAttachmentsModel() {Taskset = taskset, EntityId = tasksetId});
        }

    }    
}
