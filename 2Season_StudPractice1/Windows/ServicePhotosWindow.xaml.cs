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
    /// Логика взаимодействия для ServicePhotosWindow.xaml
    /// </summary>
    public partial class ServicePhotosWindow : Window
    {
        public ServicePhotosWindow(int service_id)
        {
            InitializeComponent();
            ServicePhotoFrame.Content = new ServicePhotosPage(service_id);
        }
    }
}
