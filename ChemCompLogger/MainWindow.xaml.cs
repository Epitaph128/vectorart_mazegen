using ChemCompLogger.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChemCompLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private AppState appState;

        view.AppConsole appConsole;
        view.AppView appView;

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Vector Maze Generator";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            appConsole = new view.AppConsole(ref textBoxGameStatus);
            appState = new AppState(appConsole);
            appView = new view.AppView(this, appState, appConsole, ButtonGrid);

            // add listener for key presses
            this.PreviewKeyDown += new KeyEventHandler(HandleKeys);

            this.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
            this.Loaded += MainWindow_Loaded;
            this.SizeChanged += MainWindow_SizeChanged;
            this.LocationChanged += MainWindow_LocationChanged;

            // https://stackoverflow.com/questions/1127647/convert-system-drawing-icon-to-system-media-imagesource
            Bitmap bitmap = Properties.Resources.nootracker_icon_small_shadow.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource nootrackerIconBitMap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            this.Icon = nootrackerIconBitMap;
        }

        protected override void OnRender(DrawingContext obj)
        {
            base.OnRender(obj);
            appView.PosOrSizeChanged();
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            appView.PosOrSizeChanged();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            appView.PosOrSizeChanged();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            appView.RecordInitialWindowValues();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            appView.AppClosing();
        }

        // KEYPRESSES
        private void HandleKeys(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                appView.CloseOnMainMenu();
            }
            else if (e.Key == Key.F1)
            {
                appView.PositionInitial();
            }
            else if (e.Key == Key.F2)
            {
                appView.PositionLeft();
            }
            else if (e.Key == Key.F3)
            {
                appView.PositionRight();
            }
            else if (e.Key == Key.F4)
            {
                appView.PositionMax();
            }
        }
    }
}
