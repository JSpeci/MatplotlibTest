using ENVIS.Model;
using MatplotlibCS;
using MatplotlibCS.PlotItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MatplotlibTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string path = "C:\\Users\\King\\Desktop\\EnvisBackUps\\2018-04.cea";
            MyDataReader reader = new MyDataReader();
            reader.LoadCea(path);

            List<UniArchiveBase> result = new List<UniArchiveBase>();

            for (int i = 0; i < 10000; i++)
            {
                result.Add(reader.LoadNext());
            }

            // values restricted to one day 1.4.2018
            var values = reader.values.Where(i => i.Key.Day == 1 && i.Key.Month == 4).Select(i => double.Parse(i.Value)).ToList(); // 8644 values for each 10 seconds
            var values2 = result.Select(i => double.Parse(i.GetMemberValue("I_avg_3I").ToString())).Take(8644).ToList();
            var values3 = result.Select(i => double.Parse(i.GetMemberValue("U_avg_U3").ToString())).Take(8644).ToList();

            var times2 = reader.values.Where(i => i.Key.Day == 1 && i.Key.Month == 4).Select(i => i.Key.Hour as object).ToList(); // 8644 values for each 10 seconds

            double[] X = new double[] { -10, -8.5, -2, 1, 6, 9, 10, 14, 15, 19 };
            double[] Y = new double[] { -4, 6.5, -2, 3, -8, -5, 11, 4, -5, 10 };
            var osaX = X.Select(i => i as object).ToList();

            string tempfolder = System.IO.Path.GetTempPath();
            tempfolder = "C:\\Users\\King\\Documents\\BP\\";

            string pythonExe = "C:\\Users\\King\\AppData\\Local\\Programs\\Python\\Python37\\python.exe";
            //string plotPath = "C:\\Users\\King\\source\\repos\\MatplotlibCS\\MatplotlibCS\\Python\\matplotlib_cs.py";
            string plotPath = "C:\\Users\\King\\source\\repos\\MatplotlibTest\\MatplotlibTest\\MatplotlibCS\\matplotlib_cs.py";
            var matplotLibEngine = new MatplotlibCS.MatplotlibCS(pythonExe, plotPath);

            var figure = new Figure(3, 1)
            {
                FileName = "P_avg_3P_C_fromMatplotLibCS.pdf",
                OnlySaveImage = true,
                DPI = 150,
                Width = 1920,
                Height = 1080,
                Subplots =
                {
                    new Axes(1, "dayhours", "P(W)")
                    {
                        Title = "1.4.2018",
                        LegendLocation = LegendLocation.UpperLeft,
                        Grid = new Grid()
                        {
                            MinorAlpha = 0.2,
                            MajorAlpha = 1.0,
                            XMajorTicks = new[] {0.0, 24.0, 1.0}, //min, max, step
                            YMajorTicks = new[] {0.0, 4200.0, 600.0},
                            //XMinorTicks = new[] {0.0, 7.25, 0.25},
                            //YMinorTicks = new[] {-1, 2.5, 0.125}
                        },
                        PlotItems =
                        {
                            new Line2D("řada P-avg-3P-C")
                            {
                                X = times2,
                                Y = values,
                                LineStyle = LineStyle.Solid,
                                Color = Color.Blue,
                            },

                            new Annotation("Arrow text annotation","Arrow text annotation example", 14.0, 1000.0,22.0, 1500.0)
                            {
                                Color = Color.Blue
                            },

                            //new Vline("vert line", new[] {3.0 as object}, -1, 1),
                            //new Hline("hrzt line", new[] {0.1, 0.25, 0.375}, 0, 5) {LineStyle = LineStyle.Dashed, Color = Color.Magenta}
                        }
                    },
                    new Axes(2, "dayhours", "I(A)")
                    {
                        Title= "",
                        LegendLocation = LegendLocation.UpperLeft,
                        Grid = new Grid()
                        {
                            MinorAlpha = 0.2,
                            MajorAlpha = 1.0,
                            XMajorTicks = new[] {0.0, 24.0, 1.0}, //min, max, step
                            YMajorTicks = new[] {0.0, 19.0, 5.0},
                            //XMinorTicks = new[] {0.0, 7.25, 0.25},
                            //YMinorTicks = new[] {-1, 2.5, 0.125}
                        },
                        PlotItems =
                        {
                            new Line2D("řada I-avg-3I")
                            {
                                X = times2,
                                Y = values2,
                                LineStyle = LineStyle.Solid,
                                Color = Color.Yellow,
                                LineWidth = 2.0F,
                            },

                            new Text("Named annotation","2px solid line - comment text 15pt", 11.0, 9.0)
                            {
                                FontSize = 15
                            },

                            //new Vline("vert line", new[] {3.0 as object}, -1, 1),
                            //new Hline("hrzt line", new[] {0.1, 0.25, 0.375}, 0, 5) {LineStyle = LineStyle.Dashed, Color = Color.Magenta}
                        }
                    },
                    new Axes(3, "dayhours", "U(V)")
                    {
                        Title= "",
                        LegendLocation = LegendLocation.UpperLeft,
                        Grid = new Grid()
                        {
                            MinorAlpha = 0.2,
                            MajorAlpha = 1.0,
                            XMajorTicks = new[] {0.0, 24.0, 1.0}, //min, max, step
                            YMajorTicks = new[] {220.0, 250.0, 5.0},
                            //XMinorTicks = new[] {0.0, 7.25, 0.25},
                            //YMinorTicks = new[] {-1, 2.5, 0.125}
                        },
                        PlotItems =
                        {
                            new Line2D("řada U-avg-U3")
                            {
                                X = times2,
                                Y = values3,
                                LineStyle = LineStyle.Solid,
                                Color = Color.Green,
                            },
                            new Text("Named annotation","horizontal and vertical lines - 2px", 11.0, 245.0)
                            {
                                FontSize = 13
                            },
                            new Vline("vertical line", new[] {3.5 as object}, 220.0, 250.0){LineStyle = LineStyle.Solid, Color = Color.Magenta, LineWidth = 2F},
                            new Hline("hrzt line", new[] {232.0}, 0, 23.0) {LineStyle = LineStyle.Dashed, Color = Color.Magenta}
                        }
                    }

                }
            };

            //act
            var t = matplotLibEngine.BuildFigure(figure);
            t.Wait();
        }
    }
}
