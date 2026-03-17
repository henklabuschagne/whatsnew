using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhatsNewAPI.Services;

namespace WhatsNewAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportExportController : ControllerBase
    {
        private readonly IExcelService _excelService;

        public ImportExportController(IExcelService excelService)
        {
            _excelService = excelService;
        }

        [HttpPost("import/excel")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                {
                    return BadRequest(new { message = "File must be an Excel file (.xlsx or .xls)" });
                }

                using (var stream = file.OpenReadStream())
                {
                    var result = await _excelService.ImportFromExcelAsync(stream);
                    
                    if (result.Success)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during import", error = ex.Message });
            }
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                var fileBytes = await _excelService.ExportToExcelAsync();
                var fileName = $"WhatsNew_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during export", error = ex.Message });
            }
        }

        [HttpGet("template/excel")]
        public IActionResult DownloadTemplate()
        {
            try
            {
                using (var package = new OfficeOpenXml.ExcelPackage())
                {
                    OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    
                    var worksheet = package.Workbook.Worksheets.Add("Releases");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "Version";
                    worksheet.Cells[1, 2].Value = "Release Date";
                    worksheet.Cells[1, 3].Value = "Change Type";
                    worksheet.Cells[1, 4].Value = "Description";
                    worksheet.Cells[1, 5].Value = "Module Tags";

                    // Style headers
                    using (var range = worksheet.Cells[1, 1, 1, 5])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Add example row
                    worksheet.Cells[2, 1].Value = "2.1.0";
                    worksheet.Cells[2, 2].Value = DateTime.Now.ToString("yyyy-MM-dd");
                    worksheet.Cells[2, 3].Value = "New Feature";
                    worksheet.Cells[2, 4].Value = "Added user authentication";
                    worksheet.Cells[2, 5].Value = "security, user-interface";

                    worksheet.Cells[3, 1].Value = "2.1.0";
                    worksheet.Cells[3, 2].Value = DateTime.Now.ToString("yyyy-MM-dd");
                    worksheet.Cells[3, 3].Value = "Bug Fix";
                    worksheet.Cells[3, 4].Value = "Fixed login page styling";
                    worksheet.Cells[3, 5].Value = "user-interface";

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    var fileBytes = package.GetAsByteArray();
                    var fileName = "WhatsNew_Template.xlsx";

                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred generating template", error = ex.Message });
            }
        }
    }
}
