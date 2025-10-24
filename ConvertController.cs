using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf;
// FIX: Sahi using statements
using Syncfusion.PdfToWordConverter; 
using Syncfusion.PdfToExcelConverter; 
using Syncfusion.DocIO.DLS;
using Syncfusion.XlsIO;
using System.IO;

namespace SyncfusionBridgeAPI.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class ConvertController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromForm] IFormFile file, [FromForm] string target_format)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file received." });
            }

            if (string.IsNullOrEmpty(target_format) || (target_format.ToLower() != "docx" && target_format.ToLower() != "xlsx"))
            {
                return BadRequest(new { error = "Invalid or missing target_format (must be docx or xlsx)." });
            }

            try
            {
                using (MemoryStream inputStream = new MemoryStream())
                {
                    file.CopyTo(inputStream);
                    inputStream.Position = 0; 

                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        if (target_format.ToLower() == "docx")
                        {
                            // --- PDF to Word Logic (FIXED) ---
                            using (PdfToWordConverter converter = new PdfToWordConverter(inputStream))
                            {
                                converter.Convert(outputStream, FormatType.Docx); 
                            }
                            // -------------------------
                        }
                        else if (target_format.ToLower() == "xlsx")
                        {
                            // --- PDF to Excel Logic (FIXED) ---
                            using (PdfToExcelConverter converter = new PdfToExcelConverter(inputStream))
                            {
                                // Table detection ko enable karna
                                converter.Settings.AutoDetectTables = true; 
                                converter.Convert(outputStream);
                            }
                            // --------------------------
                        }

                        outputStream.Position = 0;
                        string contentType = target_format.ToLower() == "docx" 
                                           ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" 
                                           : "application/vnd.openxmlformats-officedocument.spreadsheetml.document";
                        
                        return File(outputStream.ToArray(), contentType, $"output.{target_format}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Conversion failed: {ex.Message}" });
            }
        }
    }
}