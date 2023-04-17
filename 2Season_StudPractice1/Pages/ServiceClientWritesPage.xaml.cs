using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using _2Season_StudPractice1.Materials;
using _2Season_StudPractice1.Materials.ConstTempMaterials;

namespace _2Season_StudPractice1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ServiceClientWritesPage.xaml
    /// </summary>
    public partial class ServiceClientWritesPage : Page
    {
        private List<ServiceWriteConstructor> taked_writes = new List<ServiceWriteConstructor>();
        private List<ServiceWriteConstructor> sorted_writes = new List<ServiceWriteConstructor>();
        public ServiceClientWritesPage()
        {
            InitializeComponent();
            ListFabric();
            ListSorting();

            WritesList.ItemsSource = sorted_writes;
        }

        private void ListFabric()
        {
            var today_date = DateTime.Now;
            var tomorrow_date = today_date;
            tomorrow_date = tomorrow_date.AddDays(1);

            var search_allWrites = App.Connection.ClientService.Where(x => x.StartTime > today_date && x.StartTime < tomorrow_date).ToList();
            foreach (var row_write in search_allWrites)
            {
                ServiceWriteConstructor new_write = new ServiceWriteConstructor();
                var search_service = App.Connection.Service.Where(x => x.ID == row_write.ServiceID).FirstOrDefault();
                var search_client = App.Connection.Client.Where(x => x.ID == row_write.ClientID).FirstOrDefault();
                new_write.ServiceName = search_service.Title;
                new_write.ClientFullName = $"{search_client.LastName} {search_client.FirstName[0]}. {search_client.Patronymic[0]}.";
                new_write.ClientEmail = search_client.Email;
                new_write.ClientPhone = search_client.Phone;
                new_write.WriteDateTime = row_write.StartTime.ToString();
                new_write.WriteDateTime_forSorting = row_write.StartTime;

                var remaining_time = row_write.StartTime - today_date;
                int remaining_timeInSeconds = Convert.ToInt32(remaining_time.TotalSeconds);

                int remaining_days = remaining_timeInSeconds / 86400;
                int remaining_hours = (remaining_timeInSeconds % 86400) / 3600;
                int remaining_minuts = ((remaining_timeInSeconds % 86400) % 3600) / 60;

                string control_remainingTime = "";
                if (remaining_days != 0)
                {
                    control_remainingTime += $"{remaining_days} день ";
                }
                if (remaining_hours != 0)
                {
                    control_remainingTime += $"{remaining_hours} час ";
                }
                if (remaining_minuts != 0)
                {
                    control_remainingTime += $"{remaining_minuts} минут ";
                }

                new_write.WriteDateTimeRemains = control_remainingTime;

                //Добавление остатка времени
                if (remaining_hours < 1)
                {
                    object UnformalStyle = Application.Current.FindResource("RemainingTimeUnformal");
                    new_write.RemainTimeStyle = (Style)UnformalStyle;
                }
                else
                {
                    object FormalStyle = Application.Current.FindResource("RemainingTimeFormal");
                    new_write.RemainTimeStyle = (Style)FormalStyle;
                }

                taked_writes.Add(new_write);
            }
        }

        private void ListSorting()
        {
            bool isSorted = false;
            while(!isSorted)
            {
                isSorted = true;
                for (int i = 1; i < taked_writes.Count; i++)
                {
                    if (taked_writes[i].WriteDateTime_forSorting < taked_writes[i - 1].WriteDateTime_forSorting)
                    {
                        ServiceWriteConstructor temp_constr = taked_writes[i];
                        taked_writes[i] = taked_writes[i - 1];
                        taked_writes[i - 1] = temp_constr;
                        isSorted = false;
                    }
                }
            }

            sorted_writes = taked_writes;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            WritesList.Height = this.Height - 100;
        }
    }
}
