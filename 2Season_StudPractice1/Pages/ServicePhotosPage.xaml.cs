using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _2Season_StudPractice1.AdoApp;
using _2Season_StudPractice1.Materials.ConstTempMaterials;
using Microsoft.Win32;

namespace _2Season_StudPractice1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ServicePhotosPage.xaml
    /// </summary>
    public partial class ServicePhotosPage : Page
    {
        private int _serviceId = -1;
        private List<ServicePhotoConstructor> servicePhotos = new List<ServicePhotoConstructor>();

        private int taked_photoId = -1;
        public ServicePhotosPage(int taked_ServiceId)
        {
            _serviceId = taked_ServiceId;
            InitializeComponent();
            SearchServicePhotos();
            UpdateList();
        }

        private void SearchServicePhotos()
        {
            var photos = App.Connection.ServicePhoto.Where(x=> x.ServiceID == _serviceId).ToList();
            foreach(var row_photo in photos)
            {
                ServicePhotoConstructor new_photo = new ServicePhotoConstructor();
                new_photo.Id = row_photo.ID;
                new_photo.ServiceId = _serviceId;

                MemoryStream new_stream = new MemoryStream(row_photo.Image);
                BitmapImage new_bitmap = new BitmapImage();
                new_bitmap.BeginInit();
                new_bitmap.StreamSource = new_stream;
                new_bitmap.EndInit();
                new_photo.ServiceImage = new_bitmap;

                servicePhotos.Add(new_photo);
            }
        }


        private void DeletePhotoBut_Click(object sender, RoutedEventArgs e)
        {
            if (taked_photoId != -1)
            {
                var search_photo = servicePhotos.Find(x => x.Id == taked_photoId);
                servicePhotos.Remove(search_photo);
                DeletePhotoBut.Visibility = Visibility.Hidden;
                UpdateList();

                var search_photo_inBase = App.Connection.ServicePhoto.Where(x=> x.ID == taked_photoId).FirstOrDefault();
                if (search_photo_inBase != null)
                {
                    App.Connection.ServicePhoto.Remove(search_photo_inBase);
                    App.Connection.SaveChanges();
                }
            }
        }

        private void AddPhotoBut_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog new_dialog = new OpenFileDialog();
            new_dialog.Multiselect = false; 
            new_dialog.Filter = "Image files: (*.BMP, *.JPG, *.GIF, *.TIF, *.PNG)|*.bmp; *.jpg; *.gif; *.tif; *.png";
            if (new_dialog.ShowDialog() == true)
            {
                var fileName = new_dialog.FileName;
                var taked_file = File.ReadAllBytes(fileName);
                byte[] image_Data = taked_file;

                ServicePhoto photo_inData = new ServicePhoto()
                {
                    ServiceID = _serviceId,
                    Image = image_Data,
                };

                App.Connection.ServicePhoto.Add(photo_inData);
                App.Connection.SaveChanges();

                ServicePhotoConstructor new_photo = new ServicePhotoConstructor()
                {
                    Id = photo_inData.ID,
                    ServiceId = _serviceId,
                    ServiceImage = new BitmapImage(new Uri(fileName)),
                };

                servicePhotos.Add(new_photo);
            }
            UpdateList();
            UnselectItems();
        }

        private void UpdateList()
        {
            PhotoesListView.ItemsSource = null;
            PhotoesListView.ItemsSource = servicePhotos;
        }

        private void UnselectItems()
        {
            PhotoesListView.SelectedIndex = -1;
            taked_photoId = -1;
            DeletePhotoBut.Visibility = Visibility.Hidden;
        }

        private void PhotoesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PhotoesListView.SelectedItem != null)
            {
                var taked_constructor = PhotoesListView.SelectedItem as ServicePhotoConstructor;
                if (taked_photoId != -1 && taked_photoId == taked_constructor.Id)
                {
                    taked_photoId = -1;
                    DeletePhotoBut.Visibility = Visibility.Hidden;
                }
                else if (taked_photoId != taked_constructor.Id)
                {
                    taked_photoId = taked_constructor.Id;
                    DeletePhotoBut.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
