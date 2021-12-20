using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;



namespace RTSHelper {



    public class CircularProgress : Shape { // Tomado de https://wpf.2000things.com/2014/09/10/1155-a-circular-progress-indicator/.



        static CircularProgress() {

            Brush brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            brush.Freeze();
            StrokeProperty.OverrideMetadata(typeof(CircularProgress), new FrameworkPropertyMetadata(brush));
            FillProperty.OverrideMetadata(typeof(CircularProgress), new FrameworkPropertyMetadata(Brushes.Transparent));
            StrokeThicknessProperty.OverrideMetadata(typeof(CircularProgress), new FrameworkPropertyMetadata(10.0));

        } // CircularProgress>


        public double Value {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        } // Value>


        private static FrameworkPropertyMetadata valueMetadata = new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
                    null, new CoerceValueCallback(CoerceValue));


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CircularProgress), 
            valueMetadata);


        private static object CoerceValue(DependencyObject depObj, object baseVal) {

            double val = (double)baseVal;
            val = Math.Min(val, 99.999);
            val = Math.Max(val, 0.0);
            return val;

        } // CoerceValue>


        protected override Geometry DefiningGeometry {

            get {

                var startAngle = 90.0;
                var endAngle = 90.0 - ((Value / 100.0) * 360.0);
                var maxWidth = Math.Max(0.0, RenderSize.Width - StrokeThickness);
                var maxHeight = Math.Max(0.0, RenderSize.Height - StrokeThickness);
                var xStart = maxWidth / 2.0 * Math.Cos(startAngle * Math.PI / 180.0);
                var yStart = maxHeight / 2.0 * Math.Sin(startAngle * Math.PI / 180.0);
                var xEnd = maxWidth / 2.0 * Math.Cos(endAngle * Math.PI / 180.0);
                var yEnd = maxHeight / 2.0 * Math.Sin(endAngle * Math.PI / 180.0);

                var geom = new StreamGeometry();
                using (var ctx = geom.Open()) {
                    ctx.BeginFigure(new Point((RenderSize.Width / 2.0) + xStart, (RenderSize.Height / 2.0) - yStart), isFilled: true, isClosed: false);  // Closed
                    ctx.ArcTo(new Point((RenderSize.Width / 2.0) + xEnd, (RenderSize.Height / 2.0) - yEnd), new Size(maxWidth / 2.0, maxHeight / 2),
                        0.0, (startAngle - endAngle) > 180, SweepDirection.Clockwise, isStroked: true, isSmoothJoin: false);
                }

                return geom;

            }

        } // DefiningGeometry>



    } // CircularProgress>



} // RTSHelper>
