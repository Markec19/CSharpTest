namespace CSharpTest.Data.Service
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Models.Employee>> GetAll(string key);
    }
}
