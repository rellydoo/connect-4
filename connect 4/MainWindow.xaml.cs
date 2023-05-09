using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

namespace connect_4
{
    public partial class MainWindow : Window
    {


        private int curentplayer = 0;
        private Random random = new Random();
        private Window1 menuWindow;
        public bool AIon { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            menuWindow = new Window1(this);
            menuWindow.Show();
            this.Hide();
            curentplayer = random.Next(0, 2);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {

            // Get the selected game mode
            //string gameMode = ((ComboBoxItem)GameModeComboBox.SelectedItem).Content.ToString();

            // Create a new game window
            MainWindow playWindow = new MainWindow();

            // Show the game window and hide the start menu window
            playWindow.Show();
            this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (AIon == false)
            {
                twoPlayerClick(sender, e);
            }
            else
            {
                onePlayerClick(sender, e);
            }

        }

        private void twoPlayerClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b.Content != "")
            {
                return;
            }

            bool valid = false;

            int currentRow = Grid.GetRow(b);
            int currentColumn = Grid.GetColumn(b);

            if (currentRow == 5)
                valid = true;

            // Check if place below current place has something in it
            if (valid == false && GetButtonText(GameGrid, currentColumn, currentRow + 1) != "")
                valid = true;

            if (valid == false)
                return;


            if (curentplayer == 0)
            {
                b.Content = "Red";
                curentplayer = 1;
                ChangeButtonImageChipRed(b);
            }
            else
            {
                b.Content = "Yellow";
                curentplayer = 0;
                ChangeButtonImageChipYellow(b);
            }
            Players winner = checkforwinner();

            if (winner == Players.None)
            {

            }
            else
            {
                var endScreen = new GameOver(winner.ToString());
                endScreen.ShowDialog();

                ClearGame();
                menuWindow.Show();
                this.Hide();
                //GameOver = false;
                //ResetGame();
            }
        }

        private void onePlayerClick(object sender, RoutedEventArgs e)
        {
            // Human player things
            Button b = sender as Button;
            if (b.Content != "")
            {
                return;
            }

            bool valid = false;

            int currentRow = Grid.GetRow(b);
            int currentColumn = Grid.GetColumn(b);

            if (currentRow == 5)
                valid = true;

            // Check if place below current place has something in it
            if (valid == false && GetButtonText(GameGrid, currentColumn, currentRow + 1) != "")
                valid = true;

            if (valid == false)
                return;


            if (curentplayer == 0)
            {
                b.Content = "Red";
                curentplayer = 1;
                ChangeButtonImageChipRed(b);
            }
            else
            {
                b.Content = "Yellow";
                curentplayer = 0;
                ChangeButtonImageChipYellow(b);
            }

            Players winner = checkforwinner();

            if (winner == Players.None)
            {

            }
            else
            {
                ShowGameOver(winner.ToString());
                return;
            }
            // Human player done

            // Time for AI to move
            Random random = new Random();
            int column = random.Next(0, 7);
            int row = GetNextAvailableRow(column);

            while (row == -1)
            {
                column = random.Next(0, 7);
                row = GetNextAvailableRow(column);
            }

            Button button = GetButton(GameGrid, column, row);

            if (curentplayer == 0)
            {
                button.Content = "Red";
                curentplayer = 1;
                ChangeButtonImageChipRed(button);
            }
            if (curentplayer == 1)
            {
                button.Content = "Yellow";
                ChangeButtonImageChipYellow(button);
                curentplayer = 0;
            }

            winner = checkforwinner();

            if (winner != Players.None)
            {
                ShowGameOver(winner.ToString());
                return;
            }
        }

        private void ShowGameOver(string winner)
        {
            var endScreen = new GameOver(winner);
            endScreen.ShowDialog();

            ClearGame();
            menuWindow.Show();
            this.Hide();

        }

        public static Button GetButton(Grid g, int col, int row)
        {
            Button b = g.Children.OfType<Button>()
                .Cast<Button>()
                .First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

            return b;
        }

        public static string GetButtonText (Grid g, int col, int row)
        {
            Button b = GetButton(g, col, row);

            string output = "";
            if (b.Content != null && b.Content.GetType() == typeof(Image))
            {
                Image i = b.Content as Image;
                output = i.Source.ToString();
                if (output.Contains("Red"))
                    output = "Red";
                if (output.Contains("Yellow"))
                    output = "Yellow";
                    
            }

            return output;
        }


