using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.XlsIO;
using System.IO;

namespace SyncfusionBridgeAPI.Controllers
{
    // Yeh define karta hai ki is Controller ko /api/convert par access kiya ja sakta hai
    [Route("api/[controller]")] 
    [ApiController]
    // Controller ka naam yahan 'Convert' hai, isliye URL /api/convert banega
    public class ConvertController : ControllerBase
    {
        // Yeh function (endpoint) aapka Python code call karega
        // Yeh IFormFile (file) aur string (target_format) data leta hai (jaisa humne Python mein plan kiya tha)
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
                // Input file ko memory mein padhna
                using (MemoryStream inputStream = new MemoryStream())
                {
                    file.CopyTo(inputStream);
                    inputStream.Position = 0; // Stream ko shuru mein laana

                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        if (target_format.ToLower() == "docx")
                        {
                            // --- PDF to Word Logic (Syncfusion) ---
                            // PdfDocument object banana input stream se
                            using (PdfDocument pdfDoc = new PdfDocument(inputStream))
                            {
                                // PDF ko Word (docx) format mein Export karna
                                pdfDoc.Export(outputStream, FormatType.Docx); 
                            }
                            // -------------------------
                        }
                        else if (target_format.ToLower() == "xlsx")
                        {
                            // --- PDF to Excel Logic (Syncfusion) ---
                            // PdfDocument object banana input stream se
                            using (PdfDocument pdfDoc = new PdfDocument(inputStream))
                            {
                                // PDF ko Excel (xlsx) format mein Export karna
                                // 'true' parameter 'table detection' ko enable karta hai
                                pdfDoc.Export(outputStream, ExcelVersion.Excel2016, true);
                            }
                            // --------------------------
                        }

                        // Output stream ko Python ko wapas bhej dena (binary file data)
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
                // Agar koi bhi error ho (jaise license issue ya file corruption), toh 500 status ke saath error message bhej dena
                return StatusCode(500, new { error = $"Conversion failed: {ex.Message}" });
            }
        }
    }
}