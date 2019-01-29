﻿using MatplotlibCS;
using MatplotlibCS.PlotItems;
using System;
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

            for (int i = 0; i < 10000; i++)
            {
                var loadedRow = reader.LoadNext();
            }

            // values restricted to one day 1.4.2018
            var times = reader.values.Where(i => i.Key.Day == 1 && i.Key.Month == 4).Select(i => double.Parse(i.Key.Hour.ToString())).ToArray(); // 8644 values for each 10 seconds
            var values = reader.values.Where(i => i.Key.Day == 1 && i.Key.Month == 4).Select(i => double.Parse(i.Value)).ToList(); // 8644 values for each 10 seconds
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

            var figure = new Figure(1, 1)
            {
                FileName = "P_avg_3P_C_fromMatplotLibCS.png",
                OnlySaveImage = true,
                DPI = 150,
                Subplots =
                {
                    new Axes(1, "The X axis", "The Y axis")
                    {
                        Title = "Sin(x), Sin(2x), VLines, HLines, Annotations",
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
                            new Line2D("P(W)")
                            {
                                X = times2,
                                Y = values,
                                LineStyle = LineStyle.Solid,
                                Color = Color.Blue,
                            },

                            //new Text("Text annotation","", 4.5, 0.76)
                            //{
                            //    FontSize = 17
                            //},

                            //new Annotation("Arrow text annotation","", 0.5, -0.7, 3, 0)
                            //{
                            //    Color = Color.Blue
                            //},

                            //new Vline("vert line", new[] {3.0 as object}, -1, 1),
                            //new Hline("hrzt line", new[] {0.1, 0.25, 0.375}, 0, 5) {LineStyle = LineStyle.Dashed, Color = Color.Magenta}
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
