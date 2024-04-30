using Confluent.Kafka;
using EmployeeApplication.Database;
using EmployeeApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EmployeeApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger, EmployeeDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetEmployees")]
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            _logger.LogInformation("Requesting for employees");

            return await _context.Employees.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(string name, string surName)
        {
            var employee = new Employee(Guid.NewGuid(), name, surName);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            var message = new Message<string, string>()
            {
                Key = employee.Id.ToString(),
                Value = JsonSerializer.Serialize(employee)
            };
            //client

            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092",
                Acks = Acks.All,
                
            };


            var producer = new ProducerBuilder<string, string>(producerConfig).Build();
            await producer.ProduceAsync("employeeTopic", message);
            producer.Dispose();
            return CreatedAtAction(nameof(CreateEmployee), new { id = employee.Id }, employee);
        }
    }
}
