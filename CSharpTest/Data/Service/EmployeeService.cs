using CSharpTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;

namespace CSharpTest.Data.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HttpClient _httpClient;

        public EmployeeService(HttpClient httpsClient)
        {
            _httpClient = httpsClient;
        }

        public async Task<IEnumerable<Employee>> GetAll(string key)
        {
            var url = $"https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code={key}";
            var json = await _httpClient.GetStringAsync(url);
            var allEmployees = JsonSerializer.Deserialize<List<Employee>>(json);

            var groupedEmployees = allEmployees
                .Where(e => e.DeletedOn == null && e.EmployeeName != null 
                        && e.StarTimeUtc < e.EndTimeUtc)
                .GroupBy(e => e.EmployeeName)
                .Select(g => 
                {
                    double totalHours = g.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).TotalHours);
                    return new Employee
                    {
                        EmployeeName = g.Key,
                        TimeWorked = Math.Floor(totalHours)
                    };
                }).OrderByDescending(e => e.TimeWorked) 
                .ToList();


            return groupedEmployees;
        }
    }
}
