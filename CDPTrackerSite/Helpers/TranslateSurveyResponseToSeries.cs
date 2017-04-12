using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDPTrackerSite.Models;
using CDPTrackerSite.Helpers;
namespace CDPTrackerSite.Helpers
{
    public class TranslateSurveyResponseToSeries
    {
        public static GraphicSerieWithLeyend TranslateTo(SurveyResponseAndIdentifierModel from)
        {

            List<PointXYGraphic> ListPoints = new List<PointXYGraphic>();
            foreach(var item in from.SurveyResponse)
            {
                var onePoint = new PointXYGraphic()
                {
                    Name = item.Text,
                    value = item.ResponseId
                };
                ListPoints.Add(onePoint);
            }
            GraphicSerieWithLeyend To = new GraphicSerieWithLeyend()
            {
                 NameSerie = from.Identifier,
                  Point = ListPoints
            };

            return To;
        }
    }
}