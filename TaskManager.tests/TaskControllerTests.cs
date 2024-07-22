using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TaskManager.tests
{
    public class TaskControllerTests
    {
        private readonly HttpClient _client;
        private readonly AppDbContext _context;

        public TaskControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TaskTestDb")
                .Options;
            _context = new AppDbContext(options);
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };

            // Seed initial data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var task = new Task { Name = "Test Task", IsComplete = false };
            _context.Tasks.Add(task);
            _context.SubTasks.Add(new SubTask
            {
                Name = "Sub Task 1",
                IsComplete = false,
                Task = task
            });
            _context.SubTasks.Add(new SubTask
            {
                Name = "Sub Task 2",
                IsComplete = false,
                Task = task
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetTask_ReturnsTaskWithSubTaskCount()
        {
            // Arrange
            var taskId = _context.Tasks.First().Id;

            // Act
            var response = await _client.GetAsync($"/tasks/{taskId}");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("SubTaskCount", content); // Check if the
            response contains sub - task count
        }

        [Fact]
        public async Task
CompleteTaskMarksTaskAsCompleteWhenAllSubTasksAreComplete()
        {
            // Arrange
            var taskId = _context.Tasks.First().Id;
            var subTasks = _context.SubTasks.Where(st => st.TaskId ==
taskId).ToList();
            foreach (var subTask in subTasks)
            {
                subTask.IsComplete = true;
            }
            _context.SaveChanges();

            // Act
            var response = await
_client.PutAsync($"/tasks/{taskId}/complete", null);

            // Assert
            var task = _context.Tasks.Find(taskId);
            Assert.True(task.IsComplete);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}