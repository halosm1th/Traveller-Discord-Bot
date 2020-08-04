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

namespace TravellerGalaxyMapDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void DrawGalaxy(object sender, RoutedEventArgs e)
        {
            var splits = new[] {
                2, //Galaxy is a 2x2 quadrants
                3, //Quadrant is 3x3 megasectors
                8,//megasector is 8x8 Super Sectors
                6, //super secgtors are 6x6 sectors
                4//sectors are 4x4 subsectors
            };

        }

        private void QuadrantDownLine()
        {
            var quadrantDownLine = new Line();
            quadrantDownLine.X1 = GalaxyCanvas.Width / 2;
            quadrantDownLine.X2 = GalaxyCanvas.Width / 2;
            quadrantDownLine.Y1 = 0;
            quadrantDownLine.Y2 = GalaxyCanvas.Height;
            quadrantDownLine.Stroke = System.Windows.Media.Brushes.Black;
            GalaxyCanvas.Children.Add(quadrantDownLine);
        }
    }
}
