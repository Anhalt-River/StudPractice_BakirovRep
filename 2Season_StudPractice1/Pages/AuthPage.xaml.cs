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

namespace _2Season_StudPractice1.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private bool isCodeActivated = false;
        public AuthPage()
        {
            InitializeComponent();
        }

        private void ActivationCodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(ActivationCodeBox.Text == "")
            {
                ActivationBut.Content = "Зайти без использования кода";
                isCodeActivated = false;
            }
            else
            {
                ActivationBut.Content = "Активировать код";
                isCodeActivated = true;
            }
        }

        private void ActivationBut_Click(object sender, RoutedEventArgs e)
        {
            if(isCodeActivated)
            {
                if(ActivationCodeBox.Text == "0000")
                {
                    NavigationService.Navigate(new ListOfServicesPage(isCodeActivated));
                }
            }
            else
            {
                NavigationService.Navigate(new ListOfServicesPage(isCodeActivated));
            }
        }
    }
}
