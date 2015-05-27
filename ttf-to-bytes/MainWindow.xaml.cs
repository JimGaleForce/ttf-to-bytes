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

namespace ttf_to_bytes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int width = 12;
        private const int height = 12;
        private const int area = width*height;

        public MainWindow()
        {
            InitializeComponent();
            CreateBytes();
        }

        private void CreateBytes()
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                var pixels = CreateBytes1(c);


                break; //just testing
            }
        }

        private byte[,] CreateBytes1(char c)
        {
            this.text.Text = c.ToString();

            var item = this.text;

            var color = Brushes.White;

            grid.Measure(grid.DesiredSize);
            grid.Arrange(new Rect(grid.DesiredSize));
            grid.UpdateLayout();

            var rtb = new RenderTargetBitmap(
                                    (int)item.Width,
                                    (int)item.Height,
                                    96,
                                    96,
                                    PixelFormats.Pbgra32);

            rtb.Render(this.text);
            var stride = (int)item.Width * (rtb.Format.BitsPerPixel / 8);
            stride += (4 - stride % 4);
            var pixels = new byte[stride * (int) item.Height];

            rtb.CopyPixels(pixels, stride, 0);
            //interpret by 84 (21 width??) - measure start x,y and w,h
            var monoGrid = new byte[height, width];

            int x0 = int.MaxValue, x1 = -1, y0 = int.MaxValue, y1 = -1;
            for (var x = 0; x < Math.Min((int) item.Width, width); x++)
            {
                for (var y = 0; y < Math.Min((int)item.Height, height); y++)
                {
                    if (HasValue(pixels, stride, x, y))
                    {
                        x0 = Math.Min(x0, x);
                        y0 = Math.Min(y0, y);
                        x1 = Math.Max(x1, x);
                        y1 = Math.Max(y1, y);

                        monoGrid[y, x] = 255;
                        canvas.Children.Add(new Line {X1 = x, X2 = x+1, Y1 = y, Y2 = y, Stroke = color, StrokeThickness = 1});
                        canvas.Children.Add(new Line { X1 = x, X2 = x, Y1 = y, Y2 = y + 1, Stroke = color, StrokeThickness = 1 });
                    }
                }
            }

            grid.UpdateLayout();

            return monoGrid;
        }

        bool HasValue(byte[] pixels, int stride, int x, int y)
        {
            return pixels[y*stride + x*4] == 255;
        }
    }
}
