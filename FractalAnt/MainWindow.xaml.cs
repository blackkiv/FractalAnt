using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace FractalAnt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Thread thread = null;

        private List<List<Rectangle>> rectangles = null;
        Rectangle currRectangle = null;
        int speed;
        int currColumn;
        int currRow;
        int direction;  // 0 - top;
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
                    colorBrush = ((SolidColorBrush)currRectangle.Fill);

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
                int columnK = 0;
                int rowK = 0;
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
                { Stop_App(); }

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
            catch( ThreadInterruptedException)
            {
                
            }
        }

        private void Initialize_gridPanel()
        {
            speed = Convert.ToInt32(sliderSpeed.Value);

            gridPanel.ColumnDefinitions.Clear();
            gridPanel.RowDefinitions.Clear();
            var size = Convert.ToInt32(sizeTextBox.Text);

            for (int i = 0; i < size; ++i)
            {
                gridPanel.ColumnDefinitions.Add(new ColumnDefinition());
                gridPanel.RowDefinitions.Add(new RowDefinition());
            }

            rectangles = new List<List<Rectangle>>(size);

            for (int i = 0; i < size; ++i)
            {
                var sublist = new List<Rectangle>(size);
                for (int j = 0; j < size; ++j)
                {
                    var rectangle = new Rectangle();
                    rectangle.MouseLeftButtonUp += Rectangle_Click;
                    //rectangle.Stroke = new SolidColorBrush(Colors.Black);
                    rectangle.Fill = new SolidColorBrush(Colors.White);
                    //if ((i + j) % 2 == 0)
                    //    rectangle.Fill = new SolidColorBrush(Colors.White);
                    //else
                    //    rectangle.Fill = new SolidColorBrush(Colors.Black);
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
            int column = Grid.GetColumn(rectangle);
            int row = Grid.GetRow(rectangle);
            //MessageBox.Show($"column = {column}\nrow = {row}");
            currRectangle = rectangles[row][column];
            currColumn = column;
            currRow = row;
            direction = 0;

            thread = new Thread(
                Painter
            );
            thread?.Start();

        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            Initialize_gridPanel();
        }

        private void sliderSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            speedTextBlock.Text = Convert.ToString(Convert.ToInt32(((Slider)sender).Value));
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            Stop_App();
        }
    }
}
