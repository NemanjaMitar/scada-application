using DataConcentrator;
using DataConcentrator.Model;
using System.Linq;
using System.Windows;

namespace ScadaGUI
{
    public partial class DetailsWindow : Window
    {
        private readonly AnalogInput _analogInput;

        public DetailsWindow(AnalogInput analogInput)
        {
            InitializeComponent();
            _analogInput = analogInput;
            DataContext = _analogInput;
            RefreshAlarms();
        }

        private void RefreshAlarms()
        {
            var alarms = ContextClass.Instance.Alarms.Where(a => a.TagName == _analogInput.Name).ToList();
            AlarmsDataGrid.ItemsSource = alarms;
        }

        private void AcknowledgeButton_Click(object sender, RoutedEventArgs e)
        {
            var alarms = ContextClass.Instance.Alarms.Where(a => a.TagName == _analogInput.Name && a.State == EAlarmState.Active).ToList();
            foreach (var alarm in alarms)
            {
                alarm.State = EAlarmState.Acknowledged;
            }
            ContextClass.Instance.SaveChanges();

            _analogInput.AlarmColor = "Yellow";
            Logger.Log($"Alarmi za tag '{_analogInput.Name}' su acknowledge-ovani.");
            RefreshAlarms();
        }

        private void RemoveAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmsDataGrid.SelectedItem is Alarm alarm)
            {
                ContextClass.Instance.Alarms.Remove(alarm);
                ContextClass.Instance.SaveChanges();
                Logger.Log($"Alarm ID {alarm.Id} za tag '{_analogInput.Name}' je obrisan.");
                RefreshAlarms();
            }
        }
    }
}
