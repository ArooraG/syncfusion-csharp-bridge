using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing; // FIX: PdfLoadedDocument ke liye zaroori
using Syncfusion.DocIO;       // FIX: FormatType ke liye zaroori
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

            // Target format (docx ya xlsx) check karna
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
                        // FIX: PdfLoadedDocument ka istemal
                        using (PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputStream))
                        {
                            if (target_format.ToLower() == "docx")
                            {
                                // FIX: PdfLoadedDocument ka Export method aur FormatType.Docx
                                loadedDocument.Export(outputStream, FormatType.Docx); 
                            }
                            else if (target_format.ToLower() == "xlsx")
                            {
                                // FIX: PdfLoadedDocument ka Export method
                                loadedDocument.Export(outputStream, ExcelVersion.Excel2016, true);
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