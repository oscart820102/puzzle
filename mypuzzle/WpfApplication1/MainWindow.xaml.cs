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
    //i
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            creatpuzzle(5,6);
    
            
        }

        private void randomfill(Rectangle rec) {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int x = rnd.Next(6);
            ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("picture/gold.png", UriKind.Relative)));
            switch (x) { 
                case 1:
                    brush = new ImageBrush(new BitmapImage(new Uri("picture/gold.png", UriKind.Relative)));
                    gold++;
                    break;
                case 2:
                    brush = new ImageBrush(new BitmapImage(new Uri("picture/dark.png", UriKind.Relative)));
                    dark++;
                    break;
                case 3:
                    brush = new ImageBrush(new BitmapImage(new Uri("picture/fire.png", UriKind.Relative)));
                    fire++;
                    break;
                case 4:
                    brush = new ImageBrush(new BitmapImage(new Uri("picture/water.png", UriKind.Relative)));
                    water++;
                    break;
                case 5:
                     brush = new ImageBrush(new BitmapImage(new Uri("picture/wood.png", UriKind.Relative)));
                     wood++;
                     break;
                case 0:
                    brush = new ImageBrush(new BitmapImage(new Uri("picture/heart.png", UriKind.Relative)));
                    heart++;
                    break;
                default:
                    MessageBox.Show("no puzzle!!!!!");
                    break;
            }
         //   ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("picture/gold.png", UriKind.Relative)));
            rec.Fill = brush;
           
            return ;
        }
        
        private void creatpuzzle(int x, int y) 
        {
            wood = 0; water = 0; fire = 0; gold = 0; dark = 0; heart = 0;
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    Rectangle rec = new Rectangle()
                    {
                        Height = 50,
                        Width = 50,
                        Stroke= new SolidColorBrush(Colors.Black),
                      
                    };
                    randomfill(rec);
               
                    rec.MouseDown +=  new MouseButtonEventHandler(this.Rectangle_MouseDown); 
                    rec.MouseMove +=  new MouseEventHandler(this.Rectangle_MouseMove);
                    rec.MouseUp += new MouseButtonEventHandler(this.Rectangle_MouseUp);
                    Canvas.SetTop(rec, 40 +i*55);
                    Canvas.SetLeft(rec, 40+ j*55);
                    
                    can1.Children.Add(rec);
                }
            }
            lbdark.Content = "暗珠: " + dark + "顆";
            lbwater.Content = "水珠: " + water + "顆";
            lbfire.Content = "火珠: " + fire + "顆";
            lbwood.Content = "木珠: " + wood + "顆";
            lbgold.Content = "光珠: " + gold + "顆";
            lbheart.Content = "心珠: " + heart + "顆";
            lbcombo.Content = "最大combo數: " + ((dark / 3) + (water / 3) + (fire / 3) + (wood / 3) + (gold / 3) + (heart / 3));
        }

        int wood = 0, water = 0, fire = 0, gold = 0,dark=0,heart=0;

        double mouseX;
        double mouseY;
        double oldpositionX; //add by me
        double oldpositionY;    // add by me
        double currentshapX;
        double currentshapY;

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Rectangle item = sender as Rectangle;
            mouseX = e.GetPosition(null).X;
            mouseY = e.GetPosition(null).Y;
            item.CaptureMouse();
            oldpositionX = (double)item.GetValue(Canvas.LeftProperty);
            oldpositionY = (double)item.GetValue(Canvas.TopProperty);
            lb1.Content = oldpositionX;
            lb2.Content = oldpositionY;
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle item = sender as Rectangle;
            item.ReleaseMouseCapture();
            mouseX = -1;
            mouseY = -1;
            item.SetValue(Canvas.LeftProperty, oldpositionX); //轉完自動歸位圓圈
            item.SetValue(Canvas.TopProperty, oldpositionY);  //
        }


        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle item = sender as Rectangle;
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
            //    Geometry g = item.RenderedGeometry;
                //座標位置轉換為視窗的座標
          //      g.Transform = item.TransformToAncestor(this) as MatrixTransform;
          //      VisualTreeHelper.HitTest(this, null,
           //         new HitTestResultCallback(myHitTestResult),
              //      new GeometryHitTestParameters(g));
            }
        }
        
        
        
        
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
              

                if (currentshapX > oldpositionX + 40 || currentshapX < oldpositionX - 40 || currentshapY > oldpositionY + 40 || currentshapY < oldpositionY - 40)
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

        private void btrand_Click(object sender, RoutedEventArgs e)
        {
            can1.Children.RemoveRange(11,34); //從第5個產生的物件開始移除(移除拼圖)
            creatpuzzle(5, 6);
        }

    }
}
