namespace WinFormsApp18;

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }
    public Point(double x, double y)
    {
        X= x; Y=y;
    }

    public Point(int x, int y)
    {
        X = x; 
        Y = y;
    }
    public Point()
    {
            
    }
    //public System.Drawing.Point ToPoint() => new System.Drawing.Point((int)X, (int)Y);


}