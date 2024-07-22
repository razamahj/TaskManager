using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;

namespace TaskManager.Tests
{
    [TestFixture]
    public class TaskApiTests
    {
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddDbContext<AppDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDatabase");
                        });
                    });
                });

            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task CanCreateAndRetrieveTask()
        {
            // Arrange
            var newTask = new Task
            {
                Name = "Test Task",
                Description = "Test Description",
                IsComplete = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/tasks", newTask);
            response.EnsureSuccessStatusCode();

            var createdTask = await response.Content.ReadFromJsonAsync<Task>();
            Assert.IsNotNull(createdTask);

            var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
            getResponse.EnsureSuccessStatusCode();

            var retrievedTask = await getResponse.Content.ReadFromJsonAsync<Task>();
            Assert.IsNotNull(retrievedTask);
            Assert.AreEqual(newTask.Name, retrievedTask.Name);
            Assert.AreEqual(newTask.Description, retrievedTask.Description);
            Assert.AreEqual(newTask.IsComplete, retrievedTask.IsComplete);
        }

        [Test]
        public async Task CanCreateAndRetrieveSubTask()
        {
            // Arrange
            var task = new Task
            {
                Name = "Test Task",
                Description = "Test Description",
                IsComplete = false
            };

            var taskResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            taskResponse.EnsureSuccessStatusCode();

            var createdTask = await taskResponse.Content.ReadFromJsonAsync<Task>();
            Assert.IsNotNull(createdTask);

            var newSubTask = new SubTask
            {
                TaskId = createdTask.Id,
                Name = "Test SubTask",
                Description = "Test SubTask Description",
                IsComplete = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/subtasks", newSubTask);
            response.EnsureSuccessStatusCode();

            var createdSubTask = await response.Content.ReadFromJsonAsync<SubTask>();
            Assert.IsNotNull(createdSubTask);

            var getResponse = await _client.GetAsync($"/api/subtasks/{createdSubTask.Id}");
            getResponse.EnsureSuccessStatusCode();

            var retrievedSubTask = await getResponse.Content.ReadFromJsonAsync<SubTask>();
            Assert.IsNotNull(retrievedSubTask);
            Assert.AreEqual(newSubTask.Name, retrievedSubTask.Name);
            Assert.AreEqual(newSubTask.Description, retrievedSubTask.Description);
            Assert.AreEqual(newSubTask.IsComplete, retrievedSubTask.IsComplete);
        }

        [Test]
        public async Task CannotDeleteTaskWithSubTasks()
        {
            // Arrange
            var task = new Task
            {
                Name = "Task with SubTask",
                Description = "Description",
                IsComplete = false
            };

            var taskResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            taskResponse.EnsureSuccessStatusCode();

            var createdTask = await taskResponse.Content.ReadFromJsonAsync<Task>();
            Assert.IsNotNull(createdTask);

            var subTask = new SubTask
            {
                TaskId = createdTask.Id,
                Name = "SubTask",
                Description = "SubTask Description",
                IsComplete = false
            };

            var subTaskResponse = await _client.PostAsJsonAsync("/api/subtasks", subTask);
            subTaskResponse.EnsureSuccessStatusCode();

            // Act
            var response = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task CanDeleteTaskWithoutSubTasks()
        {
            // Arrange
            var task = new Task
            {
                Name = "Task without SubTask",
                Description = "Description",
                IsComplete = false
            };

            var taskResponse = await _client.PostAsJsonAsync("/api/tasks", task);
            taskResponse.EnsureSuccessStatusCode();

            var createdTask = await taskResponse.Content.ReadFromJsonAsync<Task>();
            Assert.IsNotNull(createdTask);

            // Act
            var response = await _client.DeleteAsync($"/api/tasks/{createdTask.Id}");

            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}