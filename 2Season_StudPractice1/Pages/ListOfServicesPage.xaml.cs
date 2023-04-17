using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _2Season_StudPractice1.Materials.ConstTempMaterials;
using _2Season_StudPractice1.Windows;
using System.IO;
using System.Globalization;
using Microsoft.Win32;

namespace _2Season_StudPractice1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ListOfServicesPage.xaml
    /// </summary>
    public partial class ListOfServicesPage : Page
    {
        private List<ServiceConstructor> saved_list = new List<ServiceConstructor>();
        private List<ServiceConstructor> first_list = new List<ServiceConstructor>();


        public ListOfServicesPage(bool codePacket)
        {
            InitializeComponent();
            App.GlobalData_isCodeTaked = codePacket;
            FillServices(codePacket);
            App.GlobalData_isEditPageOpen = false;
            SortBoard.Height = 0;
            FiltrBoard.Height = 0;
            ListCounter();
            /*Style style = new Style();
            {
                Visibility = Visibility.Visible;
            };
            style.Setters.Add(new Setter(VisibilityProperty, Visibility.Visible));
            (FindResource("VisibleButtons") as Style).Setters.Add(new Setter { Property = Control.VisibilityProperty, Value = Visibility.Visible });*/

            if (!codePacket)
            {
                AllServicePanel.Visibility = Visibility.Hidden;
            }
        }

        void FillServices(bool isCodeTaked) //Заполнение услуг
        {
            var all_RowServices = App.Connection.Service.ToList();
            List<ServiceConstructor> all_Services = new List<ServiceConstructor>();

            foreach(var rowService in all_RowServices)
            {
                ServiceConstructor new_service = new ServiceConstructor();
                new_service.Id = rowService.ID;
                new_service.Title = rowService.Title;
                new_service.Cost = Convert.ToInt32(rowService.Cost.ToString().Split(',')[0]);
                var duration_cifrus = Convert.ToDouble(rowService.DurationInSeconds) / 60;
                var duration_converter = duration_cifrus.ToString().Split(',');
                if(duration_converter.Length > 1)
                {
                    var duration_after_comma = duration_converter[1].ToCharArray();
                    new_service.DurationInMinuts = duration_converter[0] + "," + duration_after_comma[0] + duration_after_comma[1];
                }
                else
                {
                    new_service.DurationInMinuts = duration_cifrus.ToString();
                }
                new_service.Discount = Convert.ToInt32(rowService.Discount);

                if (isCodeTaked)
                {
                    new_service.FeaturesAvailable = Visibility.Visible;
                }

                if(rowService.Discount != 0)
                {
                    object resource = Application.Current.FindResource("CustomDiscountColor");
                    new_service.DiscountColor = (Style)resource;

                    new_service.DiscountExist = Visibility.Visible;
                    new_service.OldCost = new_service.Cost;
                    double minus = Convert.ToDouble(new_service.Cost) * (Convert.ToDouble(new_service.Discount) / 100);
                    new_service.Cost -= Convert.ToInt32(minus);
                }
                else
                {
                    object resource = Application.Current.FindResource("WhiteDiscountColor");
                    new_service.DiscountColor = (Style)resource;
                }

                /*//Конструктор для создания картинки
                var raw_path = rowService.MainImagePath.Split('\\');
                var first_word = raw_path[0].Split();
                string new_path = $"{first_word[1]} {first_word[2]}/{raw_path[1]}";
                new_service.ImagePath = $"/Materials/{new_path}";   */             

                if(rowService.Image != null)
                {
                    MemoryStream memoryStream = new MemoryStream(rowService.Image);
                    BitmapImage new_Bitmap = new BitmapImage();
                    new_Bitmap.BeginInit();
                    new_Bitmap.StreamSource = memoryStream;
                    new_Bitmap.EndInit();
                    new_service.ServiceImage = new_Bitmap;
                }
                else
                {
                    new_service.ServiceImage = new BitmapImage(new Uri("pack://application:,,,/Materials/school_logo.png"));
                }

                all_Services.Add(new_service);
            }
           
           servicesList.ItemsSource = all_Services;


           saved_list = all_Services;
           foreach(var service in all_Services)
           {
              first_list.Add(service);
           }
        }

        private void EditServiceBut_Click(object sender, RoutedEventArgs e) //Переход в окно редактирования/создания записи
        {
            if(!App.GlobalData_isEditPageOpen)
            {
                var item = (sender as Button).DataContext as ServiceConstructor;
                EditServiceWindow editServiceWindow = new EditServiceWindow(item.Id);
                editServiceWindow.Owner = App.Current.MainWindow;
                editServiceWindow.Show();
                App.GlobalData_isEditPageOpen = true;
            }
        }

        private void SortingServices_DecreaseCost()
        {
            bool isNotSorted = false;
            while (!isNotSorted)
            {
                isNotSorted = true;
                for (int i = 1; i < saved_list.Count; i++)
                {
                    if (saved_list[i - 1].Cost < saved_list[i].Cost)
                    {
                        ServiceConstructor temp_constr = saved_list[i - 1];
                        saved_list[i - 1] = saved_list[i];
                        saved_list[i] = temp_constr;
                        isNotSorted = false;
                    }
                }
            }

            servicesList.ItemsSource = saved_list;
        }

        private void SortingServices_GrowCost()
        {
            bool isNotSorted = false;
            while (!isNotSorted)
            {
                isNotSorted = true;
                for (int i = 1; i < saved_list.Count; i++)
                {
                    if (saved_list[i - 1].Cost > saved_list[i].Cost)
                    {
                        ServiceConstructor temp_constr = saved_list[i - 1];
                        saved_list[i - 1] = saved_list[i];
                        saved_list[i] = temp_constr;
                        isNotSorted = false;
                    }
                }
            }

            servicesList.ItemsSource = saved_list;
        }

        private void SortingServices_Devolve()
        {
            saved_list.Clear();
            foreach(var service in first_list)
            {
                saved_list.Add(service);
            }

            servicesList.ItemsSource = saved_list;
        }

        private int sortController = -1;
        private void Sort_CostBut_Click(object sender, RoutedEventArgs e)
        {
            servicesList.ItemsSource = null;
            switch (sortController)
            {
                case -1: //Стирание сортировки   
                    sortController = 1;
                    Sort_CostBut.Content = "По возраст.";
                    SortingServices_GrowCost();
                    break;
                case 1:  //Сортировка по убыванию
                    sortController = 2;
                    Sort_CostBut.Content = "По убыв.";
                    SortingServices_DecreaseCost();
                    break;
                case 2: //Сортировка по возрастанию 
                    sortController = -1;
                    Sort_CostBut.Content = "Нет";
                    SortingServices_Devolve();
                    break;
            }
        }


        private bool isSortToggle;
        private void Sort_MainBut_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation doubAnime = new DoubleAnimation();
            if(!isSortToggle)
            {
                doubAnime.To = 180;
                doubAnime.Duration = TimeSpan.FromSeconds(1);
                SortBoard.BeginAnimation(Border.HeightProperty, doubAnime);
                isSortToggle = true;
            }
            else
            {
                doubAnime.To = 0;
                doubAnime.Duration = TimeSpan.FromSeconds(1);
                SortBoard.BeginAnimation(Border.HeightProperty, doubAnime);
                isSortToggle = false;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchByTitle();
            ListCounter();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SearchByTitle() //Метод поиска данных
        {
            List<ServiceConstructor> search_list = new List<ServiceConstructor>();
            foreach(var service in saved_list)
            {
                if(service.Title.Contains(SearchBox.Text))
                {
                    search_list.Add(service);
                }
            }
            servicesList.ItemsSource = search_list;
        }

        private void RemoveService(int service_id)
        {
            var search_record = App.Connection.ClientService.Where(x=> x.ServiceID == service_id).FirstOrDefault();
            if(search_record == null)
            {
                var search_service = App.Connection.Service.Where(x => x.ID == service_id).FirstOrDefault();
                var search_listItem = saved_list.Where(x=> x.Id == service_id).FirstOrDefault();
                saved_list.Remove(search_listItem);
                servicesList.ItemsSource = null;
                servicesList.ItemsSource = saved_list;

                var search_servicePhoto = App.Connection.ServicePhoto.Where(x => x.ServiceID == service_id).ToList();
                foreach (var taked_photo in search_servicePhoto)
                {
                    App.Connection.ServicePhoto.Remove(taked_photo);
                }

                App.Connection.Service.Remove(search_service);
                App.Connection.SaveChanges();

                MessageBox.Show("Запись успешно удалена!", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Вы не можете удалить данную услугу, так как с ней связаны записи клиентов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void DeleteServiceBut_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as ServiceConstructor;
            RemoveService(item.Id);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e) //Масштабирование элементов внутри
        {
            servicesList.Height = App.Current.MainWindow.Height - 150;
        }

        private int filtrController = -1;
        private void Filtr_discountBut_Click(object sender, RoutedEventArgs e)
        {
            servicesList.ItemsSource = null;
            int firstBorder = 0;
            int secondBorder = 0;
            switch (filtrController)
            {
                case -1: //Фильтрация от 0 до 5% 
                    filtrController = 1;
                    Filtr_DiscountBut.Content = "от 0 до 5%";

                    firstBorder = 0;
                    secondBorder = 5;
                    break;

                case 1: //Фильтрация от 5 до 15% 
                    filtrController = 2;
                    Filtr_DiscountBut.Content = "от 5 до 15%";

                    firstBorder = 5;
                    secondBorder = 15;
                    break;

                case 2: //Фильтрация от 15 до 30% 
                    filtrController = 3;
                    Filtr_DiscountBut.Content = "от 15 до 30%";

                    firstBorder = 15;
                    secondBorder = 30;
                    break;

                case 3: //Фильтрация от 30 до 70% 
                    filtrController = 4;
                    Filtr_DiscountBut.Content = "от 30 до 70%";

                    firstBorder = 30;
                    secondBorder = 70;
                    break;

                case 4: //Фильтрация от 70 до 100% 
                    filtrController = 5;
                    Filtr_DiscountBut.Content = "от 70 до 100%";

                    firstBorder = 70;
                    secondBorder = 101;

                    break;
                case 5: //Фильтрация невведения
                    filtrController = -1;
                    Filtr_DiscountBut.Content = "Не введен";

                    firstBorder = 0;
                    secondBorder = 100;

                    break;
            }
            FiltrationSystem(firstBorder, secondBorder);
            ListCounter();
        }

        private void FiltrationSystem(int firstBorder, int secondBorder)
        {
            List<ServiceConstructor> search_list = new List<ServiceConstructor>();
            foreach (var service in saved_list)
            {
                if (service.Discount >= firstBorder && service.Discount < secondBorder)
                {
                    search_list.Add(service);
                }             
            }
            servicesList.ItemsSource = search_list;
        }

        private bool isFiltrToggle;
        private void Filtr_MainBut_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation doubAnime = new DoubleAnimation();
            if (!isFiltrToggle)
            {
                doubAnime.To = 180;
                doubAnime.Duration = TimeSpan.FromSeconds(1);
                FiltrBoard.BeginAnimation(Border.HeightProperty, doubAnime);
                isFiltrToggle = true;
            }
            else
            {
                doubAnime.To = 0;
                doubAnime.Duration = TimeSpan.FromSeconds(1);
                FiltrBoard.BeginAnimation(Border.HeightProperty, doubAnime);
                isFiltrToggle = false;
            }
        }

        private void ListCounter() //Подсчет элементов в списках
        {
            int count = servicesList.Items.Count;
            SearchResultBlock.Text = count.ToString();
        }

        private void WriteClientBut_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button).DataContext as ServiceConstructor;
            NavigationService.Navigate(new WriteClientPage(item.Id));
        }

        private void AuthBackBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void AllServiceClientsWritesBut_Click(object sender, RoutedEventArgs e)
        {
            if (!App.GlobalData_isEditAllWritesOpen)
            {
                AllClientServiceWritesWindow allClientServiceWindow = new AllClientServiceWritesWindow();
                allClientServiceWindow.Owner = App.Current.MainWindow;
                allClientServiceWindow.Show();
                App.GlobalData_isEditAllWritesOpen = true;
            }
        }
    }
}
