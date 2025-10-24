using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf;
using Syncfusion.DocIO.DLS;
using Syncfusion.XlsIO;
using System.IO;

// Sirf zaroori classes ka istemal

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
                        using (PdfDocument pdfDoc = new PdfDocument(inputStream))
                        {
                            if (target_format.ToLower() == "docx")
                            {
                                // --- PDF to Word Logic (Using Direct Export) ---
                                // Yeh method original packages mein hai
                                pdfDoc.Export(outputStream, FormatType.Docx); 
                                // -------------------------
                            }
                            else if (target_format.ToLower() == "xlsx")
                            {
                                // --- PDF to Excel Logic (Using Direct Export) ---
                                // Yeh method original packages mein hai
                                pdfDoc.Export(outputStream, ExcelVersion.Excel2016, true);
                                // --------------------------
                            }
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