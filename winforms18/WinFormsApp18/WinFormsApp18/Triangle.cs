using System.Drawing;
namespace WinFormsApp18;

public class Triangle
{
    public Point A {get;set;}
    public Point B { get; set; }
    public Point C { get; set; }
    public Color Color { get; set; } = Color.Black;
    public Triangle(Point A, Point B, Point C) 
    {
        this.A = A;
        this.B = B;
        this.C = C;
    }
    

    public double Square()
    { 
        double Distance(Point p1, Point p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        double P(double d1, double d2, double d3) => (d1 + d2 + d3) / 2;
        var d1 = Distance(this.A, this.B);
        var d2 = Distance(this.B, this.C);
        var d3 = Distance(this.A, this.C);
        var p = P(d1, d2, d3);
        return Math.Sqrt(p * (p - d1) * (p - d2) * (p - d3));
    }
}