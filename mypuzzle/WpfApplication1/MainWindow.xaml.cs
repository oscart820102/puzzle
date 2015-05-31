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
//add for mouse
using System.Windows.Threading;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;


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
            // timer顯示座標
            DispatcherTimer timer;
            timer = new DispatcherTimer();
            timer.Tick += timer_1Tick;
            timer.Start();
            //
            InitializeComponent();
            creatpuzzle(5,6);               
        }

        private void timer_1Tick(object sender, EventArgs e)
        {

            this.plabel.Content = "(" + System.Windows.Forms.Cursor.Position.X.ToString() + "," + System.Windows.Forms.Cursor.Position.Y.ToString() + ")";
        }

       
        private void randomfill(System.Windows.Shapes.Rectangle rec) {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int x = rnd.Next(6);
        
            ImageBrush brush = null ;
            //問助教為什麼要用image 不能用new的   ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("images/gold.png", UriKind.Relative)));

      
            switch (x) { 
                case 1:
                    image1.Source = new BitmapImage(new Uri("images/gold.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    gold++;
                    break;
                case 2:
                    image1.Source = new BitmapImage(new Uri("images/dark.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    dark++;
                    break;
                case 3:
                    image1.Source = new BitmapImage(new Uri("images/fire.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    fire++;
                    break;
                case 4:
                    image1.Source = new BitmapImage(new Uri("images/water.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    water++;
                    break;
                case 5:
                    image1.Source = new BitmapImage(new Uri("images/wood.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                     wood++;
                     break;
                case 0:
                    image1.Source = new BitmapImage(new Uri("images/heart.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
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
                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle()
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

        //mouse test
        double mOldX;
        double mOldY;
        double mNewX;
        double mNewY;
        //

        double mouseX;
        double mouseY;
        double oldpositionX;    //add by me
        double oldpositionY;    // add by me
        double currentshapX;
        double currentshapY;
        System.Windows.Shapes.Rectangle CurrentRec = null;

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
          

            System.Windows.Shapes.Rectangle item = sender as System.Windows.Shapes.Rectangle;
            CurrentRec = item;
           item.Fill = new SolidColorBrush(Colors.Red);  //delete later, just for mouse test
            //mouse test
            mOldX = System.Windows.Forms.Cursor.Position.X;
            mOldY = System.Windows.Forms.Cursor.Position.Y;
            //

            Canvas.SetZIndex(item, 100);  //移動的物體總是在最上方
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
            System.Windows.Shapes.Rectangle item = sender as System.Windows.Shapes.Rectangle;
            item.ReleaseMouseCapture();
            Canvas.SetZIndex(item, 0);  //物體位置回歸水平
            mouseX = -1;
            mouseY = -1;
            item.SetValue(Canvas.LeftProperty, oldpositionX); //轉完自動歸位圓圈
            item.SetValue(Canvas.TopProperty, oldpositionY);  //

            //mouse test
        
            mNewX = System.Windows.Forms.Cursor.Position.X;
            mNewY = System.Windows.Forms.Cursor.Position.Y;
            //

        }


        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Shapes.Rectangle item = sender as System.Windows.Shapes.Rectangle;
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
              //  將正方形形圖轉成幾何圖形
                Geometry g = item.RenderedGeometry;
            //    座標位置轉換為視窗的座標
                g.Transform = item.TransformToAncestor(this) as MatrixTransform;
                VisualTreeHelper.HitTest(this, null,
                    new HitTestResultCallback(myHitTestResult),
                    new GeometryHitTestParameters(g));
            }
        }
           
  
        public HitTestResultBehavior myHitTestResult(HitTestResult result)
        {
            if (result.VisualHit is System.Windows.Shapes.Rectangle && result.VisualHit!= CurrentRec)
            {
                System.Windows.Shapes.Rectangle rect = result.VisualHit as System.Windows.Shapes.Rectangle;
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
            can1.Children.RemoveRange(15,33); //從第10個產生的物件開始移除(移除拼圖)
            creatpuzzle(5, 6);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Mouse.MoveTo(tbx.Text, tby.Text);
            Mouse.LeftClick();

       //     Mouse.DragTo(mOldX.ToString(), mOldY.ToString(), mNewX.ToString(), mNewY.ToString());
        }

        /*       
    private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
    {
           
        Ellipse item = sender as Ellipse;
        Canvas.SetZIndex(item, 100);  //移動的物體總是在最上方
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
        Canvas.SetZIndex(item, 0);  //物體位置回歸水平
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

    */


    }

    //add for mouse
    internal class NativeContansts
    {
        public const int WH_MOUSE_LL = 14;
        public const int WH_KEYBOARD_LL = 13;
        public const int WH_MOUSE = 7;
        public const int WH_KEYBOARD = 2;

        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_RBUTTONDOWN = 0x204;
        public const int WM_MBUTTONDOWN = 0x207;
        public const int WM_LBUTTONUP = 0x202;
        public const int WM_RBUTTONUP = 0x205;
        public const int WM_MBUTTONUP = 0x208;
        public const int WM_LBUTTONDBLCLK = 0x203;
        public const int WM_RBUTTONDBLCLK = 0x206;
        public const int WM_MBUTTONDBLCLK = 0x209;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public const int WM_SYSKEYDOWN = 0x104;
        public const int WM_SYSKEYUP = 0x105;

        public const int MEF_LEFTDOWN = 0x00000002;
        public const int MEF_LEFTUP = 0x00000004;
        public const int MEF_MIDDLEDOWN = 0x00000020;
        public const int MEF_MIDDLEUP = 0x00000040;
        public const int MEF_RIGHTDOWN = 0x00000008;
        public const int MEF_RIGHTUP = 0x00000010;

        public const int KEF_EXTENDEDKEY = 0x1;
        public const int KEF_KEYUP = 0x2;

        public const byte VK_SHIFT = 0x10;
        public const byte VK_CAPITAL = 0x14;
        public const byte VK_NUMLOCK = 0x90;

        public const int WM_IME_SETCONTEXT = 0x0281;
        public const int WM_CHAR = 0x0102;
        public const int WM_IME_COMPOSITION = 0x010F;
        public const int GCS_COMPSTR = 0x0008;
    }

    //public partial class Form1 : Form
    //{
    //    public Form1()
    //    {
    //        InitializeComponent();
    //    }

    //    private void button1_Click(object sender, EventArgs e)
    //    {
    //        Thread.Sleep(500);
    //        Mouse.MoveTo(tX1.Text, tY1.Text);
    //    }

    //    private void button4_Click(object sender, EventArgs e)
    //    {
    //        Thread.Sleep(500);
    //        Mouse.MoveTo(tX1.Text, tY1.Text);
    //        Mouse.LeftClick();
    //    }

    //    private void button2_Click(object sender, EventArgs e)
    //    {
    //        Thread.Sleep(500);
    //        Mouse.MoveTo(tX1.Text, tY1.Text);
    //        // Mouse.LeftDoubleClick();
    //    }

    //    private void button3_Click(object sender, EventArgs e)
    //    {
    //        Thread.Sleep(500);
    //        Mouse.DragTo(tX1.Text, tY1.Text, tX2.Text, tY2.Text);
    //    }

    //    private void timer1_Tick(object sender, EventArgs e)
    //    {
    //        this.Text = "(" + Cursor.Position.X.ToString() + "," + Cursor.Position.Y.ToString() + ")";
    //    }
    //}

    public static partial class Mouse
    {

        [DllImport("User32")]


        public extern static void mouse_event(int dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);



        static public void DragTo(string sor_X, string sor_Y, string des_X, string des_Y)
        {
            MoveTo(sor_X, sor_Y);
          //  Thread.Sleep(1200);
            LeftDown();
            Thread.Sleep(200);
            MoveTo(des_X, des_Y);
     //       Thread.Sleep(200);
            LeftUp();
        }
        /// <summary>
        /// 模擬壓住滑鼠左鍵。
        /// </summary>
        /// 
        static public void MoveTo(string tx, string ty)
        {
            int x, y;
            int.TryParse(tx, out x);
            int.TryParse(ty, out y);


            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);
            //    Cursor.Position = new Point(x, y);
        }

        public static void LeftDown()
        {
            mouse_event(NativeContansts.MEF_LEFTDOWN, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬釋放滑鼠左鍵。
        /// </summary>
        public static void LeftUp()
        {
            mouse_event(NativeContansts.MEF_LEFTUP, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬點擊滑鼠左鍵。
        /// </summary>
        public static void LeftClick()
        {
            LeftDown();
            LeftUp();
        }

        /// <summary>
        /// 模擬壓住滑鼠右鍵。
        /// </summary>
        public static void RightDown()
        {
            mouse_event(NativeContansts.MEF_RIGHTDOWN, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬釋放滑鼠右鍵。
        /// </summary>
        public static void RightUp()
        {
            mouse_event(NativeContansts.MEF_RIGHTUP, 0, 0, 0, IntPtr.Zero);
        }
        /// <summary>
        /// 模擬點擊滑鼠右鍵。
        /// </summary>
        public static void RightClick()
        {
            RightDown();
            RightUp();
        }


    }



}
