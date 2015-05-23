using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        double mouseX;
        double mouseY;
        double oldpositionX; //add by me
        double oldpositionY;    // add by me
        double currentshapX;
        double currentshapY;

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse item = sender as Ellipse;
            mouseX = e.GetPosition(null).X;
            mouseY = e.GetPosition(null).Y;
            item.CaptureMouse();
            oldpositionX = (double)item.GetValue(Canvas.LeftProperty);
            oldpositionY = (double)item.GetValue(Canvas.TopProperty);
            lb1.Content = oldpositionX;
            lb2.Content = oldpositionY;
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse item = sender as Ellipse;
            item.ReleaseMouseCapture();
            mouseX = -1;
            mouseY = -1;
            item.SetValue(Canvas.LeftProperty, oldpositionX); //轉完自動歸位圓圈
            item.SetValue(Canvas.TopProperty, oldpositionY);  //
        }


        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            Ellipse item = sender as Ellipse;
            if (item.IsMouseCaptured)
            {

                // Calculate the current position of the object.
                double deltaX = e.GetPosition(null).X - mouseX;
                double deltaY = e.GetPosition(null).Y - mouseY;
                double newLeft = deltaX + (double)item.GetValue(Canvas.LeftProperty);
                double newTop = deltaY + (double)item.GetValue(Canvas.TopProperty);

                // Set new position of object.
                item.SetValue(Canvas.LeftProperty, newLeft);
                item.SetValue(Canvas.TopProperty, newTop);
                
                //add by me
                currentshapX = (double)item.GetValue(Canvas.LeftProperty);

                currentshapY = (double)item.GetValue(Canvas.TopProperty);


                // Update position global variables.
                mouseX = e.GetPosition(null).X;
                mouseY = e.GetPosition(null).Y;

                //將圓形圖轉成幾何圖形
                Geometry g = item.RenderedGeometry;
                //座標位置轉換為視窗的座標
                g.Transform = item.TransformToAncestor(this) as MatrixTransform;
                VisualTreeHelper.HitTest(this, null,
                    new HitTestResultCallback(myHitTestResult),
                    new GeometryHitTestParameters(g));
            }
        }





        public HitTestResultBehavior myHitTestResult(HitTestResult result)
        {
            if (result.VisualHit is Rectangle)
            {
                Rectangle rect = result.VisualHit as Rectangle;
            //    rect.Fill = new SolidColorBrush(Colors.Red);
              

                if (currentshapX > oldpositionX + 30 || currentshapX < oldpositionX - 30 || currentshapY > oldpositionY + 30 || currentshapY < oldpositionY - 30)
                {
                    double tempX, tempY;
                    tempX = (double)rect.GetValue(Canvas.LeftProperty);
                    

                    tempY = (double)rect.GetValue(Canvas.TopProperty);
                    rect.SetValue(Canvas.LeftProperty, oldpositionX);
                    rect.SetValue(Canvas.TopProperty, oldpositionY);
                    oldpositionX = tempX;
                    oldpositionY = tempY;
lb1.Content = oldpositionX;
lb2.Content = oldpositionY;

                }            
            }
            return HitTestResultBehavior.Continue;
        }

    }
}
