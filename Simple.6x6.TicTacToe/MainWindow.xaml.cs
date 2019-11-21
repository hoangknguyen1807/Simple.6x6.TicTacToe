using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Simple._6x6.TicTacToe
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

        int Rows = 6;
        int Cols = 6;
        int countUsedSquared;
        int totalSquare;

        const int WinCondition = 5;
        List<Tuple<int, int>> ORIENT_HORIZONTAL;
        List<Tuple<int, int>> ORIENT_VERTICAL;
        List<Tuple<int, int>> ORIENT_SW_NE;
        List<Tuple<int, int>> ORIENT_NW_SE;

        bool isXTurn = true;

        int[,] _a;
        Button[,] _buttons;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            countUsedSquared = 0;
            totalSquare = Rows * Cols;
            const int ButtonWidth = 70;
            const int ButtonHeight = 70;
            const int Padding = 1;
            const int TopOffset = 80;
            const int LeftOffset = 24;

            labelStatus.Foreground = Brushes.Blue;
            labelStatus.Content = "X Turn";
            labelStatus.FontWeight = FontWeights.DemiBold;
            
            

            ORIENT_HORIZONTAL = new List<Tuple<int, int>>();
            ORIENT_VERTICAL = new List<Tuple<int, int>>();
            ORIENT_SW_NE = new List<Tuple<int, int>>();
            ORIENT_NW_SE = new List<Tuple<int, int>>();

            ORIENT_HORIZONTAL.Add(new Tuple<int, int>(0, -1));
            ORIENT_HORIZONTAL.Add(new Tuple<int, int>(0, 1));
            ORIENT_VERTICAL.Add(new Tuple<int, int>(-1, 0));
            ORIENT_VERTICAL.Add(new Tuple<int, int>(1, 0));
            ORIENT_SW_NE.Add(new Tuple<int, int>(1, -1));
            ORIENT_SW_NE.Add(new Tuple<int, int>(-1, 1));
            ORIENT_NW_SE.Add(new Tuple<int, int>(-1, -1));
            ORIENT_NW_SE.Add(new Tuple<int, int>(1, 1));

            // Model - Tao ra ma tran nut bam Rows x Cols
            _a = new int[Rows, Cols];
            _buttons = new Button[Rows, Cols];

            // UI
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    _a[i, j] = 0;
                    var button = new Button();
                    button.Width = ButtonWidth;
                    button.Height = ButtonHeight;
                    button.Tag = new Tuple<int, int>(i, j);
                    button.BorderThickness = new Thickness(1.5);
                    button.BorderBrush = Brushes.Black;
                    button.FontWeight = FontWeights.Bold;
                    button.FontSize = 32;
                    button.Click += Button_Click;
                    //button.Margin = new Thickness(10);

                    // Dua vao model quan li UI
                    _buttons[i, j] = button; // Luu tham chieu toi button

                    // Dua vao UI
                    uiCanvas.Children.Add(button);
                    Canvas.SetLeft(button, LeftOffset + j * (ButtonWidth + Padding));
                    Canvas.SetTop(button, TopOffset + i * (ButtonHeight + Padding));
                }
                
            }
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var (i, j) = button.Tag as Tuple<int, int>;

            if (_a[i,j] == 0 && isXTurn)
            {
                
                labelStatus.Foreground = Brushes.Red;
                labelStatus.Content = "O Turn";
            }
            else
            {
                
                labelStatus.Foreground = Brushes.Blue;
                labelStatus.Content = "X Turn";
            }
            

            //MessageBox.Show($"Click on button #{i} - {j}");

            if (_a[i, j] == 0)
            {
                countUsedSquared++;
                
                if (isXTurn)
                {
                    button.Foreground = Brushes.Blue;
                    button.Content = "X"; // UI

                    _a[i, j] = 1; // Model / Entity / Data transfer

                }
                else
                {
                    button.Foreground = Brushes.Red;
                    button.Content = "O"; // UI
                    _a[i, j] = 2;
                }


                var gameOver = checkWin(_a, i, j);

                if (gameOver)
                {
                    if (isXTurn)
                    {
                        MessageBox.Show("X won!", "Game Over!");
                    }
                    else
                    {
                        MessageBox.Show("O won!", "Game Over!");
                    }
                }
                else
                {
                    if (countUsedSquared == totalSquare)
                    {
                        MessageBox.Show("Draw!", "Game Over!");
                        return;
                    }
                    isXTurn = !isXTurn;
                }
             // Model / State
            }
        }

        private bool checkWin(int[,] a, int i, int j)
        {
            var gameOver = false; // false - draw, true - somebody won

            gameOver = checkWinOrientation(ORIENT_HORIZONTAL, new Tuple<int, int>(i, j),
                new Tuple<int, int>(-1, -1), new Tuple<int, int>(Rows, Cols));
            if (gameOver == true)
            {
                return true;
            }

            gameOver = checkWinOrientation(ORIENT_VERTICAL, new Tuple<int, int>(i, j),
                new Tuple<int, int>(-1, -1), new Tuple<int, int>(Rows, Cols));

            if (gameOver == true)
            {
                return true;
            }

            gameOver = checkWinOrientation(ORIENT_SW_NE, new Tuple<int, int>(i, j),
               new Tuple<int, int>(Rows, -1), new Tuple<int, int>(-1, Cols));

            if (gameOver == true)
            {
                return true;
            }

            gameOver = checkWinOrientation(ORIENT_NW_SE, new Tuple<int, int>(i, j),
               new Tuple<int, int>(-1, -1), new Tuple<int, int>(Rows, Cols));

            if (gameOver == true)
            {
                return true;
            }

            return gameOver;
        }

        private bool checkWinOrientation(List<Tuple<int, int>> orientation, Tuple<int, int> curPos,
            Tuple<int, int> lowerBound, Tuple<int, int> upperBound)
        {
            int count = 1;
            var (i, j) = curPos;
            int curI, curJ;
            int di, dj;

            (curI, curJ) = (i, j);
            (di, dj) = orientation[0];

            while (curI + di != lowerBound.Item1 &&
                curJ + dj != lowerBound.Item2)
            {
                curI += di;
                curJ += dj;
                if (_a[i, j] == _a[curI, curJ])
                {
                    count++;
                }
                else
                    break;
            }

            (curI, curJ) = (i, j);
            (di, dj) = orientation[1];

            while (curI + di != upperBound.Item1 &&
                curJ + dj != upperBound.Item2)
            {
                curI += di;
                curJ += dj;
                if (_a[i, j] == _a[curI, curJ])
                {
                    count++;
                }
                else
                    break;
            }

            return (count >= WinCondition);
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            //StreamReader reader = new StreamReader(filename);
            //reader.Read();
            string filename;
            var screen = new SaveFileDialog();
            if (screen.ShowDialog() == true)
            {
                filename = screen.FileName;


                var writer = new StreamWriter(filename);
                // Dong dau tien la luot di hien tai
                writer.WriteLine(isXTurn ? "X" : "O");

                // Theo sau la ma tran bieu dien game
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        writer.Write($"{_a[i, j]}");
                        if (j < Cols - 1)
                        {
                            writer.Write(" ");
                        }
                    }
                    writer.WriteLine("");
                }

                writer.Close();
                MessageBox.Show("Game is saved!", "Saving");
            }
            else
            {
                MessageBox.Show("Game is not saved!", "Saving");
            }
        }

        private void LoadMenu_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            if (screen.ShowDialog() == true)
            {
                var filename = screen.FileName;
                countUsedSquared = 0;
                var reader = new StreamReader(filename);
                var firstLine = reader.ReadLine();
                isXTurn = (firstLine == "X");

                for (int i = 0; i < Rows; i++)
                {
                    var tokens = reader.ReadLine().Split(
                        new string[] { " " }, StringSplitOptions.None);
                    // Model

                    for (int j = 0; j < Cols; j++)
                    {
                        _a[i, j] = int.Parse(tokens[j]);

                        if (_a[i, j] == 1)
                        {
                            _buttons[i, j].Foreground = Brushes.Blue;
                            _buttons[i, j].Content = "X";
                            countUsedSquared++;
                        }

                        if (_a[i, j] == 2)
                        {
                            _buttons[i, j].Foreground = Brushes.Red;
                            _buttons[i, j].Content = "O";
                            countUsedSquared++;
                        }

                        if (_a[i, j] == 0)
                        {
                            _buttons[i, j].Content = "";
                        }
                    }
                }
                MessageBox.Show("Game is loaded");
            }
            else
            {
                MessageBox.Show("Game is not loaded!", "Loading");
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            isXTurn = true;
            countUsedSquared = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    _a[i, j] = 0;
                    _buttons[i, j].Content = "";
                }
            }
        }
    }
}
