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
using _2Season_StudPractice1.AdoApp;
using _2Season_StudPractice1.Materials.ConstTempMaterials;

namespace _2Season_StudPractice1.Pages
{
    /// <summary>
    /// Логика взаимодействия для WriteClientPage.xaml
    /// </summary>
    public partial class WriteClientPage : Page
    {
        private bool isTimeNormal = false;
        private int service_DurationInSec = 0;
        private int taked_clientId = -1;
        private int taked_serviceId = -1;
        public WriteClientPage(int service_id)
        {
            InitializeComponent();
            FillClientList();
            FillServiceInfo(service_id);
        }

        private void FillServiceInfo(int service_id)
        {
            var taked_service = App.Connection.Service.Where(x=> x.ID == service_id).FirstOrDefault();
            taked_serviceId = taked_service.ID;
            ServiceNameBlock.Text = taked_service.Title;
            service_DurationInSec = taked_service.DurationInSeconds;

            var duration_cifrus = Convert.ToDouble(taked_service.DurationInSeconds) / 60;
            var duration_converter = duration_cifrus.ToString().Split(',');
            if (duration_converter.Length > 1)
            {
                var duration_after_comma = duration_converter[1].ToCharArray();
                ServiceMinutDurationBlock.Text = duration_converter[0] + "," + duration_after_comma[0] + duration_after_comma[1];
            }
            else
            {
                ServiceMinutDurationBlock.Text = duration_cifrus.ToString();
            }

        }

        private void FillClientList()
        {
            List<ClientConstructor> client_list = new List<ClientConstructor>();
            var row_list = App.Connection.Client.ToList();
            foreach(var row_client in row_list)
            {
                ClientConstructor new_client = new ClientConstructor();
                new_client.Id = row_client.ID;
                new_client.FirstName = row_client.FirstName;
                new_client.LastName = row_client.LastName;
                new_client.Patronymic = row_client.Patronymic;

                client_list.Add(new_client);
            }

            clientList.ItemsSource= client_list;
        }

        private void ListBackBut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void clientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clientList.SelectedItem != null)
            {
                var taked_client = clientList.SelectedItem as ClientConstructor;
                string clientFullName = $"{taked_client.FirstName} {taked_client.LastName} {taked_client.Patronymic}";
                TakedClientBlock.Text = clientFullName;
                taked_clientId = taked_client.Id;
            }
            else
            {
                TakedClientBlock.Text = "Untaked";
                taked_clientId = -1;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            clientList.Height = App.Current.MainWindow.Height - 150;
        }

