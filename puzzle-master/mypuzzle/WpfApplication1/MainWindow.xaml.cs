﻿using System;
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
        DispatcherTimer timersec, timershow;
        MouseButtonEventArgs curmouse = null;
        bool changemode = false;             //手動變換珠子顏色 判斷是不是轉珠模式
        int wood = 0, water = 0, fire = 0, gold = 0, dark = 0, heart = 0;
        int[,] orb = new int[5, 6];
        int[,] zero = new int[5, 6] { { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0 } };

        public MainWindow()
        {
            // timer顯示座標
            DispatcherTimer Postiontimer;
            Postiontimer = new DispatcherTimer();
            Postiontimer.Tick += Postiontimer_1Tick;
            Postiontimer.Start();
            //          
            InitializeComponent();
            creatpuzzle(5, 6);
        }

        private void creatpuzzle(int x, int y) //動態生成珠子
        {
            Array.Clear(zero, 0, zero.Length);
            wood = 0; water = 0; fire = 0; gold = 0; dark = 0; heart = 0;
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    System.Windows.Shapes.Rectangle rec = new System.Windows.Shapes.Rectangle()
                    {
                        Height = 50,
                        Width = 50,
                        Stroke = new SolidColorBrush(Colors.Black),
                    };
                    randomfill(i, j, rec);    //隨機給珠子顏色           
                    rec.MouseDown += new MouseButtonEventHandler(this.Rectangle_MouseDown);
                    rec.MouseMove += new MouseEventHandler(this.Rectangle_MouseMove);
                    rec.MouseUp += new MouseButtonEventHandler(this.Rectangle_MouseUp);
                    Canvas.SetTop(rec, 40 + i * 55);
                    Canvas.SetLeft(rec, 40 + j * 55);
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

        private void randomfill(int a, int b, System.Windows.Shapes.Rectangle rec)//隨機上色
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int x = rnd.Next(6);
            ImageBrush brush = null;
            //問助教為什麼要用image 不能用new的   ImageBrush brush = new ImageBrush(new BitmapImage(new Uri("images/gold.png", UriKind.Relative)));      
            switch (x)
            {//金1 暗2 火3 水4 木5 心6
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
                    MessageBox.Show("no rpuzzle!!!!!");
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
        System.Windows.Shapes.Rectangle CurrentRec = null;  //宣告目前移動的正方形

        private int find_combo(int[,] ori)
        {
            //int[,] ori = orb;
            int count = 0;
            bool judge = false;
            int[,] temp = new int[5, 6];
            // 1. filter all 3+ consecutives.
            //  (a) horizontals
            for (var i = 0; i < 5; ++i)
            {
                var prev_1_orb = 19;
                var prev_2_orb = 19;
                for (var j = 0; j < 6; ++j)
                {
                    var cur_orb = ori[i, j];
                    if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != 19 && cur_orb != 29)
                    {
                        temp[i, j] = cur_orb;
                        temp[i, j - 1] = cur_orb;
                        temp[i, j - 2] = cur_orb;
                    }
                    prev_1_orb = prev_2_orb;
                    prev_2_orb = cur_orb;
                }
            }
            //  (b) verticals
            for (var j = 0; j < 6; ++j)
            {
                var prev_1_orb = 19;
                var prev_2_orb = 19;
                for (var i = 0; i < 5; ++i)
                {
                    var cur_orb = ori[i, j];
                    if (prev_1_orb == prev_2_orb && prev_2_orb == cur_orb && cur_orb != 19 && cur_orb != 29)
                    {
                        temp[i, j] = cur_orb;
                        temp[i - 1, j] = cur_orb;
                        temp[i - 2, j] = cur_orb;
                    }
                    prev_1_orb = prev_2_orb;
                    prev_2_orb = cur_orb;
                }
            }

            int[,] temp2 = ori;
            /*for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 6; j++)
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
            //for (int i = 0; i < 5; i++)   //印出combo狀態
            //{
            //    for (int j = 0; j < 6; j++)
            //    {
            //        textbox1.Text += temp[i, j].ToString() + " ";
            //    }
            //    textbox1.Text += "\n";
            //}
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
                    if (ori[i, j] != 0)
                    {
                        //combojudge = true;
                        ori = setzero(ori, i, j);
                        count++;
                    }
                }
            return count;
        }

        private int[,] setzero(int[,] ori, int i, int j)
        {
            int temp = ori[i, j];
            ori[i, j] = 0;
            if (i > 0)
                if (temp == ori[i - 1, j])
                    ori = setzero(ori, i - 1, j);
            if (i < 4)
                if (temp == ori[i + 1, j])
                    ori = setzero(ori, i + 1, j);
            if (j > 0)
                if (temp == ori[i, j - 1])
                    ori = setzero(ori, i, j - 1);
            if (j < 5)
                if (temp == ori[i, j + 1])
                    ori = setzero(ori, i, j + 1);
            return ori;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (changemode)  //換色模式
            {
                double x, y;
                System.Windows.Shapes.Rectangle item = sender as System.Windows.Shapes.Rectangle;
                // item.Fill = new SolidColorBrush(Colors.Red);  //delete later, just for mouse test
                ImageBrush brush = null;
                //image1.Source = new BitmapImage(new Uri("images/dark.png", UriKind.Relative));
                //brush = new ImageBrush(image1.Source);
                //item.Fill = brush;
                y = (double)item.GetValue(Canvas.LeftProperty);
                x = (double)item.GetValue(Canvas.TopProperty);
                x = (x - 40) / 55;
                y = (y - 40) / 55;
                lb1.Content = x;
                lb2.Content = y;
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
                        //   MessageBox.Show("no puzzle!!!!!");
                        break;
                }
                lbdark.Content = "暗珠: " + dark + "顆";
                lbwater.Content = "水珠: " + water + "顆";
                lbfire.Content = "火珠: " + fire + "顆";
                lbwood.Content = "木珠: " + wood + "顆";
                lbgold.Content = "光珠: " + gold + "顆";
                lbheart.Content = "心珠: " + heart + "顆";
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

                System.Windows.Shapes.Rectangle item = sender as System.Windows.Shapes.Rectangle;
                CurrentRec = item;  //記錄下要移動的正方形(hittest才可以判斷)
                // item.Fill = new SolidColorBrush(Colors.Red);  //delete later, just for mouse test
                Canvas.SetZIndex(item, 100);  //移動的物體總是在最上方
                mouseX = e.GetPosition(null).X;
                mouseY = e.GetPosition(null).Y;
                item.CaptureMouse();
                oldpositionX = (double)item.GetValue(Canvas.LeftProperty);
                oldpositionY = (double)item.GetValue(Canvas.TopProperty);
                lb1.Content = oldpositionX;
                lb2.Content = oldpositionY;
            }
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Shapes.Rectangle item = sender as System.Windows.Shapes.Rectangle;
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
            if (result.VisualHit is System.Windows.Shapes.Rectangle && result.VisualHit != CurrentRec)
            {
                System.Windows.Shapes.Rectangle rect = result.VisualHit as System.Windows.Shapes.Rectangle;
                //    rect.Fill = new SolidColorBrush(Colors.Red);            
                if (currentshapX > oldpositionX + 40 || currentshapX < oldpositionX - 40 || currentshapY > oldpositionY + 40 || currentshapY < oldpositionY - 40)
                {
                    double tempX, tempY;
                    int temp;
                    tempX = (double)rect.GetValue(Canvas.LeftProperty);
                    tempY = (double)rect.GetValue(Canvas.TopProperty);
                    rect.SetValue(Canvas.LeftProperty, oldpositionX);
                    rect.SetValue(Canvas.TopProperty, oldpositionY);
                    //lb1.Content = oldpositionX;
                    //lb2.Content = oldpositionY;
                    temp = orb[(int)(oldpositionY - 40) / 55, (int)(oldpositionX - 40) / 55];
                    orb[(int)(oldpositionY - 40) / 55, (int)(oldpositionX - 40) / 55] = orb[(int)(tempY - 40) / 55, (int)(tempX - 40) / 55];
                    orb[(int)(tempY - 40) / 55, (int)(tempX - 40) / 55] = temp;
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
            can1.Children.RemoveRange(19, 36); //從第18個產生的物件開始移除(移除拼圖)
            creatpuzzle(5, 6);
        }



        private void btchange_color_Click(object sender, RoutedEventArgs e)
        {
            changemode = true;
            lb_Mode.Content = "換色模式";
        }

        private void btspin_mode_Click(object sender, RoutedEventArgs e)
        {
            changemode = false;
            lb_Mode.Content = "轉珠模式";
        }

        void timersec_Tick(object sender, EventArgs e)
        {
            // this.lbtimer.Content = (int.Parse(this.lbtimer.Content.ToString()) + 1).ToString();
            timersec.Stop();
            timershow.Stop();

            this.lbtimer.Content = (int.Parse(this.lbtimer.Content.ToString()) + 1).ToString() + "秒到了";
            Rectangle_MouseUp(CurrentRec, curmouse);
        }

        private void timershow_Tick(object sender, EventArgs e)
        {
            this.lbtimer.Content = (int.Parse(this.lbtimer.Content.ToString()) + 1).ToString();

        }

        private void Postiontimer_1Tick(object sender, EventArgs e)
        {
            this.plabel.Content = "(" + System.Windows.Forms.Cursor.Position.X.ToString() + "," + System.Windows.Forms.Cursor.Position.Y.ToString() + ")";
        }

        private void btn_combo_Click(object sender, RoutedEventArgs e)
        {
            textbox1.Clear();
            int count;
            int[,] temp = orb;
            count = find_combo(temp);
            textbox1.Text += "總共有 " + count.ToString() + " Combo";
        }
    }


}
