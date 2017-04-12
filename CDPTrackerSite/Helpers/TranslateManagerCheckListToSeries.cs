using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CDPTrackerSite.Models;

namespace CDPTrackerSite.Helpers
{
    public class TranslateManagerCheckListToSeries
    {
        public static List<GraphicSerie> To(List<List<ManagerCheck>> From)
        {
            if (From.Count > 0)
            {
                int questionsNumber = 0;
                foreach (var itemN in From)
                {
                    var MaxQuestion = itemN.Count();
                    if (MaxQuestion > questionsNumber)
                        questionsNumber = MaxQuestion;
                }

                    if (questionsNumber > 0)
                    {
                        GraphicSerie[] serie =  new GraphicSerie[questionsNumber];
                        for (int x = 0; x < serie.Length; x++)
                        {
                            serie[x] = new GraphicSerie();
                            serie[x].NameSerie = "Response";
                        }

                        foreach (var list in From)
                        {
                            if (list.Count > 0)
                            {
                                int i = 0;
                                foreach (var item in list)
                                { 
                                    serie[i].DataSerie.Add(item.ResponseId);
                                    i++;
                                }
                            }
                            else
                            {
                                for (int x = 0; x < questionsNumber; x++)
                                { 
                                    serie[x].DataSerie.Add(0);
                                }
                            }
                        }
                        return serie.ToList();
                    }
                    else
                    {
                    return new List<GraphicSerie>(); 
                    }
                
               
            }
            else
                return new List<GraphicSerie>();
        }

        public static GraphicSerie UnifyPerformancePromotionQuestion(GraphicSerie PromotionList, GraphicSerie PerformanceList)
        {
            GraphicSerie mixSerie = new GraphicSerie();
            mixSerie.NameSerie = "Response";
            for (var i = 0; i < PerformanceList.DataSerie.Count; i++)
            {
                var itemNew = 0;
                var itemper = PerformanceList.DataSerie.ElementAt(i);
                var itemPromo = PromotionList.DataSerie.ElementAt(i);
                if ((itemper + itemPromo) > 0)
                {
                    if (itemper == itemPromo)
                        itemNew = 14;
                    else if (itemper > itemPromo)
                        itemNew = 13;
                    else
                        itemNew = 15;
                }
                else
                    itemNew = 0;
                mixSerie.DataSerie.Add(itemNew);
            }
            return mixSerie;
        }
    }
}