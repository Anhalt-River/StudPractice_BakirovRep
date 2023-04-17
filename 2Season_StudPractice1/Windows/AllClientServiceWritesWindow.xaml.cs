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
using _2Season_StudPractice1.Pages;

namespace _2Season_StudPractice1.Windows
{
    /// <summary>
    /// Логика взаимодействия для AllClientServiceWritesWindow.xaml
    /// </summary>
    public partial class AllClientServiceWritesWindow : Window
    {
        public AllClientServiceWritesWindow()
        {
            InitializeComponent();
            AllWritesFrame.Content = new ServiceClientWritesPage();

            //Обновление может быть убрано, пока не будет проведена проверка. Дабы ресурсы лишний раз не тратились на обновление целой страницы
            Update();
        }

        public async void Update()
        {
            while (true)
            {
                await updateAsync();
            }
        }

        private async Task updateAsync()
        {
            Random random = new Random();
            await Task.Delay(30000);
            int result = random.Next(0, 100); 
            AllWritesFrame.Content = new ServiceClientWritesPage();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.GlobalData_isEditAllWritesOpen = false;
        }
    }
}
