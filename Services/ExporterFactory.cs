using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using tema2mvc.Models;

namespace tema2mvc.Services
{
    public class ExporterFactory
    {
        public static IExporter CreateExcelExporter(IDataAccess<MenuItemDAO> items)
        {
            return new ExcelExporter(items);
        }

        public static IExporter CreateCsvExporter(IDataAccess<MenuItemDAO> items)
        {
            return new CsvExporter(items);
        }
    }
}
