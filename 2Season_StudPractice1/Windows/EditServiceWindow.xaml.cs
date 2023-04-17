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
using System.ComponentModel;
using _2Season_StudPractice1.AdoApp;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Data.SqlTypes;
using System.Windows.Navigation;
using _2Season_StudPractice1.Pages;

namespace _2Season_StudPractice1.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditServiceWindow.xaml
    /// </summary>
    public partial class EditServiceWindow : Window
    {
        private bool isCreating = false;
        private int taked_ServiceId = -1;
        private byte[] image_data = null;
        public EditServiceWindow(int id_openedService) //Открытие окна редактирования услуги
        {
            InitializeComponent();
            ServiceFabric(id_openedService);
            ButNamesRefresh();
            taked_ServiceId = id_openedService;
            _serviceControl_isChanged = false;
        }

        public EditServiceWindow() //Открытие окна новой услуги
        {
            isCreating = true;
            InitializeComponent();
            Service new_service = new Service()
            {
                Title = "",
                Cost = 0,   
                DurationInSeconds = 0,
            };

            App.Connection.Service.Add(new_service);
            IdServicePanel.Visibility = Visibility.Hidden;
            //ServiceIdBox.Text = new_service.ID.ToString();
            ButNamesRefresh();

            _serviceControl_Cost = false;
        }

        private void ButNamesRefresh()
        {
            if (isCreating) //Создание новой услуги
            {
                WindowContext.Text = "Создание услуги";
                SaveCardBut.Content = "Создать услугу";
            }
            else //Редактирование уже созданной
            {
                WindowContext.Text = "Редактирование услуги";
                SaveCardBut.Content = "Сохранить изменения";
            }
        }

        private void ServiceFabric(int id_openedService)
        {
            ServiceIdBox.IsReadOnly = true;

            var search_service = App.Connection.Service.Where(x=> x.ID == id_openedService).FirstOrDefault();
            ServiceIdBox.Text = search_service.ID.ToString();
            ServiceTitleBox.Text = search_service.Title;
            ServiceDurationBox.Text = search_service.DurationInSeconds.ToString();
            ServiceCostBox.Text = search_service.Cost.ToString();
                             
            /*//Конструктор для создания картинки
            var raw_path = search_service.MainImagePath.Split('\\');
            var first_word = raw_path[0].Split();
            string new_path = $"{first_word[1]} {first_word[2]}/{raw_path[1]}";
            TakedImage.Source = new BitmapImage(new Uri($@"/Materials/{new_path}", UriKind.Relative));*/

            LogoSearcher.Tag = search_service;
            if (search_service.Description != null)
            {
                ServiceDescriptionBox.Text = search_service.Description;
            }
            if (search_service.Discount != null)
            {
                ServiceDiscountBox.Text = search_service.Discount.ToString();
            }
            if (search_service.Image != null)
            {
                MemoryStream memoryStream = new MemoryStream(search_service.Image);
                BitmapImage new_Bitmap = new BitmapImage();
                new_Bitmap.BeginInit();
                new_Bitmap.StreamSource = memoryStream;
                new_Bitmap.EndInit();
                TakedImage.Source = new_Bitmap;
            }
            else
            {
                TakedImage.Source = new BitmapImage(new Uri("pack://application:,,,/Materials/school_logo.png"));
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            (App.Current.MainWindow as MainWindow).MainWindow_Frame.Content = new Pages.ListOfServicesPage(true);
        }

        private bool _serviceControl_General = true;
        private bool _serviceControl_isChanged = false;
        private void SaveCardBut_Click(object sender, RoutedEventArgs e)
        {
            if(!_serviceControl_Cost)
            {
                MessageBox.Show("Стоимость не нормализована. Используйте только цифры в поле стоимости", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _serviceControl_General = false;
            }

            if (!_serviceControl_Duration)
            {
                MessageBox.Show("Длительность не задана. Используйте для того целочисленные значения, меньше 14400 секунд", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _serviceControl_General = false;
            }

            if (!_serviceControl_Title)
            {
                MessageBox.Show("Имя не задано", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _serviceControl_General = false;
            }

            if(_serviceControl_isChanged)
            {
                if (!_serviceControl_General)
                {
                    return;
                }


                if (isCreating)
                {
                    Service new_service = new Service()
                    {
                        Title = ServiceTitleBox.Text,
                        Cost = Convert.ToDecimal(ServiceCostBox.Text),
                        DurationInSeconds = Convert.ToInt32(ServiceDurationBox.Text)
                    };

                    if (_serviceChange_Description)
                    {
                        new_service.Description = ServiceDescriptionBox.Text;
                    }

                    if (_serviceChange_Discount)
                    {
                        new_service.Discount = Convert.ToInt32(ServiceDiscountBox.Text);
                    }

                    if (_serviceChange_Image)
                    {
                        new_service.Image = image_data;
                    }

                    MessageBox.Show("Услуга создана", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    App.Connection.Service.Add(new_service);
                    App.Connection.SaveChanges();

                    isCreating = false;
                    ButNamesRefresh();
                }
                else
                {
                    var search_service = App.Connection.Service.Where(x=> x.ID == taked_ServiceId).FirstOrDefault();
                    if(_serviceChange_Title)
                    {
                        search_service.Title = ServiceTitleBox.Text;
                    }

                    if (_serviceChange_Cost)
                    {
                        search_service.Cost = Convert.ToDecimal(ServiceCostBox.Text);
                    }

                    if (_serviceChange_Duration)
                    {
                        search_service.DurationInSeconds = Convert.ToInt32(ServiceDurationBox.Text);
                    }

                    if (_serviceChange_Description)
                    {
                        search_service.Description = ServiceDescriptionBox.Text;
                    }

                    if (_serviceChange_Discount)
                    {
                        search_service.Discount = Convert.ToInt32(ServiceDiscountBox.Text);
                    }

                    if (_serviceChange_Image)
                    {
                        search_service.Image = image_data;
                    }

                    MessageBox.Show("Изменения сохранены", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                    App.Connection.SaveChanges();
                }
                RemoveChanges();
            }
            else
            {
                MessageBox.Show("Сохранения не произошло: вы не произвели изменений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void RemoveChanges()
        {
            _serviceControl_General = true;
            _serviceControl_Cost = true;
            _serviceControl_Duration = true;
            _serviceControl_Title = true;

            _serviceControl_isChanged = false;

            _serviceChange_Description = false;
            _serviceChange_Cost = false;
            _serviceChange_Discount = false;
            _serviceChange_Duration = false;
            _serviceChange_Image = false;
            _serviceChange_Title = false;

        }

        private void DeleteCardBut_Click(object sender, RoutedEventArgs e)
        {
            if(!isCreating)
            {
                var search_service = App.Connection.Service.Where(x=> x.ID == taked_ServiceId).FirstOrDefault();
                var search_writes = App.Connection.ClientService.Where(x=> x.ServiceID == taked_ServiceId).FirstOrDefault();
                if (search_writes != null)
                {
                    var search_servicePhoto = App.Connection.ServicePhoto.Where(x => x.ServiceID == taked_ServiceId).ToList();
                    foreach (var taked_photo in search_servicePhoto)
                    {
                        App.Connection.ServicePhoto.Remove(taked_photo);
                    }
                    App.Connection.Service.Remove(search_service);

                    App.Connection.SaveChanges();
                }
                MessageBox.Show("Удаление невозможно по причине наличия записей клиентов на эту запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Удаление невозможно по причине отсутствия создаваемой услуги в базе данных", "Упс!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool _serviceChange_Duration = false;
        private bool _serviceControl_Duration = false;
        private void ServiceDurationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ServiceDurationBox.Text != "")
            {
                CalculDuration();
                _serviceChange_Duration = true;
                _serviceControl_isChanged = true;  //Изменение произошло
            }
        }

        private void CalculDuration()
        {
            try
            {
                int convert_value = Convert.ToInt32(ServiceDurationBox.Text);

                if (convert_value > 14400)
                {
                    ServiceDurationBox.Text = "14400";
                    MessageBox.Show("Вы ввели значение суммарно превышающее 4-е часа(14400 секунд)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _serviceControl_Duration = true;
            }
            catch (FormatException)
            {
                ServiceDurationBox.Text = "";
                MessageBox.Show("Непринятый формат, используйте цифры", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool _serviceChange_Title = false;
        private bool _serviceControl_Title = false;
        private void ServiceTitleBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (ServiceTitleBox.Text != "" && !isCreating)
            {
                ServiceNameSearch();
                _serviceChange_Title = true;
                _serviceControl_Title = true;
            }
            else if(ServiceTitleBox.Text != "")
            {
                CreatingServiceNameSearch();
                _serviceChange_Title = true;
                _serviceControl_Title = true;
            }
            else
            {
                _serviceControl_Title = false;
            }
            _serviceControl_isChanged = true;  //Изменение произошло
        }

        private void ServiceNameSearch()
        {
            int taked_id = Convert.ToInt32(ServiceIdBox.Text);
            var service_search = App.Connection.Service.Where(x=> x.Title == ServiceTitleBox.Text 
            && x.ID != taked_id).FirstOrDefault();
            if(service_search != null)
            {
                MessageBox.Show("Данная услуга уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ServiceTitleBox.Text = "";
            }
        }

        private void CreatingServiceNameSearch()
        {
            var service_search = App.Connection.Service.Where(x => x.Title == ServiceTitleBox.Text).FirstOrDefault();
            if (service_search != null)
            {
                MessageBox.Show("Данная услуга уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                ServiceTitleBox.Text = "";
            }
        }

        private void NewServiceBut_Click(object sender, RoutedEventArgs e)
        {
            EditServiceWindow new_window = new EditServiceWindow();
            new_window.Show();
            this.Close();
            App.GlobalData_isEditPageOpen = true;
        }

        private bool _serviceChange_Image = false;
        private void ServiceImageBut_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Image files (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG, *.ICO, *.EMF, *.WMF)|*.bmp;*.jpg;*.gif; *.tif; *.png; *.ico; *.emf; *.wmf";
            if (openFileDialog.ShowDialog() == true)
            {
                var taked_filename = openFileDialog.FileName;
                image_data = File.ReadAllBytes(taked_filename);
                _serviceChange_Image = true;
                _serviceControl_isChanged = true;  //Изменение произошло
                TakedImage.Source = new BitmapImage(new Uri(taked_filename));

                /*//Сохранение в изображении
                var take_image = App.Connection.Service.Where(x=> x.ID == taked_ServiceId).FirstOrDefault();
                take_image.Image = File.ReadAllBytes(taked_filename);*/

                /*Taked_Image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                ClickOnImage.Visibility = Visibility.Hidden;
                isImageTaked = true;*/
            }
        }

        private bool _serviceControl_Cost = true;
        private bool _serviceChange_Cost = true;
        private void ServiceCostBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(ServiceCostBox.Text != "")
            {
                try
                {
                    decimal checker = Convert.ToDecimal(ServiceCostBox.Text);
                    _serviceControl_Cost = true;
                }
                catch (FormatException)
                {
                    _serviceControl_Cost = false;
                }
            }
            else
            {
                _serviceControl_Cost = false;
            }

            _serviceChange_Cost = true;
            _serviceControl_isChanged = true;  //Изменение произошло
        }

        private bool _serviceChange_Discount = false;
        private void ServiceDiscountBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(ServiceDiscountBox.Text != "")
            {
                try
                {
                    int checker = Convert.ToInt32(ServiceDiscountBox.Text);
                    if (checker > 100)
                    {
                        MessageBox.Show("Скидка не может быть больше ста процентов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        ServiceDiscountBox.Text = "100";
                    }
                    else if (checker < 0)
                    {
                        MessageBox.Show("Скидка не может быть меньше нуля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        ServiceDiscountBox.Text = "0";
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Для указания скидки используйте только целочисленные значения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    ServiceDiscountBox.Text = "";
                }
            }

            _serviceChange_Discount = true;
            _serviceControl_isChanged = true;  //Изменение произошло
        }

        private bool _serviceChange_Description = false;
        private void ServiceDescriptionBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _serviceChange_Description = true;
            _serviceControl_isChanged = true;  //Изменение произошло
        }

        private void DeleteImageBut_Click(object sender, RoutedEventArgs e)
        {
            if (TakedImage.Source != new BitmapImage(new Uri("pack://application:,,,/Materials/school_logo.png")))
            {
                TakedImage.Source = new BitmapImage(new Uri("pack://application:,,,/Materials/school_logo.png"));
                var search_service = App.Connection.Service.Where(x => x.ID == taked_ServiceId).FirstOrDefault();
                search_service.Image = null;
                App.Connection.SaveChanges();
            }
            else
            {
                MessageBox.Show("Не удалось убрать изображения, которого нет!", "Упс!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void AdditionalBut_Click(object sender, RoutedEventArgs e)
        {
            ServicePhotosWindow new_window = new ServicePhotosWindow(taked_ServiceId);
            new_window.Owner = this;
            new_window.Show();
        }
    }
}
