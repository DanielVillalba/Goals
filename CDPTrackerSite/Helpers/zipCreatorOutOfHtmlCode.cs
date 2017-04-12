using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace CDPTrackerSite.Helpers
{
    /// <summary>
    /// This class is a generic implementation to create a zip file bundle from a set of html pieces of code
    /// each html element should have a file name to be created.
    /// </summary>
    public class zipCreatorOutOfHtmlCode : ActionResult
    {
        private string Filename;
        private Dictionary<string, string> filesToConvert = new Dictionary<string, string>();

        public zipCreatorOutOfHtmlCode(string filename, Dictionary<string, string> filesData)
        {
            Filename = filename;
            filesToConvert = filesData;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var response = context.HttpContext.Response;
            response.ContentType = "application/gzip";
            using (var zip = new ZipFile())
            {
                foreach (string fileName in filesToConvert.Keys)
                {
                    zip.AddEntry(fileName, html2pdf.memoryPDF(filesToConvert[fileName]));
                }
                zip.Save(response.OutputStream);
                var cd = new ContentDisposition
                {
                    FileName = Filename,
                    Inline = false
                };
                response.Headers.Add("Content-Disposition", cd.ToString());
            }
        }
    }
}