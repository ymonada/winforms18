using System.Drawing.Configuration;
using System.Text.RegularExpressions;

namespace WinFormsApp18
{
    public partial class Form1 : Form
    {
        private int pointRadius = 10;
        private Color pointColor = Color.FromArgb(101, 55, 0);
        private Color isoscelesColor = Color.FromArgb(134, 134, 90);
        private Color obtusesColor = Color.FromArgb(93, 193, 10);
        private string filePath = "C:\\Users\\sukhy\\source\\repos\\WinFormsApp18\\WinFormsApp18\\file.txt";
        private string resultPath = "C:\\Users\\sukhy\\source\\repos\\WinFormsApp18\\WinFormsApp18\\result.txt";
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
        private List<Point> FileRead(string filepath)
        {
            var result = new List<Point>();
            try
            {
                string content = File.ReadAllText(filepath);
                string pattern = @"\d+ \d+";
                Regex regex = new Regex(pattern);
                MatchCollection matches = regex.Matches(content);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        var res = match.Value.Split(" ");
                        result.Add(
                            new Point(
                                int.Parse(res[0])
                                , int.Parse(res[1])));
                    }
                }
            }
            catch
            {
                throw new Exception("Error to read file");
            }
            return result;
        }
        private void WriteToFile(List<Point> iscl, List<Point> obts, string filepath) 
        {
            var res = $"""
                Рівнобедрений
                : ({iscl[0].X} {iscl[0].Y})
                : ({iscl[1].X} {iscl[1].Y})
                : ({iscl[2].X} {iscl[2].Y})
                Тупокутний
                : ({obts[0].X} {obts[0].Y})
                : ({obts[1].X} {obts[1].Y})
                : ({obts[2].X} {obts[2].Y})
                """;
            try
            {
                File.WriteAllText(filepath, res);
            }
            catch
            {
                throw new Exception("Error write to file");
            }
        }
        private void DrawingPoints(List<Point> points, Graphics g)
        {
            foreach (var i in points)
            {
                SolidBrush brush = new SolidBrush(pointColor);
                g.FillEllipse(brush,
                    i.X - pointRadius / 2,
                    i.Y - pointRadius / 2,
                    pointRadius,
                    pointRadius);
            }
        }
        private void DrawingTriangle(List<Point> points, Graphics g, Color color)
        {
            var pen = new Pen(color, 3);
            for (int i = 0; i < points.Count - 1; i++)
            {
                g.DrawLine(pen, points[i], points[i + 1]);
            }
            g.DrawLine(pen, points[points.Count - 1], points[0]);

        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            var points = FileRead(filePath);
            if (points != null)
            {
                try
                {
                    drawingPanel.Controls.Clear();
                    drawingPanel.Paint += new PaintEventHandler((object sender, PaintEventArgs e) =>
                    {
                    Graphics g = e.Graphics;
                    DrawingPoints(points, g);
                    var iscl = FindTriangle(points).isosceles;
                    var obts = FindTriangle(points).obtuse;
                    if (iscl != null)
                    {
                        DrawingTriangle(iscl, g, isoscelesColor);
                    }
                    if (obts != null)
                    {
                        DrawingTriangle(obts, g, obtusesColor);
                    }
                    DoTrianglesIntersect(iscl, obts);
                        WriteToFile(iscl, obts, resultPath);
                    });
                    drawingPanel.Refresh();
                   
                }
                catch { }
                
            }
        }
        private (List<Point> isosceles, List<Point> obtuse) FindTriangle(List<Point> points)
        {
            var isosceles = new List<Point>();
            var obtuse = new List<Point>();
            double Distance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            double P(double d1, double d2, double d3) => (d1 + d2 + d3) / 2;//полупериметр
            double Square(double d1, double d2, double d3, double p) => Math.Sqrt(p * (p - d1) * (p - d2) * (p - d3));
            bool IfBig(List<Point> pnt, double square)
            {
                if (pnt.Count == 0)
                {
                    return true;
                }
                else
                {
                    double id1 = Distance(pnt[0], pnt[1]);
                    double id2 = Distance(pnt[1], pnt[2]);
                    double id3 = Distance(pnt[2], pnt[0]);
                    if (square > Square(id1, id2, id3, P(id1, id2, id3)))
                    {
                        return true;
                    }
                }
                return false;   
            }
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i; j < points.Count; j++)
                {
                    for (int k = j; k < points.Count; k++)
                    {
                        //для рівнобедрених
                        double d1 = Distance(points[i], points[j]);
                        double d2 = Distance(points[j], points[k]);
                        double d3 = Distance(points[k], points[i]);

                        //для тупокутних
                        double a = Math.Pow(d1, 2);
                        double b = Math.Pow(d2, 2);
                        double c = Math.Pow(d3, 2);
                        var square = Square(d1, d2, d3, P(d1, d2, d3));//площа текущого
                        if (d1 == d2 || d2 == d3 || d1 == d3)
                        {//якщо площа більша записуєм новий
                            if(IfBig(isosceles, square))
                            {
                                isosceles = new List<Point> { points[i], points[j], points[k] };
                            }
                        }
                        else if((a>b+c && d1 < d2 + d3)
                            || (b > a + c && d2 < d1 + d3)
                            || (c > b + a && d3 < d2 + d1)) //a^2+b^2<c^2
                        {
                            if (IfBig(obtuse, square))
                            {
                                obtuse = new List<Point> { points[i], points[j], points[k] };
                            }
                        }
                    }
                }
            }
            return (isosceles, obtuse);
        }
        private void DoTrianglesIntersect(List<Point> t1, List<Point> t2)
        {
            // Перевірка перетинів сторін
             if(DoIntersect(t1[0], t1[1], t2[0], t2[1])
                || DoIntersect(t1[0], t1[1], t2[0], t2[2])
                || DoIntersect(t1[0], t1[1], t2[2], t2[1])
                || DoIntersect(t1[0], t1[2], t2[0], t2[1])
                || DoIntersect(t1[0], t1[2], t2[0], t2[2])
                || DoIntersect(t1[0], t1[2], t2[2], t2[1])
                || DoIntersect(t1[1], t1[2], t2[0], t2[1])
                || DoIntersect(t1[1], t1[2], t2[0], t2[2])
                || DoIntersect(t1[1], t1[2], t2[2], t2[1]))
            {
                MessageBox.Show("Трикутники перетинаються");
            }
        }
        private bool DoIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            if (o1 != o2 && o3 != o4)
                return true;

            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false;
        }
        private int Orientation(Point p, Point q, Point r)
        {
            int val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
            if (val == 0) return 0;
            return (val > 0) ? 1 : 2;
        }
        private bool OnSegment(Point p, Point q, Point r)
        {
            return q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                   q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
        }
    }
}
