using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace ElectricFieldVis
{
    public partial class Graph : Form
    {
        public Graph(ObservableCollection<double> x, ObservableCollection<string> y)
        {
            InitializeComponent();
            InitializeBarChart(x, y);
        }

        /// <summary>
        /// Creates a chart which measures the intensity of electrostatic field at a certain time
        /// </summary>
        /// <param name="values"> Intensity of electrostatic field </param>
        /// <param name="time"> Time of measure </param>
        private void InitializeBarChart(ObservableCollection<double> values, ObservableCollection<string> time)
        {
            var series = new List<ISeries>(values.Count);
            var cd = new LineSeries<double>()
            {
                Name = "Intensity[GN/C]",
                Values = values,
                DataLabelsPaint = new SolidColorPaint(SKColors.AliceBlue),
                DataLabelsSize = 9,
                DataLabelsFormatter = (x) => $"{x.Coordinate.PrimaryValue: #.##}GN/C",
                YToolTipLabelFormatter = (x) => $"{x.Coordinate.PrimaryValue: #.##}GN/C",
                Fill = null,
            };
            series.Add(cd);

            this.cartesianChart1.Series = series;
            this.cartesianChart1.XAxes =
            [
                new Axis
                {
                    Name = "Time",
                    ForceStepToMin = true,
                    MinStep = 10,
                }
            ];
            // Configure the X-axis with time values
            this.cartesianChart1.XAxes = new List<Axis>
            {
                new Axis
                {
                    Labels = time, // Convert float to string for labels
                    Name = "Time [s]"
                }
            };
            this.cartesianChart1.YAxes =
            [
                new Axis
                {
                    Name = "Intensity [GN/C]"
                }
            ];

            this.cartesianChart1.Title = new LabelVisual()
            {
                Text = "Intensity of the field at probes position",
                TextSize = 20
            };
            this.cartesianChart1.LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom;
        }

        private void cartesianChart1_Load(object sender, EventArgs e)
        {

        }
    }
}
