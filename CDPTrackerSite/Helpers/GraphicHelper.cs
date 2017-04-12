using System;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace CDPTrackerSite.Helpers
{
    public enum GraphicType
    {
        AgreeDisagree,
        YesNo,
        PerformPromotion,
        ABC
    }

    public enum GraphicSize
    {
        Main,
        Trend
    }


    public class GraphicSerie
    {
        public GraphicSerie ()
        {
            DataSerie = new List<double>();
        }

        public String NameSerie { get; set; }
        public List<double> DataSerie { get; set; }
    }

    public class GraphicSerieWithLeyend
    {
        public String NameSerie { get; set; }
        public List<PointXYGraphic>  Point {get; set;}
    }

    public class PointXYGraphic
    {
        public string Name { get; set; }
        public int value { get; set; }
    }

    public class GraphicHelper
    {
        private Chart chartBox;  
        private GraphicType _graphicType;
        private GraphicSize _graphicSize;
        private SeriesChartType _chartType;
        private GraphicSerie  _dataList;
        private List<GraphicSerieWithLeyend> _dataListWithLegend;
        private List<String> _quarterLabels;

        //  private string _nameSeries = "Response";
        private string _title = "Great Results";

        public GraphicType GraphicType
        {
            get { return _graphicType; }
            set { _graphicType = value; }
        }

        public GraphicSize GraphicSize
        {
            get { return _graphicSize; }
            set { _graphicSize = value; }
        }

        public GraphicSerie   DataLists
        {
            get { return _dataList; }
            set { _dataList = value; }
        }

        public List<GraphicSerieWithLeyend> DataListsLeyend
        {
            get { return _dataListWithLegend; }
            set { _dataListWithLegend = value; }
        }

        public String GraphicTitle
        {
            get { return _title;}
            set { _title = value;  }
        }

        public List<String> QuarterLabels
        {
            get { return _quarterLabels; }
            set { _quarterLabels = value; }
        }


        public GraphicHelper()   
        {
            chartBox = new Chart();
            _graphicType = GraphicType.AgreeDisagree;
        }
        
        public MemoryStream GetGraphic ()
        {
            if (_dataList == null)
            {
                if (_dataListWithLegend == null)
                    return null;
                else
                    if (_dataListWithLegend.Count == 0)
                        return null;
            }
            else
            {
                if(_dataList.DataSerie.Count == 0)
                    if (_dataListWithLegend == null)
                        return null;
                    else
                    if (_dataListWithLegend.Count == 0)
                        return null;
            }

            

            if (_graphicSize == GraphicSize.Trend)
            {

                chartBox.CustomizeLegend += ChartBox_CustomizeLegend;
                chartBox.Width = 450;
                chartBox.Height = 250;
                _chartType = SeriesChartType.Point;
            }
            else
            {
                chartBox.CustomizeLegend += ChartBox_CustomizeLegendXAxis;
                chartBox.Width = 900;
                chartBox.Height = 500;
                _chartType = SeriesChartType.Bar;

            }


            chartBox.BackColor = Color.White; //Color.FromArgb(211, 223, 240);
            chartBox.BorderlineDashStyle = ChartDashStyle.Solid;
            chartBox.BackSecondaryColor = Color.White;
            chartBox.BackGradientStyle = GradientStyle.LeftRight;
            chartBox.BorderlineWidth = 1;

             chartBox.Titles.Add("Default");
            chartBox.Titles[0].Text = _title;
         
            chartBox.Titles[0].Font = new Font("Verdana,Arial,Helvetica,sans-serif", 14F, FontStyle.Regular);


        



            chartBox.ChartAreas.Add(CreateChartArea("ChartArea"));

            if (_dataListWithLegend != null)
            {
                foreach (var data in _dataListWithLegend)
                {
                    var NameSerie = data.NameSerie;
                    chartBox.Series.Add(NameSerie);
                   // chartBox.Series[NameSerie].ChartType = _chartType;

                    // Set marker size
                    chartBox.Series[NameSerie].MarkerSize = 12;

                    foreach (var item in data.Point)
                    {
                        chartBox.Series[NameSerie].Points.AddXY(item.Name, item.value);
                    }

                    if (_chartType == SeriesChartType.Point)
                        chartBox.Series[NameSerie].MarkerStyle = MarkerStyle.Diamond;

                    chartBox.Series[NameSerie].ChartArea = "ChartArea";
                    chartBox.Legends.Add(CreateLegend(NameSerie));
                }
            }
            else
            {
                var NameSerie = _dataList.NameSerie;
                chartBox.Series.Add(NameSerie);
                chartBox.Series[NameSerie].ChartType = _chartType;

                // Set marker size
                chartBox.Series[NameSerie].MarkerSize = 12;

                foreach (var item in _dataList.DataSerie)
                {
                    if(item > 0)
                        chartBox.Series[NameSerie].Points.AddY(item);
                    else
                    {
                        var emptyDataPoint = new DataPoint();
                        emptyDataPoint.IsEmpty = true;
                        chartBox.Series[NameSerie].Points.Add(emptyDataPoint);
                    }
                }

                if (_chartType == SeriesChartType.Point)
                    chartBox.Series[NameSerie].MarkerStyle = MarkerStyle.Diamond;
                
                chartBox.Series[NameSerie].ChartArea = "ChartArea";
                chartBox.Legends.Add(CreateLegend(NameSerie));
               
            }

            MemoryStream ms = new MemoryStream();
            chartBox.SaveImage(ms);
            return ms;
        }

        private ChartArea CreateChartArea(String AreaName)
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = AreaName;
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            //chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.Enabled = false;
            //chartArea.AxisX.TitleFont = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.IsSameFontSizeForAllAxes = false;
            chartArea.AxisX.Interval = 1;
            chartArea.AxisY.Interval = 1;
       
            if (_graphicSize == GraphicSize.Trend)
            {
                chartArea.AxisX.Minimum = 0;
                chartArea.AxisX.Maximum = 4;
            }
            else
            {
                chartArea.AxisX.IsLabelAutoFit = true;
                chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont | LabelAutoFitStyles.StaggeredLabels | LabelAutoFitStyles.WordWrap;
            }

            if (_graphicType == GraphicType.AgreeDisagree)
            {
                chartArea.AxisY.Minimum = 0;
                chartArea.AxisY.Maximum = 5;
            }
            else if (_graphicType == GraphicType.YesNo)
            {   
                chartArea.AxisY.Minimum = 5;
                chartArea.AxisY.Maximum = 8;
            }
            else if (_graphicType == GraphicType.ABC)
            {
                chartArea.AxisY.Minimum = 7;
                chartArea.AxisY.Maximum = 11;
            }
            else if(_graphicType== GraphicType.PerformPromotion)
            {
                chartArea.AxisY.Maximum = 16;
                chartArea.AxisY.Minimum = 12;
            }
             

            return chartArea;
        }

        private Legend CreateLegend(String NewLegend)
        {
            Legend legend = new Legend();
            legend.Name = NewLegend;
            legend.Docking = Docking.Top;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 10F, FontStyle.Regular);
            legend.LegendStyle = LegendStyle.Row;

            return legend;
        }

        private void ChartBox_CustomizeLegend(object sender, CustomizeLegendEventArgs e)
        {

            CustomLabelsCollection yAxisLabels = chartBox.ChartAreas["ChartArea"].AxisY.CustomLabels;

            if (_graphicType == GraphicType.AgreeDisagree)
            {
                //yAxisLabels.RemoveAt(0);
                //yAxisLabels[0].Text = String.Empty;
                yAxisLabels[0].Text = String.Empty;
                yAxisLabels[1].Text = "Strongly Disagree";
                yAxisLabels[2].Text = "Disagree";
                yAxisLabels[3].Text = "Neutral";
                yAxisLabels[4].Text = "Agree";
                yAxisLabels[5].Text = "Strongly Agree";
            }
            else if(_graphicType == GraphicType.YesNo )
            {
                    yAxisLabels[0].Text = String.Empty;
                    yAxisLabels[1].Text = "Yes";
                    yAxisLabels[2].Text = "No";
                    yAxisLabels[3].Text = string.Empty;


            } else if (_graphicType == GraphicType.ABC)
            {
                yAxisLabels[0].Text = String.Empty;
                yAxisLabels[1].Text = "A";
                yAxisLabels[2].Text = "B";
                yAxisLabels[3].Text = "C";
                yAxisLabels[4].Text = "NA";


            }
            else if (_graphicType == GraphicType.PerformPromotion)
            {
                yAxisLabels[0].Text = String.Empty;
                yAxisLabels[1].Text = "Promotion Ready";
                yAxisLabels[2].Text = "Not Promotion";
                yAxisLabels[3].Text = "Performance problem";
                yAxisLabels[4].Text = String.Empty;
            }

            CustomLabelsCollection xAxisLabels = chartBox.ChartAreas["ChartArea"].AxisX.CustomLabels;
            xAxisLabels[0].Text = String.Empty;

            int x = 1;
            foreach(var item in _quarterLabels)
            {
                xAxisLabels[x].Text = item;
                x++;
            }
        }

        private void ChartBox_CustomizeLegendXAxis(object sender, CustomizeLegendEventArgs e)
        {

            CustomLabelsCollection yAxisLabels = chartBox.ChartAreas["ChartArea"].AxisY.CustomLabels;

            if (_graphicType == GraphicType.AgreeDisagree)
            {
                yAxisLabels[0].Text = String.Empty;
                yAxisLabels[1].Text = "Strongly Disagree";
                yAxisLabels[2].Text = "Disagree";
                yAxisLabels[3].Text = "Neutral";
                yAxisLabels[4].Text = "Agree";
                yAxisLabels[5].Text = "Strongly Agree";
            }
             
            
        }
    }
}