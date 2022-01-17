using ChemCompLogger.model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChemCompLogger.view
{
    internal class AppView
    {
        // view-model objects/references
        internal AppConsole appConsole;
        private Button[] buttons;
        private Grid buttonGrid;

        /* to avoid magic number usage, but cannot be changed
         * without making modification to MainWindow.xaml */
        private const int buttonCount = 8;

        private MainWindow mainWindow;

        private AppState appState;

        private double initialWinWidth;
        private double initialWinHeight;

        private double initialTop;
        private double initialLeft;

        internal bool AppSnappedToSide
        {
            get; private set;
        }

        internal bool LogPanelShown
        {
            get; private set;
        }

        internal bool ShiftHeld
        {
            get; private set;
        }

        public AppView(MainWindow mw, AppState appState, AppConsole appConsole, Grid buttonGrid)
        {
            mainWindow = mw;
            mainWindow.Width = initialWinWidth = SystemParameters.WorkArea.Width * (3.0D/4.0D);
            mainWindow.Height = initialWinHeight = SystemParameters.WorkArea.Height * (3.0D/4.0D);
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            AppSnappedToSide = false;
            LogPanelShown = true;

            this.appState = appState;
            this.appConsole = appConsole;

            this.buttonGrid = buttonGrid;

            // initialize buttons
            buttons = new Button[buttonCount];
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i] = new Button();
                buttons[i].Name = $"b{i}";
                buttons[i].Click += ButtonClicked;

                // add button to buttonGrid
                this.buttonGrid.Children.Add(buttons[i]);

                // layout buttons in 2 column layout
                if (i % 2 == 1)
                {
                    Grid.SetColumn(buttons[i], 1);
                }
                if (i > 1)
                {
                    Grid.SetRow(buttons[i], i / 2);
                }
            }

            ToggleLogPanel();
            loadMenu();

            mw.KeyDown -= MainWindow_KeyDown;
            mw.KeyDown += MainWindow_KeyDown;
            mw.KeyUp -= MainWindow_KeyUp;
            mw.KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.IsUp && (e.Key == Key.LeftShift || e.Key == Key.RightShift))
            {
                ShiftHeld = false;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsDown && (e.Key == Key.LeftShift || e.Key == Key.RightShift))
            {
                ShiftHeld = true;
            }
        }

        internal void CloseOnMainMenu()
        {
            if (appState.GetMenuState().GetCurrentPage() == "main_menu" ||
                appState.GetMenuState().GetCurrentPage() == "view_active_doses")
            {
                mainWindow.Close();
            }
        }

        internal void PosOrSizeChanged()
        {
            var windowCloseToHalfWidth = Math.Abs(mainWindow.Width - SystemParameters.WorkArea.Width / 2) < 16;
            var windowCloseToFullHeight = Math.Abs(mainWindow.Height - SystemParameters.WorkArea.Height) < 16;

            if (mainWindow.Left <= 0 && mainWindow.Top < 8 && windowCloseToHalfWidth && windowCloseToFullHeight)
            {
                if (LogPanelShown)
                {
                    ToggleLogPanel();
                }
                AppSnappedToSide = true;
            }
            else if (mainWindow.Left > SystemParameters.WorkArea.Width / 2 - 16 &&
                mainWindow.Top < 8 && windowCloseToHalfWidth && windowCloseToFullHeight)
            {
                if (LogPanelShown)
                {
                    ToggleLogPanel();
                }
                AppSnappedToSide = true;
            }
            else
            {
                AppSnappedToSide = false;
            }
        }

        internal void RecordInitialWindowValues()
        {
            initialTop = mainWindow.Top;
            initialLeft = mainWindow.Left;
        }

        internal void PositionInitial()
        {
            mainWindow.Left = initialLeft;
            mainWindow.Top = initialTop;
            mainWindow.Width = initialWinWidth;
            mainWindow.Height = initialWinHeight;
            mainWindow.WindowState = WindowState.Normal;
            if (!LogPanelShown)
            {
                ToggleLogPanel();
            }
        }

        internal void PositionLeft()
        {
            mainWindow.Left = 0;
            mainWindow.Top = 0;
            mainWindow.Width = SystemParameters.WorkArea.Width / 2;
            mainWindow.Height = SystemParameters.WorkArea.Height;
            mainWindow.WindowState = WindowState.Normal;
        }

        internal void PositionRight()
        {
            mainWindow.Left = SystemParameters.WorkArea.Width / 2;
            mainWindow.Top = 0;
            mainWindow.Width = SystemParameters.WorkArea.Width / 2;
            mainWindow.Height = SystemParameters.WorkArea.Height;
            mainWindow.WindowState = WindowState.Normal;
        }

        internal void PositionMax()
        {
            mainWindow.WindowState = WindowState.Maximized;
        }

        internal void ToggleLogPanel()
        {
            if (mainWindow.textBoxGameStatus.Visibility == Visibility.Visible)
            {
                mainWindow.textBoxGameStatus.Visibility = Visibility.Hidden;
                mainWindow.MasterGrid.ColumnDefinitions.RemoveAt(1);
                LogPanelShown = false;
            }
            else if (mainWindow.textBoxGameStatus.Visibility == Visibility.Hidden)
            {
                mainWindow.textBoxGameStatus.Visibility = Visibility.Visible;
                mainWindow.MasterGrid.ColumnDefinitions.Add(new ColumnDefinition());
                LogPanelShown = true;
            }
        }

        private void loadMenu()
        {
            // for now, write the opening text of first page after loading story
            // TODO: title page format perhaps or special field in metadata object
            appState.GetMenuState().ShowOpeningText();
            UpdateButtons();
        }

        // if a left-panel button is clicked, handle the event
        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            // determine Button which triggered the event
            Button button = (Button)sender;
            int buttonNumber = int.Parse(button.Name.Substring(1));

            // if an action is associated w/ the button, this will return 0
            // otw, the page will be changed by this function and the new page
            // number will be returned
            var pageRedirectedTo = appState.GetMenuState().ButtonClicked(buttonNumber);
            if (!string.IsNullOrWhiteSpace(pageRedirectedTo))
            {
                PageChanged(pageRedirectedTo);
                return;
            }

            string action = appState.GetMenuState().ButtonAction(buttonNumber);
            switch (action)
            {
                case "generate_mazes":
                    appState.GameGenerator.CreateMazes(appState);
                    break;

                case "change_shape":
                    appState.ChangeMazeShape();
                    var mb5 = appState.GetMenuState().GetButton(1);
                    mb5.text = "Maze Shape\n" + "{ Current: " + (appState.MazeShape) + " }";
                    UpdateButtons();
                    break;

                case "add_maze_count":
                    if (ShiftHeld)
                    {
                        if (appState.UniqueMazes == 1)
                        {
                            appState.UniqueMazes = 10;
                        }
                        else
                        {
                            appState.UniqueMazes += 10;
                        }
                    }
                    else
                    {
                        appState.UniqueMazes += 1;
                    }
                    var mb3 = appState.GetMenuState().GetButton(2);
                    mb3.text = "Unique Mazes\n" + "{ Current: " + (appState.UniqueMazes) + " }";
                    UpdateButtons();
                    break;

                case "reset_maze_count":
                    appState.UniqueMazes = 1;
                    var mb4 = appState.GetMenuState().GetButton(2);
                    mb4.text = "Unique Mazes\n" + "{ Current: " + (appState.UniqueMazes) + " }";
                    UpdateButtons();
                    break;

                case "toggle_diagonals":
                    var mb2 = appState.GetMenuState().GetButton(4);
                    appState.UseDiagonals = !appState.UseDiagonals;
                    if (appState.UseDiagonals)
                    {
                        mb2.text = "Diagonal Connections\n{ " + appState.UseDiagonals +
                        " }";
                    }
                    else
                    {
                        mb2.text = "Diagonal Connections\n{ " + appState.UseDiagonals +
                        " }";
                    }
                    UpdateButtons();
                    break;

                case "change_difficulty_enhancer":
                    appState.ChangeDifficultyEnhancer();
                    var mb1 = appState.GetMenuState().GetButton(5);
                    mb1.text = "Difficulty Enhancer\n{ " + appState.DifficultyEnhancer +
                        " }";
                    UpdateButtons();
                    break;

                case "change_style":
                    appState.ChangeMazeStyle();
                    var mb6 = appState.GetMenuState().GetButton(6);
                    mb6.text = "Maze Style\n" + "{ " + (appState.MazeStyle) + " }";
                    UpdateButtons();
                    break;

                case "toggle_log_panel":
                    ToggleLogPanel();
                    break;
            }
        }

        private void ChangePage(string page)
        {
            appState.GetMenuState().SetCurrentPage(page);
            PageChanged(page);
        }

        private void PageChanged(string pageOpened)
        {
            switch (pageOpened)
            {
                // hide tableHeader on following pages
                case "main_menu":
                    mainWindow.tableHeader.Visibility = Visibility.Hidden;
                    break;
                default:
                    mainWindow.tableHeader.Visibility = Visibility.Visible;
                    mainWindow.tableHeader.FontSize = 18;
                    break;
            }

            UpdateButtons();
        }

        /* called when a story is first loaded, and whenever a button
        * is clicked while the StoryState is on a given page */
        private void UpdateButtons()
        {
            var displays = appState.GetMenuState().GetButtonDisplays();
            for (int i = 0; i < buttonCount; i++)
            {
                var text = displays[i];
                if (text == null)
                {
                    buttons[i].Visibility = Visibility.Hidden;
                } else
                {
                    string buttonText = text.Trim();
                    string formattedButtonText = "";
                    char[] buttonTextChars = buttonText.ToCharArray();
                    // format buttonText to replace newlines w/ special escape string for XAML line break
                    for (int ci = 0; ci < buttonTextChars.Length; ci++)
                    {
                        if (buttonTextChars[ci] == '\n')
                        {
                            formattedButtonText += Environment.NewLine;
                        }
                        else
                        {
                            formattedButtonText += buttonTextChars[ci];
                        }
                    }
                    buttons[i].Content = formattedButtonText;
                    buttons[i].Visibility = Visibility.Visible;
                }
            }
        }

        internal void AppClosing()
        {
            
        }
    }
}
