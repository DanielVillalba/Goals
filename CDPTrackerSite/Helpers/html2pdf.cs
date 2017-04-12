using System;
using SelectPdf;
namespace CDPTrackerSite.Helpers
{
    public class html2pdf
    {
        public static byte[] memoryPDF(string html)
        {
            // read parameters from the webpage
            string baseUrl = "c:\\";

            string pdf_page_size = "Letter";
            PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), pdf_page_size, true);

            string pdf_orientation = "Portrait";
            PdfPageOrientation pdfOrientation = (PdfPageOrientation)Enum.Parse(
                typeof(PdfPageOrientation), pdf_orientation, true);

            int webPageWidth = 1024;
            int webPageHeight = 0;

            // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = pageSize;
            converter.Options.PdfPageOrientation = pdfOrientation;
            converter.Options.WebPageWidth = webPageWidth;
            converter.Options.WebPageHeight = webPageHeight;
            converter.Options.ViewerPreferences.PageLayout = PdfViewerPageLayout.SinglePage;

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(html, baseUrl);

            // save pdf document
            byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();
            return pdf;
        }
    }
}