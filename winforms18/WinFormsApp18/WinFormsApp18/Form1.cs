using System.Drawing.Configuration;
using System.Text.RegularExpressions;
using System.Drawing;
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
            string content = string.Empty;
            string pattern = @"\d+ \d+";
            try
            {
                content = File.ReadAllText(filepath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(content);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    var res = match.Value.Split(" ");
                    result.Add(new Point(int.Parse(res[0]), int.Parse(res[1])));
                }
            }
            return result;
        }
        private void WriteToFile(List<Triangle> Final,string filepath) 
        {
            var res = "точки/n";
            foreach (var i in Final)
            {
                res += $": ({i.A.X} {i.A.Y})";
                res += $": ({i.B.X} {i.B.Y})";
                res += $": ({i.B.X} {i.C.Y})";
            }
            try
            {
                File.WriteAllText(filepath, res);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        private void DrawingTriangle(List<Triangle> triangles, Graphics g, Color color)
        {
            var pen = new Pen(color, 3);
            for (int i = 0; i < triangles.Count; i++)
            {
                g.DrawLine(pen, triangles[i].A, triangles[i].B);
                g.DrawLine(pen, triangles[i].B, triangles[i].C);
                g.DrawLine(pen, triangles[i].C, triangles[i].A);
            }
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
                        iscl = iscl.OrderBy(s => s.Square()).ToList();
                        obts = obts.OrderBy(s => s.Square()).ToList();
                        if (iscl != null)
                        {
                            DrawingTriangle(iscl, g, isoscelesColor);
                        }

                        if (obts != null)
                        {
                            DrawingTriangle(obts, g, obtusesColor);
                        }

                        var all = iscl.Union(obts).Union(iscl).OrderBy(s => s.Square()).ToList();
                        List<Triangle> t = new List<Triangle>();
                        var inter = false;
                        for (int i = 0; i < all.Count; i++)
                        {
                            for (int j = i + 1; j < all.Count; j++)
                            {
                                if (DoTrianglesIntersect(all[i], all[j]))
                                {
                                    t.Add(all[j]);
                                    t.Add(all[j]);
                                    inter = true;
                                }
                            }
                        }
                            inter ? MessageBox.Show("Трикутники перетинаються")
                            : MessageBox.Show("Трикутники не перетинаються");
                        all.Distinct();
                        WriteToFile(all, resultPath);
                    });
                    drawingPanel.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                
            }
        }
        private (List<Triangle> isosceles, List<Triangle> obtuse) FindTriangle(List<Point> points)
        {  
            double Distance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            List<Triangle> isosceles = new List<Triangle>();
            List<Triangle> obtuse = new List<Triangle>();
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i; j < points.Count; j++)
                {
                    for (int k = j; k < points.Count; k++)
                    {
                        double d1 = Distance(points[i], points[j]);
                        double d2 = Distance(points[j], points[k]);
                        double d3 = Distance(points[k], points[i]);
                        double a = Math.Pow(d1, 2);
                        double b = Math.Pow(d2, 2);
                        double c = Math.Pow(d3, 2);
                        if (d1 == d2|| d2 == d3|| d1 == d3)
                        {
                            isosceles.Add(new Triangle(points[i], points[j], points[k]));
                        }
                        else if ((a > b + c && d1 < d2 + d3)
                                 || (b > a + c && d2 < d1 + d3)
                                 || (c > b + a && d3 < d2 + d1))
                        {
                            obtuse.Add(new Triangle(points[i], points[j], points[k]));
                        }
                    }
                }
            }
            return (isosceles, obtuse);
        }
        private bool DoTrianglesIntersect(Triangle t1, Triangle t2)
        {
            var a = DoIntersect(t1.A, t1.B, t2.A, t2.B);
            var b = DoIntersect(t1.A, t1.B, t2.A, t2.C);
            var c = DoIntersect(t1.A, t1.B, t2.C, t2.B);
            var d = DoIntersect(t1.A, t1.C, t2.A, t2.B);
            var e = DoIntersect(t1.A, t1.C, t2.A, t2.C);
            var f = DoIntersect(t1.A, t1.C, t2.C, t2.B);
            var g = DoIntersect(t1.B, t1.C, t2.A, t2.B);
            var h = DoIntersect(t1.B, t1.C, t2.A, t2.C);
            var j = DoIntersect(t1.B, t1.C, t2.C, t2.B);
            if(a || b || c || d || e || f || g || h || j)
            {
                return true;
            }
            return false;
                

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
            double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
            if ((int)val == 0) return 0;
            return ((int)val > 0) ? 1 : 2;
        }
        private bool OnSegment(Point p, Point q, Point r)
        {
            return q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                   q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
        }
    }
}
