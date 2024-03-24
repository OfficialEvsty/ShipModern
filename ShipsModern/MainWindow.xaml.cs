using ShipsForm.GUI;
using ShipsForm.Logic.TilesSystem;
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
using ShipsForm.Data;
using ShipsForm.Exceptions;
using ShipsForm.Launching;
using ShipsForm.GUI.Elements;
using System.Runtime.InteropServices;
using ShipsModern.SupportEntities;

namespace ShipsModern
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        Launcher launcher;
        FraghtConstructor f_construct;


        private Painter m_painter;
        Point? mouseDownPos = null;
        int minDragLen = 5;
        public MainWindow()
        {
            InitializeComponent();
            AllocConsole();
            //Strict sequence
            Configuration.Init();
            SVGData.UploadAllData();
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            modelCanvas.Width = screenWidth;
            modelCanvas.Height = screenHeight;
            var config = Configuration.Instance;
            if (config is null)
                throw new ConfigFileDoesntExistError();
            Field.CreateField(true);
            var field = Field.Instance;
            if (field is null)
                throw new Exception();
            m_painter = new Painter(fieldImg, modelCanvas, field, config);
            Launcher launcher = new Launcher(m_painter);
            this.Background = Brushes.Transparent;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var point = Mouse.GetPosition(this);
            mouseDownPos = point;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var point = Mouse.GetPosition(this);
            if (mouseDownPos is null) return;

            int shiftX = (int)(point.X - mouseDownPos.Value.X);
            int shiftY = (int)(point.Y - mouseDownPos.Value.Y);
            int draggingLength = (int)Math.Pow(shiftX * shiftX + shiftY * shiftY, 0.5f);
            if (draggingLength >= minDragLen)
            {
                m_painter.OnShift(shiftX, shiftY);
            }
            mouseDownPos = null;
        }

        private void Decrease_Scale(object sender, RoutedEventArgs e)
        {
            ScaleChanger.DecreaseScale();
        }

        private void Increase_Scale(object sender, RoutedEventArgs e)
        {
            ScaleChanger.IncreaseScale();
        }
    }
}
