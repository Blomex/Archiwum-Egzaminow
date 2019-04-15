﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using archive.Data;
using Microsoft.AspNetCore.Mvc;
using archive.Models;
using archive.Models.Course;
using archive.Models.Taskset;
using archive.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace archive.Controllers
{
    public class HomeController : Controller
    {
        private readonly TasksetController _tasksetController;
        private readonly ILogger _logger;
        private readonly IRepository _repository;

        public HomeController(ILogger<SolutionController> logger, IRepository repository, TasksetController tasksetController)
        {
            _logger = logger;
            _repository = repository;
            _tasksetController = tasksetController;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _repository.Courses.OrderBy(c => c.Name).ToListAsync();
            return View(courses.Select(x => new CourseViewModel(x.Name, x.Id)));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public async Task<IActionResult> Shortcut(string shcCourse, string shcTaskset=null, short? shcTask=null)
        {
            if (string.IsNullOrEmpty(shcCourse))
            {
                return new StatusCodeResult(400);
            }
            _logger.LogDebug($"Shortcut for course={shcCourse}, taskset={shcTaskset}, task={shcTask}");

            // Find course by shortcut
            var course = await _repository.Courses.Where(e => e.ShortcutCode == shcCourse).FirstAsync();
            if (course == null)
            {
                return new StatusCodeResult(404);
            }

            if (string.IsNullOrEmpty(shcTaskset))
            {
                return await _tasksetController.Index(course.Id);
            }

            // Find the exam by shortcut
            var taskset = await _repository.Tasksets
                .Where(e => e.CourseId == course.Id && e.ShortcutCode == shcTaskset).FirstAsync();
            if (taskset == null)
            {
                return new StatusCodeResult(404);
            }

            if (shcTask == null)
            {
                return await _tasksetController.ShowTaskset(taskset.Id);
            }

            // Find the task by number
            var task = await _repository.Tasks
                .Where(e => e.TasksetId == taskset.Id && e.InTasksetNumber == shcTask).FirstAsync();
            if (task == null)
            {
                return new StatusCodeResult(404);
            }

            return new StatusCodeResult(404);
        }
    }
}
