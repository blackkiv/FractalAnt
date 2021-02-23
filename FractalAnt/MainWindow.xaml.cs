using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FractalAnt
{
    public partial class MainWindow : Window
    {
        private int currColumn;
        private Rectangle currRectangle;
        private int currRow;

        private int direction; // 0 - top;

        private List<List<Rectangle>> rectangles;
        private int speed;
        private Thread thread;

        public MainWindow()
        {
            InitializeComponent();
        }
        // 1 - right
        // 2 - bottom
        // 3 - left

        private async void Painter()
        {
            try
            {
                SolidColorBrush colorBrush = null;
                Dispatcher.Invoke(() =>
                {
                    colorBrush = (SolidColorBrush) currRectangle.Fill;

                    if (colorBrush.Color == Colors.Black)
                    {
                        direction -= 1;
                        if (direction < 0)
                            direction = 3;
                    }
                    else
                    {
                        direction += 1;
                        if (direction > 3)
                            direction = 0;
                    }
                });
                var columnK = 0;
                var rowK = 0;
                switch (direction)
                {
                    case 0:
                        rowK = -1;
                        columnK = 0;
                        break;
                    case 1:
                        rowK = 0;
                        columnK = 1;
                        break;
                    case 2:
                        rowK = 1;
                        columnK = 0;
                        break;
                    case 3:
                        rowK = 0;
                        columnK = -1;
                        break;
                    default:
                        MessageBox.Show("wtf?!?!");
                        break;
                }

                currColumn += columnK;
                currRow += rowK;
                try
                {
                    currRectangle = rectangles[currRow][currColumn];
                }
                catch (Exception)
                {
                    Stop_App();
                }

                Dispatcher.Invoke(() =>
                {
                    if (colorBrush.Color == Colors.Black)
                        colorBrush.Color = Colors.White;
                    else
                        colorBrush.Color = Colors.Black;
                });
                Thread.Sleep(speed);
                thread = new Thread(
                    Painter
                );
                thread?.Start();
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        private void Initialize_gridPanel()
        {
            speed = Convert.ToInt32(sliderSpeed.Value);

            gridPanel.ColumnDefinitions.Clear();
            gridPanel.RowDefinitions.Clear();
            var size = Convert.ToInt32(sizeTextBox.Text);


            for (var i = 0; i < size; ++i)
            {
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                gridPanel.RowDefinitions.Add(new RowDefinition());
            }


            if (rectangles != null)
            {
                for (var index = 0; index < size; ++index) rectangles[index].Clear();

                rectangles.Clear();
            }

            rectangles = new List<List<Rectangle>>(size);

            for (var i = 0; i < size; ++i)
            {
                var sublist = new List<Rectangle>(size);
                for (var j = 0; j < size; ++j)
                {
                    var rectangle = new Rectangle();
                    rectangle.MouseLeftButtonUp += Rectangle_Click;
                    rectangle.Fill = new SolidColorBrush(Colors.White);
                    sublist.Add(rectangle);
                    Grid.SetRow(rectangle, i);
                    Grid.SetColumn(rectangle, j);
                    gridPanel.Children.Add(rectangle);
                }

                rectangles.Add(sublist);
            }
        }

        private void Rectangle_Click(object sender, RoutedEventArgs e)
        {
            Set_Start_Point(sender as Rectangle);
        }

        private void Stop_App()
        {
            thread?.Interrupt();
        }

        private void Set_Start_Point(Rectangle rectangle)
        {
            var column = Grid.GetColumn(rectangle);
            var row = Grid.GetRow(rectangle);
            currRectangle = rectangles[row][column];
            currColumn = column;
            currRow = row;
            direction = 0;

            thread = new Thread(
                Painter
            );
            thread?.Start();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            Initialize_gridPanel();
        }

        private void sliderSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speedTextBlock.Text = Convert.ToString(Convert.ToInt32(((Slider) sender).Value));
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            Stop_App();
        }
    }
}