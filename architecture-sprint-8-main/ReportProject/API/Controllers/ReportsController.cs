using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace ReportProject.Controllers
{
    [Authorize(Roles = "prothetic_user")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        [HttpGet]        
        public IActionResult GetReport()
        {
            // Генерация произвольного отчета
            var reportBytes = GenerateReport();

            // Возврат файла        
            return File(reportBytes, "application/json", "report.json");
        }

        public record JsonReport(int Year, int Month, int Day, string Title);

        private byte[] GenerateReport()
        {
            // Сериализация объекта в строку JSON
            JsonReport r = new(15, 1, 2025, "Test report");
            string jsonString = JsonSerializer.Serialize(r);
            // Преобразование строки JSON в массив байт
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            return byteArray;
        }
    }
}
