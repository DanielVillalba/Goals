using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 
using CDPTrackerSite.Models;
using System.IO;
using System.Text;
using Ionic.Zip;
using DataSource;

namespace CDPTrackerSite.Helpers
{
    public class GraphicsServerController
    {
        const string ImgElementOpen = @"<div><img src='data:image/png;base64,";
        const String ImgElementClose = @"' scale='0' ></div>";

        StringBuilder html = new StringBuilder();
         
        public List<SurveyResponseAndIdentifierModel> ListResponsesByResource { get; set; }
        public List<List<ManagerCheck>> ListResponsesManagerCheck { get; set; }
        public List<String> QuarterLabels { get; set; }

        #region Fill Graphics
        public string FillMainGraphic (int resource)
        {
            GraphicHelper helper = new GraphicHelper();
            helper.GraphicSize = GraphicSize.Main;
            helper.GraphicType = GraphicType.AgreeDisagree;
         
            helper.GraphicTitle = String.Empty;
            var dataSeries = new List<GraphicSerieWithLeyend>();
            foreach ( var item in ListResponsesByResource)
            {
                var dataQ  = TranslateSurveyResponseToSeries.TranslateTo(item);
                dataSeries.Add(dataQ);
            }
            if (dataSeries.Count > 0)
            {
                helper.DataListsLeyend = dataSeries;
                byte[] byteArr = helper.GetGraphic().ToArray();
                return Convert.ToBase64String(byteArr);
            }
            else
                return string.Empty;
        }


        public string [] FillTrends(int resource)
        {
            String[] Images = new string[4]; 
            var dataSeries = TranslateManagerCheckListToSeries.To(ListResponsesManagerCheck);
            if (dataSeries.Count > 0)
            {
                var seriePerformancePromotion = TranslateManagerCheckListToSeries.UnifyPerformancePromotionQuestion(dataSeries.Skip(2).First(), dataSeries.Skip(3).First());
                Images[0] = FillTrendGraphicAgreeDisagree(dataSeries.FirstOrDefault());
                Images[1] = FillTrendGraphicYesNo(dataSeries.Skip(1).FirstOrDefault());
                Images[2] = FillTrendGraphicPerformancePromotion(seriePerformancePromotion);
                Images[3] = FillTrendGraphicABC(dataSeries.Skip(4).FirstOrDefault());
                
            }
            return Images;
        }

        public string FillTrendGraphicAgreeDisagree  ( GraphicSerie  serie, string title = "Great Results")
        {
            GraphicHelper helper = new GraphicHelper();
            helper.QuarterLabels = this.QuarterLabels;
            helper.GraphicSize = GraphicSize.Trend;
            helper.GraphicType = GraphicType.AgreeDisagree;
            helper.GraphicTitle = title;
            helper.DataLists = serie;
            byte[] byteArr = helper.GetGraphic().ToArray();
            return Convert.ToBase64String(byteArr);
        }

        public string FillTrendGraphicYesNo(GraphicSerie serie, string title = "Choose to work with")
        {
            GraphicHelper helper = new GraphicHelper();
            helper.QuarterLabels = this.QuarterLabels;
            helper.GraphicSize = GraphicSize.Trend;
            helper.GraphicType = GraphicType.YesNo;
            helper.GraphicTitle = title;
            helper.DataLists = serie;
            byte[] byteArr = helper.GetGraphic().ToArray();
            return Convert.ToBase64String(byteArr);
        }

        public string FillTrendGraphicABC(GraphicSerie serie, string title ="Category")
        {
            GraphicHelper helper = new GraphicHelper();
            helper.QuarterLabels = this.QuarterLabels;
            helper.GraphicSize = GraphicSize.Trend;
            helper.GraphicType = GraphicType.ABC;
            helper.GraphicTitle = title;
            helper.DataLists = serie;
            byte[] byteArr = helper.GetGraphic().ToArray();
            return Convert.ToBase64String(byteArr);
        }

        public string FillTrendGraphicPerformancePromotion(GraphicSerie serie, string title = "Promotion ready / performance problem")
        {
            GraphicHelper helper = new GraphicHelper();
            helper.QuarterLabels = this.QuarterLabels;
            helper.GraphicSize = GraphicSize.Trend;
            helper.GraphicType = GraphicType.PerformPromotion;
            helper.GraphicTitle = title;
            helper.DataLists = serie;
            byte[] byteArr = helper.GetGraphic().ToArray();
            return Convert.ToBase64String(byteArr);
        }

        #endregion


        #region html labels
        public string CreateHtmlElement (string image)
        {
            var element = new StringBuilder();
            element.Append(ImgElementOpen);
            element.Append(image);
            element.Append(ImgElementClose);
            return element.ToString();
        }

        public String CreateHtmlMain (String Title, String MainImage)
        {
            var element = new StringBuilder();
            element.Append("<Table style='font-family:Verdana,Arial,Helvetica,sans-serif;'>");
            element.Append("<tr>");
            element.Append("<th style= 'font-size: 18px;'>");
            element.Append(Title);
            element.Append("</th>");
            element.Append("<tr>");
            element.Append("<td>");
            element.Append(MainImage);
            element.Append("</td>");
            element.Append("</tr>");

            return element.ToString();
        }

        public string CreateElementTrends (string [] images)
        {
            var element = new StringBuilder();
            if (!String.IsNullOrEmpty(images[0])){
                element.Append("<tr>");
                element.Append("<td>");
                element.Append("<Table>");
                element.Append("<tr>");
                element.Append("<td>");
                element.Append(CreateHtmlElement(images[0]));
                element.Append("</td>");
                element.Append("<td>");
                element.Append(CreateHtmlElement(images[1]));
                element.Append("</td>");
                element.Append("</tr>");
                element.Append("<tr>");
                element.Append("<td>");
                element.Append(CreateHtmlElement(images[2]));
                element.Append("</td>");
                element.Append("<td>");
                element.Append(CreateHtmlElement(images[3]));
                element.Append("</td>");
                element.Append("</tr>");
                element.Append("</Table>");
                element.Append("</td>");
                element.Append("</tr>");
            }
            return element.ToString();
        }

        public string CreateHtmlElementsComments (string commentText, bool IsEmployeComments)
        {
            // Cierre de tags principales
            StringBuilder element = new StringBuilder();
            element.Append("<tr>");
            element.Append("<td width='80%'>");
            element.Append("<Table>");
            if (IsEmployeComments)
            {
                element.Append("<tr>");
                element.Append("<td width='10%'>");
                element.Append("</td>");
                element.Append("<td width='80%'><b>");
                element.Append("Employee Comments: ");
                element.Append("</b></td>");
                element.Append("<td>");
                element.Append("</td>");
                element.Append("</tr>");
            }
            else
            {
                element.Append("<tr>");
                element.Append("<td width='10%'>");
                element.Append("</td>");
                element.Append("<td width='80%'><b>");  
                element.Append("Manager Comments: ");
                element.Append("</b></td>");
                element.Append("</tr>");
            }
            element.Append("<tr>");
            element.Append("<td width='10%'>");
            element.Append("</td>");
            element.Append("<td width='80%'>");       
            element.Append(commentText);
            element.Append("</td>");
            element.Append("<td>");
            element.Append("</td>");
            element.Append("</tr>");

            element.Append("</Table>");
            element.Append("</td>");
            element.Append("<td>");
            element.Append("</td>");
            element.Append("</tr>");
            return element.ToString();
        }

        public string CreateEndMainTags()
        {
            // Cierre de tags principales
            StringBuilder element = new StringBuilder();
            element.Append("</Table>");
            return element.ToString();
        }

        #endregion

    }
}