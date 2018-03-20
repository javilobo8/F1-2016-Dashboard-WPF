using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace F1Dashboard
{
    class Speedometer
    {
        public static Point center = new Point(100, 100);
        public static int start_range = 90;
        public static int end_range = 360 - 90;
        
        public static float MAX_SPEED = 400f;
        public static float MAX_RPM = 14000f;

        public static float circ_distance = 270f;

        public static float rpm_step_unit = 1000;
        public static float rpm_steps = MAX_RPM / rpm_step_unit;
        public static float rpm_step_deg_size = circ_distance / rpm_steps;

        public static float speed_step_unit = 10;
        public static float speed_steps = MAX_SPEED / speed_step_unit;
        public static float speed_step_deg_size = circ_distance / speed_steps;

        private Canvas canvas;
        private Line line_speed = new Line() { X1 = 100, Y1 = 10, X2 = 100, Y2 = 100, Stroke = ColorSet.SM_GREEN, StrokeThickness = 4, };
        private Line line_rpm = new Line() { X1 = 100, Y1 = 10, X2 = 100, Y2 = 100, Stroke = ColorSet.SM_ORANGE, StrokeThickness = 2, };

        private float d_speed = 0f;
        private float d_rpm = 0f;

        public Speedometer(Canvas _canvas)
        {
            this.canvas = _canvas;
        }

        public void Init()
        {
            Canvas.SetZIndex(line_speed, 100);
            this.canvas.Children.Add(line_speed);
            Canvas.SetZIndex(line_rpm, 100);
            this.canvas.Children.Add(line_rpm);

            this.UpdateSpeedLine(-90);
            this.UpdateRpmLine(-90);
            this.DrawSpeedSteps();
            this.DrawRpmSteps();
        }

        public void Update(F1Data data)
        {
            d_speed = Util.MPHtoKMH(data.Get("speed"));
            d_rpm = data.Get("engineRate");

            float speed_degrees = Util.CalcDegrees(d_speed, start_range, end_range, MAX_SPEED);
            float rpm_degrees = Util.CalcDegrees(d_rpm, start_range, end_range, MAX_RPM);

            this.UpdateSpeedLine(speed_degrees);
            this.UpdateRpmLine(rpm_degrees);
        }

        private void DrawLine(Point p1, Point p2, Brush brush, int thickness)
        {
            Line line = new Line();
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;
            line.Stroke = brush;
            line.StrokeThickness = thickness;
            this.canvas.Children.Add(line);
        }

        private void DrawSpeedSteps()
        {
            Point point1 = Util.Rotate(new Point(15, 100), center, -90);
            Point point2 = Util.Rotate(new Point(20, 100), center, -90);

            for (int i = 0; i <= speed_steps; i++)
            {
                float deg = (i * speed_step_deg_size);
                Point p1 = Util.Rotate(point1, center, deg);
                Point p2 = Util.Rotate(point2, center, deg);
                DrawLine(p1, p2, ColorSet.MD_WHITE, 2);
            }
        }

        private void DrawRpmSteps()
        {
            Point point1 = Util.Rotate(new Point(30, 100), center, -90);
            Point point2 = Util.Rotate(new Point(50, 100), center, -90);

            for (int i = 0; i <= rpm_steps; i++)
            {
                float deg = (i * rpm_step_deg_size);
                Point p1 = Util.Rotate(point1, center, deg);
                Point p2 = Util.Rotate(point2, center, deg);
                DrawLine(p1, p2, ColorSet.MD_WHITE, 2);
            }
        }

        private void UpdateSpeedLine(double degrees)
        {
            Point point1 = Util.Rotate(new Point(11, 100), center, degrees);
            Point point2 = Util.Rotate(new Point(20, 100), center, degrees);

            this.line_speed.X1 = point1.X;
            this.line_speed.Y1 = point1.Y;
            this.line_speed.X2 = point2.X;
            this.line_speed.Y2 = point2.Y;
        }

        private void UpdateRpmLine(double degrees)
        {
            Point point1 = Util.Rotate(new Point(30, 100), center, degrees);
            Point point2 = Util.Rotate(new Point(85, 100), center, degrees);

            this.line_rpm.X1 = point1.X;
            this.line_rpm.Y1 = point1.Y;
            this.line_rpm.X2 = point2.X;
            this.line_rpm.Y2 = point2.Y;
        }
    }
}
