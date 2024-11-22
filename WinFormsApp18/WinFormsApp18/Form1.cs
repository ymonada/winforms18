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
        private string filePath = "file.txt";
        private string resultPath = "result.txt";
        List<Point> points = [];
        List<Triangle> triangles = [];
        List<Triangle> result = [];
        public Form1()
        {
            InitializeComponent();
            Paint += Drawing;
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
            var res = "точки\n";
            foreach (var i in Final)
            {
                res += $"Point: ({i.A.X} {i.A.Y})\n";
                res += $"Point: ({i.B.X} {i.B.Y})\n";
                res += $"Point: ({i.B.X} {i.C.Y})\n";
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
        private void Drawing(object sender, PaintEventArgs e) 
        {
            var g = e.Graphics;
            SolidBrush brush = new SolidBrush(pointColor);            
            if (points.Count > 0)
            {
                foreach (var i in points)
                {
                   
                    g.FillEllipse(brush,
                        (int)i.X - pointRadius / 2,
                        (int)i.Y - pointRadius / 2,
                        pointRadius,
                        pointRadius);
                }
            }
            if(triangles.Count > 0)
            {
                foreach(var i in result)
                {
                    var pen = new Pen(i.Color, 3);
                    g.DrawLine(pen, (int)i.A.X, (int)i.A.Y, (int)i.B.X, (int)i.B.Y);
                    g.DrawLine(pen, (int)i.B.X, (int)i.B.Y, (int)i.C.X, (int)i.C.Y);
                    g.DrawLine(pen, (int)i.C.X, (int)i.C.Y, (int)i.A.X, (int)i.A.Y);
                }
            }
            
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                points = FileRead(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            if (points.Count>0)
            {          
                FindIsosceles();
                FindObtuse();
                var iscl = triangles.Where(s =>s.Color == isoscelesColor).ToList();
                var obts = triangles.Where(s => s.Color == obtusesColor).ToList();
                iscl = iscl.OrderBy(s => s.Square()).ToList();
                obts = obts.OrderBy(s => s.Square()).ToList();                  
                if (iscl.Count > 0)
                {
                    if (obts.Count > 0)
                    {
                        result.Add(iscl[0]);
                        result.Add(iscl[^1]);
                        result.Add(obts[0]);
                        result.Add(obts[^1]);
                    }
                }
                bool inter = false;
                for (int i = 0; i < result.Count; i++)
                {
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        if (DoTrianglesIntersect(result[i], result[j]))
                        {                                   
                            inter = true;
                            break;
                        }
                    }
                }                               
                WriteToFile(result, resultPath);                                
                Refresh();
                if (inter)
                    MessageBox.Show("Трикутники перетинаються");
                else MessageBox.Show("Трикутники не перетинаються");
            }
        }
        private double Distance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        private void FindObtuse()
        {
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    for (int k = j + 1; k < points.Count; k++)
                    {
                        double d1 = Distance(points[i], points[j]);
                        double d2 = Distance(points[j], points[k]);
                        double d3 = Distance(points[k], points[i]);
                        double a = Math.Pow(d1, 2);
                        double b = Math.Pow(d2, 2);
                        double c = Math.Pow(d3, 2);
                        double cosA = (b + c - a) / (2 * Math.Sqrt(b) * Math.Sqrt(c));
                        double cosB = (a + c - b) / (2 * Math.Sqrt(a) * Math.Sqrt(c));
                        double cosC = (a + b - c) / (2 * Math.Sqrt(a) * Math.Sqrt(b));
                        bool isObtuseA = cosA < 0 && cosA >= -0.9397; // Кут при точці p1
                        bool isObtuseB = cosB < 0 && cosB >= -0.9397; // Кут при точці p2
                        bool isObtuseC = cosC < 0 && cosC >= -0.9397; // Кут при точці p3
                        if (isObtuseA || isObtuseB || isObtuseC)
                        {
                            triangles.Add(new Triangle(points[i], points[j], points[k], obtusesColor));
                        }
                    }
                }
            }
        }
        private void FindIsosceles()
        {  
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i+1; j < points.Count; j++)
                {
                    for (int k = j+1; k < points.Count; k++)
                    {
                        double d1 = Distance(points[i], points[j]);
                        double d2 = Distance(points[j], points[k]);
                        double d3 = Distance(points[k], points[i]);                      
                        if ((d1 == d2 && d2+d1 != d3)
                            || (d3 == d2 && d2 + d3 != d1)
                            || (d1 == d3 && d3 + d1 != d2))
                        {
                            triangles.Add(new Triangle(points[i], points[j], points[k], isoscelesColor));
                        }                       
                    }
                }
            }
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

            if (o1 != o2 && o3 != o4) return true;
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
        private bool OnSegment(Point p, Point q, Point r) => q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                   q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
        
    }
}
