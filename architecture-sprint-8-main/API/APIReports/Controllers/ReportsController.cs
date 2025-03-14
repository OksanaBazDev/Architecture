using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Text;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReportsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "ProtheticUserPolicy")] // ��������� ���� prothetic_user
        public IActionResult GetReport()
        {
            // ��������� ������������� ������
            var reportBytes = GenerateReport();
            
            // ������� �����        
            return File(reportBytes, "application/json", "report.json");            
        }

        public record JsonReport(int Year, int Month, int Day, string Title);

        private byte[] GenerateReport()
        {
            // ������������ ������� � ������ JSON
            JsonReport r = new(15,1, 2025, "Test report");
            string jsonString = JsonSerializer.Serialize(r);
            // �������������� ������ JSON � ������ ����
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonString);
            return byteArray;
        }
    }
}