        private bool isStartTimeBox_Changed = false;
        private void StartTimeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EndTimeBlock == null)
            {
                return;
            }

            if(isStartTimeBox_Changed)
            {
                switch (StartTimeBox.Text)
                {
                    case "":
                        StartTimeBox.Text = "ЧЧ:MM:СС";
                        isStartTimeBox_Changed = false;
                        break;
                    case " ":
                        StartTimeBox.Text = "ЧЧ:MM:СС";
                        isStartTimeBox_Changed = false;
                        break;
                    case "ЧЧ:ММ:СС":
                        StartTimeBox.Text = "ЧЧ:MM:СС";
                        isStartTimeBox_Changed = false;
                        break;

                }
            }
            else
            {
                isStartTimeBox_Changed = true;
            }

            bool test = TimeValidationChecker(StartTimeBox.Text);
        }

        private bool TimeValidationChecker(string taked_string)
        {
            var raw_string = taked_string.Split(':');
            if(raw_string.Length == 3)
            {
                int takedStartTime_InSecunds = 0;
                if (raw_string[0].Length == 2)
                {
                    try
                    {
                        int hours = Convert.ToInt32(raw_string[0]);
                        if (hours >= 24)
                        {
                            return TimeValidation_FalseAnswer();
                        }
                        else
                        {
                            takedStartTime_InSecunds += hours * 3600;
                        }
                    }
                    catch (FormatException)
                    {
                        return TimeValidation_FalseAnswer();
                    }
                }
                else
                {
                    return TimeValidation_FalseAnswer();
                }

                if (raw_string[1].Length == 2)
                {
                    try
                    {
                        int minuts = Convert.ToInt32(raw_string[1]);
                        if (minuts >= 60)
                        {
                            return TimeValidation_FalseAnswer();
                        }
                        else
                        {
                            takedStartTime_InSecunds += minuts * 60;
                        }
                    }
                    catch (FormatException)
                    {
                        return TimeValidation_FalseAnswer();
                    }
                }
                else
                {
                    return TimeValidation_FalseAnswer();
                }

                if (raw_string[2].Length == 2)
                {
                    try
                    {
                        int seconds = Convert.ToInt32(raw_string[2]);
                        if (seconds >= 60)
                        {
                            return TimeValidation_FalseAnswer();
                        }
                        else
                        {
                            takedStartTime_InSecunds += seconds;
                        }
                    }
                    catch (FormatException)
                    {
                        return TimeValidation_FalseAnswer();
                    }
                }
                else
                {
                    return TimeValidation_FalseAnswer();
                }

                int maxTime_InDay = 86400;
                if (maxTime_InDay - takedStartTime_InSecunds <= service_DurationInSec)
                {
                    return TimeValidation_FalseAnswer();
                }

                return TimeValidation_TrueAnswer(takedStartTime_InSecunds);

            }
            else 
            { 
                return TimeValidation_FalseAnswer(); 
            }
        }

        private bool TimeValidation_FalseAnswer()
        {
            object resource = Application.Current.FindResource("UnformatTimeTextBox");
            StartTimeBox.Style = (Style)resource;
            isTimeNormal = false;
            EndTimeBlock.Text = "";
            return false;
        }

        private bool TimeValidation_TrueAnswer(int taked_durationInSeconds)
        {
            object resource = Application.Current.FindResource("FormatTimeTextBox");
            StartTimeBox.Style = (Style)resource;
            isTimeNormal = true;
            taked_durationInSeconds += service_DurationInSec;
            int hours = taked_durationInSeconds / 3600;
            string format_hours = hours.ToString();
            if (format_hours.Length < 2)
            {
                if ((hours / 10) < 1) //Если в часах значение меньше десяти
                {
                    format_hours = "0" + format_hours;
                }
                else
                {
                    format_hours = format_hours + "0";
                }
            }

            int minuts = (taked_durationInSeconds % 3600) / 60;
            string format_minuts = minuts.ToString();
            if (format_minuts.Length < 2)
            {
                if ((minuts / 10) < 1) //Если в секундах значение меньше десяти
                {
                    format_minuts = "0" + format_minuts;
                }
                else
                {
                    format_minuts = format_minuts + "0";
                }
            }

            int seconds = (taked_durationInSeconds % 3600) % 60;
            string format_seconds = seconds.ToString();
            if (format_seconds.Length < 2)
            {
                if ((seconds / 10) < 1) //Если в секундах значение меньше десяти
                {
                    format_seconds = "0" + format_seconds;
                }
                else
                {
                    format_seconds = format_seconds + "0";
                }
            }
            EndTimeBlock.Text = $"{format_hours}:{format_minuts}:{format_seconds}";
            return true;
        }

        private void WriteBut_Click(object sender, RoutedEventArgs e)
        {
            if(taked_clientId != -1)
            {
                if (isTimeNormal && WriteCalendar.SelectedDate != null)
                {
                    string[] row_string_time = StartTimeBox.Text.ToString().Split(':');
                    string taked_hours = row_string_time[0];
                    string taked_minutes = row_string_time[1];
                    string taked_seconds = row_string_time[2];

                    DateTime dateTime_former = Convert.ToDateTime(WriteCalendar.SelectedDate);
                    dateTime_former = dateTime_former.AddHours(Convert.ToDouble(taked_hours));
                    dateTime_former = dateTime_former.AddMinutes(Convert.ToDouble(taked_minutes));
                    dateTime_former = dateTime_former.AddSeconds(Convert.ToDouble(taked_seconds));
                    var new_clientService = new ClientService
                    {
                        ClientID = taked_clientId,
                        ServiceID = taked_serviceId,
                        StartTime = dateTime_former
                    };

                    App.Connection.ClientService.Add(new_clientService);
                    App.Connection.SaveChanges();

                    MessageBox.Show("запись успешно создана!", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (!isTimeNormal)
                {
                    MessageBox.Show("Введенное время оказания услуги не нормализовано, услуга не может быть оказана клиенту", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else
                {
                    MessageBox.Show("Выберете время и укажите дату для создания записи", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
            else if (taked_clientId == -1)
            {
                MessageBox.Show("Клиент, для которого готовится запись, отсутствует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
    }
}
