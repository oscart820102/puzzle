using System;//
using System.Collections.Generic;
using System.Collections;
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
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
           // timer顯示座標
            DispatcherTimer timersec,timershow;
            MouseButtonEventArgs curmouse = null;
            bool changemode = false;             //手動變換珠子顏色 判斷是不是轉珠模式
            bool adv_ChangeMode = false;
            int adv_type=0;
            int wood = 0, water = 0, fire = 0, gold = 0, dark = 0, heart = 0;
            int comwood = 0, comwater = 0, comfire = 0, comgold = 0, comdark = 0, comheart = 0;
            int countwood = 0, countwater = 0, countfire = 0, countgold = 0, countdark = 0, countheart = 0;
            int[,] orb = new int[5,6];
            int[,] zero = new int[5, 6] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };
            int[,] record_orb = new int[5, 6];
        public MainWindow()
        {
              // timer顯示座標
            DispatcherTimer Postiontimer;
            Postiontimer = new DispatcherTimer();
            Postiontimer.Tick += Postiontimer_1Tick;
            Postiontimer.Start();
            //          
            InitializeComponent();
            creatpuzzle(5,6);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    record_orb[i, j] = orb[i, j];
                }
            }
        }
        
        private void creatpuzzle(int x, int y) //動態生成珠子
        {          
            wood = 0; water = 0; fire = 0; gold = 0; dark = 0; heart = 0;
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    System.Windows.Shapes.Ellipse rec = new System.Windows.Shapes.Ellipse()
                    {
                        Height = 50,
                        Width = 47,
                        //Stroke = new SolidColorBrush(Colors.Transparent),                      
                    };
                    randomfill(i , j , rec);    //隨機給珠子顏色           
                    rec.MouseDown += new MouseButtonEventHandler(this.Ellipse_MouseDown);
                    rec.MouseMove += new MouseEventHandler(this.Ellipse_MouseMove);
                    rec.MouseUp += new MouseButtonEventHandler(this.Ellipse_MouseUp);
                    Canvas.SetTop(rec, 40 +i*55);
                    Canvas.SetLeft(rec, 40+ j*55);                    
                    can1.Children.Add(rec);
                }
            }
            lbdark.Content =  dark + "顆";
            lbwater.Content =  water + "顆";
            lbfire.Content =  fire + "顆";
            lbwood.Content = wood + "顆";
            lbgold.Content =  gold + "顆";
            lbheart.Content =  heart + "顆";
            lbcombo.Content = "最大combo數: " + ((dark / 3) + (water / 3) + (fire / 3) + (wood / 3) + (gold / 3) + (heart / 3));
        }

        private void randomfill(int a, int b, System.Windows.Shapes.Ellipse rec)//隨機上色
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int x = rnd.Next(6);
            ImageBrush brush = null;
            //問助教為什麼要用image 不能用new的   ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("images/gold.png", UriKind.Relative)));      
            switch (x)
            {
                case 1:
                    image1.Source = new BitmapImage(new Uri("images/gold.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    gold++;
                    orb[a, b] = 1;
                    break;
                case 2:
                    image1.Source = new BitmapImage(new Uri("images/dark.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    dark++;
                    orb[a, b] = 2;
                    break;
                case 3:
                    image1.Source = new BitmapImage(new Uri("images/fire.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    fire++;
                    orb[a, b] = 3;
                    break;
                case 4:
                    image1.Source = new BitmapImage(new Uri("images/water.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    water++;
                    orb[a, b] = 4;
                    break;
                case 5:
                    image1.Source = new BitmapImage(new Uri("images/wood.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    wood++;
                    orb[a, b] = 5;
                    break;
                case 0:
                    image1.Source = new BitmapImage(new Uri("images/heart.png", UriKind.Relative));
                    brush = new ImageBrush(image1.Source);
                    heart++;
                    orb[a, b] = 6;
                    break;
                default:
                    MessageBox.Show("no puzzle!!!!!");
                    break;
            }
            //   ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("picture/gold.png", UriKind.Relative)));
            rec.Fill = brush;
            return;
        }

        double mouseX;
        double mouseY;
        double oldpositionX;    //圖形原本位置
        double oldpositionY;    
        double currentshapX;    //圖形隨著滑鼠移動的位置
        double currentshapY;
        System.Windows.Shapes.Ellipse CurrentRec = null;  //宣告目前移動的正方形

        private int find_combo(int[,] ori)
        {
            //int[,] ori = orb;
            int count = 0;
            bool judge = false;
            int[,] temp = new int[5 , 6];
            // 1. filter all 3+ consecutives.
            //  (a) horizontals
            for (var i = 0; i < 5; ++ i) {
                var prev_1_orb = 19;
                var prev_2_orb = 19;
                for (var j = 0; j < 6; ++ j) {
                    var cur_orb = ori[i , j];
                    if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != 19 && cur_orb != 29) {
                        temp[i , j] = cur_orb;
                        temp[i , j-1] = cur_orb;
                        temp[i , j-2] = cur_orb;
                    }
                    prev_1_orb = prev_2_orb;
                    prev_2_orb = cur_orb;
                }
            }
            //  (b) verticals
            for (var j = 0; j < 6; ++ j) {
                var prev_1_orb = 19;
                var prev_2_orb = 19;
                for (var i = 0; i < 5; ++ i) {
                    var cur_orb = ori[i , j];
                    if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != 19 && cur_orb != 29) {
                        temp[i , j] = cur_orb;
                        temp[i-1 , j] = cur_orb;
                        temp[i-2 , j] = cur_orb;
                    }
                    prev_1_orb = prev_2_orb;
                    prev_2_orb = cur_orb;
                }
            }

            int[,] temp2 = ori;
            /*for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                    temp2[i, j] = orb[i, j];
                {
                    textbox1.Text += temp[i, j].ToString() + " ";
                }
                textbox1.Text += "\n";
            }*/
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (temp2[i, j] == temp[i, j])
                    {
                        judge = true;
                        temp2[i, j] = 29;
                        for (int k = i; k > 0; k--)
                        {
                            int oritemp = temp2[k, j];
                            temp2[k, j] = temp2[k - 1, j];
                            temp2[k - 1, j] = oritemp;
                        }
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    //textbox1.Text += temp[i, j].ToString() + " ";
                }
                //textbox1.Text += "\n";
            }
            //textbox1.Text += "\n";
            count = combocount(temp);
            if (judge == true)
                return find_combo(temp2) + count;
            else
                return 0;
        }

        private int combocount(int[,] ori)
        {
            //int[,] temp = new int[6, 7];
            //bool combojudge = false;
            int count = 0;
            /*for (int i = 0; i < 5; i++)
                for (int j = 0; j < 6; j++)
                    temp[i + 1, j + 1] = ori[i, j];*/
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 6; j++)
                {
                    if (ori[i , j] != 0)
                    {
                        switch (ori[i, j])
                        {
                            case 1:
                                comgold++;
                                break;
                            case 2:
                                comdark++;
                                break;
                            case 3:
                                comfire++;
                                break;
                            case 4:
                                comwater++;
                                break;
                            case 5:
                                comwood++;
                                break;
                            case 6:
                                comheart++;
                                break;
                        }
                        //combojudge = true;
                        ori = setzero(ori , i , j);
                        count++;
                    }
                }
            return count;
        }

        private int[,] setzero(int[,] ori , int i , int j)
        {
            int temp = ori[i, j];
            switch (ori[i, j])
            {
                case 1:
                    countgold++;
                    break;
                case 2:
                    countdark++;
                    break;
                case 3:
                    countfire++;
                    break;
                case 4:
                    countwater++;
                    break;
                case 5:
                    countwood++;
                    break;
                case 6:
                    countheart++;
                    break;
            }
            ori[i, j] = 0;
            if (i > 0)
                if (temp == ori[i - 1, j] )
                    ori =  setzero(ori, i - 1, j);
            if (i < 4)
                if (temp  == ori[i + 1, j])
                    ori =  setzero(ori, i + 1, j);
            if (j > 0)
                if (temp  == ori[i, j - 1])
                    ori =  setzero(ori, i, j - 1);
            if (j < 5)
                if (temp  == ori[i, j+1])
                    ori =  setzero(ori, i, j + 1);
            return ori;
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            clearTB();
            if (changemode)  //換色模式
            {
                double x, y;
                System.Windows.Shapes.Ellipse item = sender as System.Windows.Shapes.Ellipse;
               // item.Fill = new SolidColorBrush(Colors.Red);  //delete later, just for mouse test
                ImageBrush brush = null;
                //image1.Source = new BitmapImage(new Uri("images/dark.png", UriKind.Relative));
                //brush = new ImageBrush(image1.Source);
                //item.Fill = brush;
                y = (double)item.GetValue(Canvas.LeftProperty);
                x = (double)item.GetValue(Canvas.TopProperty);
                x = (x - 40) / 55 ;
                y = (y - 40) / 55 ;
          //      lb1.Content = x;
           //     lb2.Content = y;
                if (adv_ChangeMode)
                {
                    switch (orb[(int)x, (int)y])
                    {
                        case 6:
                            heart--;
                            break;
                        case 1:
                            gold--;
                            break;
                        case 2:
                            dark--;
                            break;
                        case 3:
                            fire--;
                            break;
                        case 4:
                            water--;
                            break;
                        case 5:
                            wood--;
                            break;
                        default:
                            //  MessageBox.Show("no NUBER!!!!!");
                            break;
                    }
                    switch (adv_type) 
                    {
                        case 1:
                            image1.Source = new BitmapImage(new Uri("images/gold.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            gold++;
                            orb[(int)x, (int)y] = 1;
                            break;
                        case 2:
                            image1.Source = new BitmapImage(new Uri("images/dark.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            dark++;
                            orb[(int)x, (int)y] = 2;
                            break;
                        case 3:
                            image1.Source = new BitmapImage(new Uri("images/fire.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;                            
                            fire++;
                            orb[(int)x, (int)y] = 3;
                            break;
                        case 4:
                            image1.Source = new BitmapImage(new Uri("images/water.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            water++;
                            orb[(int)x, (int)y] = 4;
                            break;
                        case 5:
                            image1.Source = new BitmapImage(new Uri("images/wood.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            wood++;
                            orb[(int)x, (int)y] = 5;
                            break;
                        case 6:
                            image1.Source = new BitmapImage(new Uri("images/heart.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            heart++;
                            orb[(int)x, (int)y] = 6;
                            break;
                        default:
                            MessageBox.Show("adv no puzzle!!!!!");
                            break;
                    }
                }
                else
                {
                    if (zero[(int)x, (int)y] == 0)
                    {
                        zero[(int)x, (int)y] = 1;
                        switch (orb[(int)x, (int)y])
                        {
                            case 6:
                                heart--;
                                break;
                            case 1:
                                gold--;
                                break;
                            case 2:
                                dark--;
                                break;
                            case 3:
                                fire--;
                                break;
                            case 4:
                                water--;
                                break;
                            case 5:
                                wood--;
                                break;
                            default:
                                //  MessageBox.Show("no NUBER!!!!!");
                                break;
                        }
                        orb[(int)x, (int)y] = 6;
                        heart++;
                    }
                    switch (orb[(int)x, (int)y])
                    {
                        case 6:
                            image1.Source = new BitmapImage(new Uri("images/gold.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            heart--;
                            gold++;
                            orb[(int)x, (int)y] = 1;
                            break;
                        case 1:
                            image1.Source = new BitmapImage(new Uri("images/dark.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            gold--;
                            dark++;
                            orb[(int)x, (int)y] = 2;
                            break;
                        case 2:
                            image1.Source = new BitmapImage(new Uri("images/fire.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            dark--;
                            fire++;
                            orb[(int)x, (int)y] = 3;
                            break;
                        case 3:
                            image1.Source = new BitmapImage(new Uri("images/water.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            fire--;
                            water++;
                            orb[(int)x, (int)y] = 4;
                            break;
                        case 4:
                            image1.Source = new BitmapImage(new Uri("images/wood.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            water--;
                            wood++;
                            orb[(int)x, (int)y] = 5;
                            break;
                        case 5:
                            image1.Source = new BitmapImage(new Uri("images/heart.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            item.Fill = brush;
                            wood--;
                            heart++;
                            orb[(int)x, (int)y] = 6;
                            break;
                        default:
                            MessageBox.Show("no puzzle!!!!!");
                            break;
                    }
                }
                lbdark.Content = dark + "顆";
                lbwater.Content = water + "顆";
                lbfire.Content = fire + "顆";
                lbwood.Content = wood + "顆";
                lbgold.Content = gold + "顆";
                lbheart.Content = heart + "顆";
                lbcombo.Content = "最大combo數: " + ((dark / 3) + (water / 3) + (fire / 3) + (wood / 3) + (gold / 3) + (heart / 3)); 
            }
            else  //轉珠模式
            {
                int time;//轉珠時間
                if (comboTime.SelectionBoxItem.ToString() == "無限制")
                {
                    time = 100;
                }
                else
                {
                    time = int.Parse(comboTime.SelectionBoxItem.ToString());
                }

                //MessageBox.Show(comboTime.SelectionBoxItem.ToString());
                //timer
                timersec = new DispatcherTimer();
                timersec.Interval = TimeSpan.FromSeconds(time);
                timersec.Tick += timersec_Tick;
                timersec.Start();
                //
                timershow = new DispatcherTimer();
                timershow.Interval = TimeSpan.FromSeconds(1.0);
                timershow.Tick += timershow_Tick;
                this.lbtimer.Content = "0";
                timershow.Start();
                //

                curmouse = e;

                System.Windows.Shapes.Ellipse item = sender as System.Windows.Shapes.Ellipse;
                CurrentRec = item;  //記錄下要移動的正方形(hittest才可以判斷)
                // item.Fill = new SolidColorBrush(Colors.Red);  //delete later, just for mouse test
                Canvas.SetZIndex(item, 100);  //移動的物體總是在最上方
                mouseX = e.GetPosition(null).X;
                mouseY = e.GetPosition(null).Y;
                item.CaptureMouse();
                oldpositionX = (double)item.GetValue(Canvas.LeftProperty);
                oldpositionY = (double)item.GetValue(Canvas.TopProperty);
            //    lb1.Content = oldpositionX;
           //     lb2.Content = oldpositionY;
            }
            
        }

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Ellipse item = sender as System.Windows.Shapes.Ellipse;
            if (!changemode && item.IsMouseCaptured)  //如果是上色模式就不執行
            {
                
                timershow.Stop();
                timersec.Stop();
                item.ReleaseMouseCapture();
                Canvas.SetZIndex(item, 0);  //物體位置回歸水平
                mouseX = -1;
                mouseY = -1;
                item.SetValue(Canvas.LeftProperty, oldpositionX); //轉完自動歸位圓圈
                item.SetValue(Canvas.TopProperty, oldpositionY);  //  
            }
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Shapes.Ellipse item = sender as System.Windows.Shapes.Ellipse;
            if (item.IsMouseCaptured)
            {
                System.Windows.Point pt = new System.Windows.Point(mouseX, mouseY);
                VisualTreeHelper.HitTest(this, null,
                new HitTestResultCallback(myHitTestResult),
                new PointHitTestParameters(pt));
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
                /*Geometry g = item.RenderedGeometry;
            //    座標位置轉換為視窗的座標
                g.Transform = item.TransformToAncestor(this) as MatrixTransform;
                VisualTreeHelper.HitTest(this, null,
                    new HitTestResultCallback(myHitTestResult),
                    new GeometryHitTestParameters(g));*/
            }
        }
           
  
        public HitTestResultBehavior myHitTestResult(HitTestResult result)
        {
            if (result.VisualHit is System.Windows.Shapes.Ellipse && result.VisualHit != CurrentRec)
            {
                System.Windows.Shapes.Ellipse rect = result.VisualHit as System.Windows.Shapes.Ellipse;
            //    rect.Fill = new SolidColorBrush(Colors.Red);            
                //if (currentshapX > oldpositionX + 25 || currentshapX < oldpositionX - 25 || currentshapY > oldpositionY + 25 || currentshapY < oldpositionY - 25)
                //if (((currentshapX - oldpositionX) * (currentshapX - oldpositionX) + (currentshapY - oldpositionY) * (currentshapY - oldpositionY)) > 1200)
                {
                    double tempX=0, tempY=0;
                    int temp;
                    tempX = (double)rect.GetValue(Canvas.LeftProperty);                  
                    tempY = (double)rect.GetValue(Canvas.TopProperty);
                    if(tempX != oldpositionX || tempY != oldpositionY)
                    {
                    rect.SetValue(Canvas.LeftProperty, oldpositionX);
                    rect.SetValue(Canvas.TopProperty, oldpositionY);
              //      lb1.Content = currentshapX;
                //    lb2.Content = currentshapY;
                    temp = orb[(int)(oldpositionY - 40) / 55, (int)(oldpositionX - 40) / 55];
                    orb[(int)(oldpositionY - 40) / 55, (int)(oldpositionX - 40) / 55] = orb[(int)(tempY - 40) / 55, (int)(tempX - 40) / 55];
                    orb[(int)(tempY - 40) / 55, (int)(tempX - 40) / 55] = temp;
                    oldpositionX = tempX;
                    oldpositionY = tempY;
                    }
                 //   lb1.Content = oldpositionX;
                  //  lb2.Content = oldpositionY;
                }            
            }
            return HitTestResultBehavior.Continue;
        }

        private void btrand_Click(object sender, RoutedEventArgs e)
        {
            can1.Children.RemoveRange(37,45); //從第18個產生的物件開始移除(移除拼圖)
            clearTB();
            creatpuzzle(5, 6);
        }

        private void clearTB()
        {
            tbdark.Clear();
            tbfire.Clear();
            tbgold.Clear();
            tbheart.Clear();
            tbwater.Clear();
            tbwood.Clear();
            textbox1.Clear();
        }



        private void btchange_color_Click(object sender, RoutedEventArgs e)
        {
            changemode = true;
            adv_ChangeMode = false;
            lb_Mode.Content = "換色模式";
        }

        private void btspin_mode_Click(object sender, RoutedEventArgs e)
        {
            changemode = false;
            adv_ChangeMode = false;
            lb_Mode.Content = "轉珠模式";
        }


        void timersec_Tick(object sender, EventArgs e)
        {
            // this.lbtimer.Content = (int.Parse(this.lbtimer.Content.ToString()) + 1).ToString();
            timersec.Stop();
            timershow.Stop();

            this.lbtimer.Content = (int.Parse(this.lbtimer.Content.ToString()) + 1).ToString() + "秒到了";
            Ellipse_MouseUp(CurrentRec, curmouse);
        }

        private void timershow_Tick(object sender, EventArgs e)
        {
            this.lbtimer.Content = (int.Parse(this.lbtimer.Content.ToString()) + 1).ToString();

        }

        private void Postiontimer_1Tick(object sender, EventArgs e)
        {
          //  this.plabel.Content = "(" + System.Windows.Forms.Cursor.Position.X.ToString() + "," + System.Windows.Forms.Cursor.Position.Y.ToString() + ")";
        }

        private void btn_combo_Click(object sender, RoutedEventArgs e)
        {
            comwood = 0; comwater = 0; comfire = 0; comgold = 0; comdark = 0; comheart = 0;
            countwood = 0; countwater = 0; countfire = 0; countgold = 0; countdark = 0; countheart = 0;
            int count;
            int[,] temp = new int[5,6];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    temp[i, j] = orb[i, j];
                    //textbox1.Text += orb[i, j].ToString() + " ";
                }
                //textbox1.Text += "\n";
            }
            count = find_combo(temp);
            textbox1.Text = "總共有 " + count.ToString() + " Combo\n";
            tbdark.Text = "總共 " + comdark + " Combo 共 " + countdark + "顆";
            tbgold.Text = "總共 " + comgold + " Combo 共 " + countgold + "顆";
            tbwood.Text = "總共 " + comwood + " Combo 共 " + countwood + "顆";
            tbwater.Text = "總共 " + comwater + " Combo 共 " + countwater + "顆";
            tbfire.Text = "總共 " + comfire + " Combo 共 " + countfire + "顆";
            tbheart.Text = "總共 " + comheart + " Combo 共 " + countheart + "顆";
        }

        private void Red_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            changemode = true;
            adv_ChangeMode = true;
            adv_type = 3;
            lb_Mode.Content = "換色模式";
        }

        private void Yellow_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            changemode = true;
            adv_ChangeMode = true;
            adv_type = 1;
            lb_Mode.Content = "換色模式";
        }

        private void Dard_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            changemode = true;
            adv_ChangeMode = true;
            adv_type = 2;
            lb_Mode.Content = "換色模式";
        }

        private void Heart_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            changemode = true;
            adv_ChangeMode = true;
            adv_type = 6;
            lb_Mode.Content = "換色模式";
        }

        private void Water_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            changemode = true;
            adv_ChangeMode = true;
            adv_type = 4;
            lb_Mode.Content = "換色模式";
        }

        private void Wood_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            changemode = true;
            adv_ChangeMode = true;
            adv_type = 5;
            lb_Mode.Content = "換色模式";
        }

        private void Record_Bt_Click(object sender, RoutedEventArgs e)
        {
            clearTB();
            textbox1.Text = "盤面已記錄";
            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 6; j++) {
                    record_orb[i, j] = orb[i, j];
                }
            }
        }

        private void Recover_Bt_Click(object sender, RoutedEventArgs e)
        {
            clearTB();
            textbox1.Text = "盤面已恢復";
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    orb[i, j]=record_orb[i, j] ;
                }
            }
            can1.Children.RemoveRange(37, 45); 
            wood = 0; water = 0; fire = 0; gold = 0; dark = 0; heart = 0;
            for (int a = 0; a < 5; a++)
            {
                for (int b = 0; b < 6; b++)
                {
                    System.Windows.Shapes.Ellipse rec = new System.Windows.Shapes.Ellipse()
                    {
                        Height = 50,
                        Width = 50,
                        //Stroke = new SolidColorBrush(Colors.Transparent),                      
                    };
                    ImageBrush brush = null;
                    //問助教為什麼要用image 不能用new的   ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("images/gold.png", UriKind.Relative)));      
                   
                    switch (record_orb[a,b])
                    {
                        case 1:
                            image1.Source = new BitmapImage(new Uri("images/gold.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            gold++;
                            orb[a, b] = 1;
                            break;
                        case 2:
                            image1.Source = new BitmapImage(new Uri("images/dark.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            dark++;
                            orb[a, b] = 2;
                            break;
                        case 3:
                            image1.Source = new BitmapImage(new Uri("images/fire.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            fire++;
                            orb[a, b] = 3;
                            break;
                        case 4:
                            image1.Source = new BitmapImage(new Uri("images/water.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            water++;
                            orb[a, b] = 4;
                            break;
                        case 5:
                            image1.Source = new BitmapImage(new Uri("images/wood.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            wood++;
                            orb[a, b] = 5;
                            break;
                        case 6:
                            image1.Source = new BitmapImage(new Uri("images/heart.png", UriKind.Relative));
                            brush = new ImageBrush(image1.Source);
                            heart++;
                            orb[a, b] = 6;
                            break;
                        default:
                            MessageBox.Show("asfd no puzzle!!!!!");
                            break;
                    }                  
                    rec.Fill = brush;
                    rec.MouseDown += new MouseButtonEventHandler(this.Ellipse_MouseDown);
                    rec.MouseMove += new MouseEventHandler(this.Ellipse_MouseMove);
                    rec.MouseUp += new MouseButtonEventHandler(this.Ellipse_MouseUp);
                    Canvas.SetTop(rec, 40 + a * 55);
                    Canvas.SetLeft(rec, 40 + b * 55);
                    can1.Children.Add(rec);
                }
            }
            lbdark.Content = dark + "顆";
            lbwater.Content = water + "顆";
            lbfire.Content = fire + "顆";
            lbwood.Content = wood + "顆";
            lbgold.Content = gold + "顆";
            lbheart.Content = heart + "顆";
            lbcombo.Content = "最大combo數: " + ((dark / 3) + (water / 3) + (fire / 3) + (wood / 3) + (gold / 3) + (heart / 3)); 

         
        }

        }
    }


