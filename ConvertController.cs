using Microsoft.AspNetCore.Mvc;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf.Imaging;
using Syncfusion.Pdf.Parsing;
using Syncfusion.XlsIO;
using System.IO;

namespace SyncfusionBridgeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertController : ControllerBase // Class ka naam aapki file ke naam se match hona chahiye
    {
        [HttpPost("ConvertToWordAdvanced")]
        public IActionResult ConvertToWordAdvanced(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File upload nahi hui.");
            }

            // PDF document load karein
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(file.OpenReadStream());
            
            // Naya Word document banayein
            WordDocument document = new WordDocument();

            // Har page ko image bana kar Word document mein shamil karein
            for (int i = 0; i < loadedDocument.Pages.Count; i++)
            {
                IWSection section = document.AddSection();
                Stream imageStream = loadedDocument.Pages[i].ConvertToImage(PdfImageType.Bitmap);
                IWPicture picture = section.AddParagraph().AppendPicture(imageStream);

                section.PageSetup.PageSize = new Syncfusion.Drawing.SizeF(loadedDocument.Pages[i].Size.Width, loadedDocument.Pages[i].Size.Height);
                section.PageSetup.Margins.All = 0;
                picture.Width = section.PageSetup.ClientWidth;
                picture.Height = section.PageSetup.ClientHeight;
            }

            loadedDocument.Close(true);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            document.Close();
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "AdvancedConvertedDocument.docx");
        }

        [HttpPost("ConvertToExcelAdvanced")]
        public IActionResult ConvertToExcelAdvanced(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File upload nahi hui.");
            }

            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(file.OpenReadStream());
            PdfTableExtractor extractor = new PdfTableExtractor(loadedDocument);
            PdfTable[] pdfTables = extractor.ExtractTable();

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;
                IWorkbook workbook = application.Workbooks.Create(pdfTables.Length > 0 ? pdfTables.Length : 1);
                
                if (pdfTables.Length > 0)
                {
                    for (int i = 0; i < pdfTables.Length; i++)
                    {
                        IWorksheet worksheet = workbook.Worksheets[i];
                        worksheet.Name = $"Table {i + 1}";
                        for (int row = 0; row < pdfTables[i].RowCount; row++)
                        {
                            for (int col = 0; col < pdfTables[i].ColumnCount; col++)
                            {
                                worksheet.Range[row + 1, col + 1].Text = pdfTables[i][row, col].GetText();
                            }
                        }
                    }
                }
                else
                {
                    IWorksheet worksheet = workbook.Worksheets[0];
                    worksheet.Range["A1"].Text = "Is PDF document mein koi table nahi mila.";
                }

                MemoryStream stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;
                loadedDocument.Close(true);

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AdvancedConvertedDocument.xlsx");
            }
        }
    }
}