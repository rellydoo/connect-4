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
using System.Windows.Shapes;

namespace connect_4
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private MainWindow _gameWindow;

        public Window1(MainWindow gameWindow )
        {
            
            InitializeComponent();
            _gameWindow = gameWindow;
        }

        private void GameModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(GameModeComboBox.Text == "Player vs Computer")
            {
                _gameWindow.AIon = true;
            }
            else
            {
                _gameWindow.AIon = false;
            }
            _gameWindow.Show();
            this.Hide();
        }
    }
}
