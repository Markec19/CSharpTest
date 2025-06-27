using CSharpTest.Data.Service;
using Microsoft.AspNetCore.Mvc;

namespace CSharpTest.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;       
        private readonly string _apiKey;

        public EmployeeController(IEmployeeService employeeService, IConfiguration configuration)
        {
            _apiKey = configuration["ApiSettings:ApiKey"];
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAll(_apiKey);
            return View(employees);
        }

        public async Task<IActionResult> PieChart()
        {
            var employees = await _employeeService.GetAll(_apiKey);

            var imageBytes = ChartGenerator.GeneratePieChart(employees);
            return File(imageBytes, "image/png");
        }

    }
}
