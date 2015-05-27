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


        public MainWindow()
        {
            InitializeComponent();
            CreateBytes();
        }

        private void CreateBytes()
        {
            var item = this.text;

            this.text.Text = "B";
            grid.Measure(grid.DesiredSize);
            grid.Arrange(new Rect(grid.DesiredSize));
            grid.UpdateLayout();

            var rtb = new RenderTargetBitmap(
                                    (int)item.Width,
                                    (int)item.Height,
                                    96,
                                    96,
                                    PixelFormats.Pbgra32);
            //var pixels = new Int32Rect(0, 0, rtb.PixelWidth, rtb.PixelHeight);
            rtb.Render(this.text);
            var stride = (int)item.Width * (rtb.Format.BitsPerPixel / 8);
            var pixels = new byte[stride * (int) item.Height];
            stride += (4 - stride % 4);

            rtb.CopyPixels(pixels, stride, 0);
            //interpret by 84 (21 width??) - measure start x,y and w,h
        }
    }
}