        Players checkforwinner()
        {
            //check for horizantal wins
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if  (GetButtonText(GameGrid, col, row) == "Red" && GetButtonText(GameGrid, col + 1, row) == "Red" &&
                        GetButtonText(GameGrid, col + 2, row) == "Red" && GetButtonText(GameGrid, col + 3, row) == "Red")
                    {
                        return Players.Red;
                    }

                    if (GetButtonText(GameGrid, col, row) == "Yellow" && GetButtonText(GameGrid, col + 1, row) == "Yellow" &&
                        GetButtonText(GameGrid, col + 2, row) == "Yellow" && GetButtonText(GameGrid, col + 3, row) == "Yellow")
                    {
                        return Players.Yellow;
                    }
                }

            }

            //check for vertical wins.
            
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (GetButtonText(GameGrid, col, row) == "Red" && GetButtonText(GameGrid, col , row + 1) == "Red" &&
                        GetButtonText(GameGrid, col , row + 2) == "Red" && GetButtonText(GameGrid, col , row + 3) == "Red")
                    {
                        return Players.Red;
                    }

                    if (GetButtonText(GameGrid, col, row) == "Yellow" && GetButtonText(GameGrid, col , row + 1) == "Yellow" &&
                        GetButtonText(GameGrid, col , row + 2) == "Yellow" && GetButtonText(GameGrid, col, row + 3) == "Yellow")
                    {
                        return Players.Yellow;
                    }
                }

            }

            //check for diagonal wins (top left to bottom right
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (GetButtonText(GameGrid, col, row) == "Red" && GetButtonText(GameGrid, col + 1, row + 1) == "Red" &&
                        GetButtonText(GameGrid, col + 2, row + 2) == "Red" && GetButtonText(GameGrid, col + 3, row + 3) == "Red")
                    {
                        return Players.Red;
                    }

                    if (GetButtonText(GameGrid, col, row) == "Yellow" && GetButtonText(GameGrid, col + 1, row + 1) == "Yellow" &&
                        GetButtonText(GameGrid, col + 2, row + 2) == "Yellow" && GetButtonText(GameGrid, col + 3, row + 3) == "Yellow")
                    {
                        return Players.Yellow;
                    }
                }

            }

            //check for diagonal wins (top right to bottom left.
            for (int row = 0; row < 3; row++)
            {
                for (int col = 3; col <7; col++)
                {


                    if (GetButtonText(GameGrid, col, row) == "Red" && GetButtonText(GameGrid, col - 1, row + 1) == "Red" &&
                        GetButtonText(GameGrid, col - 2, row + 2) == "Red" && GetButtonText(GameGrid, col - 3, row + 3) == "Red")
                    {
                        return Players.Red;
                    }

                    if (GetButtonText(GameGrid, col, row) == "Yellow" && GetButtonText(GameGrid, col - 1, row + 1) == "Yellow" &&
                        GetButtonText(GameGrid, col - 2, row + 2) == "Yellow" && GetButtonText(GameGrid, col - 3, row + 3) == "Yellow")
                    {
                        return Players.Yellow;
                    }
                }

            }
            
            return Players.None;
        }



        private void ChangeButtonImageChipRed(Button myButton)
        {
            Uri uri = new Uri("pack://application:,,,/Images/ChipRed.png");
            BitmapImage bitmap = new BitmapImage(uri);
            Image image = new Image();
            image.Source = bitmap;
            myButton.Content = image;
        }


        private void ChangeButtonImageChipYellow(Button myButton)
        {
            Uri uri = new Uri("pack://application:,,,/Images/ChipYellow.png");
            BitmapImage bitmap = new BitmapImage(uri);
            Image image = new Image();
            image.Source = bitmap;
            myButton.Content = image;
        }

        private void ChangeButtonImageEmpty(Button myButton)
        {
            Uri uri = new Uri("pack://application:,,,/Images/Empty.png");
            BitmapImage bitmap = new BitmapImage(uri);
            Image image = new Image();
            image.Source = bitmap;
            myButton.Content = image;

        }

      
      private void ClearGame()
      {
            foreach (Button b in GameGrid.Children.OfType<Button>())
            {
                //ChangeButtonImageEmpty(b);
                b.Content = "";
            }
      }

        private void MakeComputerMove()
        {
            Random random = new Random();
            int column = random.Next(0, 7);
            int row = GetNextAvailableRow(column);
            Button button = GetButton(GameGrid, column, row);
            if (row != -1)
            {

                button.Content = "Yellow";
                ChangeButtonImageChipYellow(button);
                curentplayer = 0;
            }
            if (curentplayer == 0)
            {
                button.Content = "Red";
                curentplayer = 1;
                ChangeButtonImageChipRed(button);
            }
            else
            {
                MakeComputerMove();
            }
        }
        private int GetNextAvailableRow(int c)
        {
            for (int r = 5; r >= 0; r--)
            {
                if (GetButtonText(GameGrid, c, r) == "")
                    return r;
            }
            return -1; // column is full
        }

      
            





















        }

    enum Players
    {
        None = 0,
        Red,
        Yellow,
    }



    







}





