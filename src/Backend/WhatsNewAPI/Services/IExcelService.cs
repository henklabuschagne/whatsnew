using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WhatsNewAPI.DTOs;

namespace WhatsNewAPI.Services
{
    public interface IExcelService
    {
        Task<ExcelImportResultDto> ImportFromExcelAsync(Stream excelStream);
        Task<byte[]> ExportToExcelAsync();
    }
}
