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
            StringBuilder b = new StringBuilder();

            for (char c = 'A'; c <= 'Z'; c++)
            {
                var pixels = CreateBytes1(c);
                b.AppendLine(c + "=" + pixels);
            }
        }

        private string CreateBytes1(char c)
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
            var monoGrid = new bool[height, width];

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

                        monoGrid[y, x] = true;
                        canvas.Children.Add(new Line {X1 = x, X2 = x+1, Y1 = y, Y2 = y, Stroke = color, StrokeThickness = 1});
                        canvas.Children.Add(new Line { X1 = x, X2 = x, Y1 = y, Y2 = y + 1, Stroke = color, StrokeThickness = 1 });
                    }
                }
            }

            char[] hex = {'0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'};

            StringBuilder b = new StringBuilder();
            b.AppendFormat("!{0}{1}{2}{3}", hex[x0], hex[y0], hex[x1 - x0 + 1], hex[y1 - y0 + 1]);

            for (var x = x0; x<=x1; x++)
            {
                var bit = 0;
                var value = 0;
                for (var y = y0; y<=y1; y++)
                {
                    if (monoGrid[y,x])
                    {
                        value = value | (1 << bit);
                    }

                    if (++bit == 4)
                    {
                        b.Append(hex[value]);
                        value = 0;
                        bit = 0;
                    }
                }

                if (bit > 0)
                {
                    b.Append(hex[value]);
                }
            }

            grid.UpdateLayout();

            return b.ToString();
        }

        bool HasValue(byte[] pixels, int stride, int x, int y)
        {
            return pixels[y*stride + x*4] == 255;
        }
    }
}
