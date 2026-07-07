using DataConcentrator;
using DataConcentrator.Model;
using ScadaGUI.Services;
using ScadaGUI.Utils;
using System;
using System.Linq;
using System.Windows;

namespace ScadaGUI
{
    public partial class AlarmsWindow : Window
    {
        public AlarmsWindow()
        {
            InitializeComponent();
            ApplyLanguage();
            RefreshGrid();
            UpdateSound();
        }

        private void ApplyLanguage()
        {
            Title = LocalizationManager.Get("Alarms_Title");
            LblTitle.Text = LocalizationManager.Get("Alarms_Title");

            ColMessage.Header = LocalizationManager.Get("Alarms_ColMessage");
            ColTag.Header = LocalizationManager.Get("Alarms_ColTag");
            ColLimit.Header = LocalizationManager.Get("Alarms_ColLimit");
            ColDirection.Header = LocalizationManager.Get("Alarms_ColDirection");
            ColState.Header = LocalizationManager.Get("Alarms_ColState");

            BtnActivate.Content = LocalizationManager.Get("Alarms_Activate");
            BtnAcknowledge.Content = LocalizationManager.Get("Alarms_Acknowledge");
            BtnClose.Content = LocalizationManager.Get("Alarms_Close");

            BtnActivate.ToolTip = LocalizationManager.Get("Alarms_TtActivate");
            BtnAcknowledge.ToolTip = LocalizationManager.Get("Alarms_TtAcknowledge");
        }

        private void RefreshGrid()
        {
            AlarmsGrid.ItemsSource = null;
            AlarmsGrid.ItemsSource = ContextClass.Instance.Alarms.ToList();
        }

        // Zvuk svira dok postoji makar jedan aktivan (neacknowledge-ovan) alarm.
        private void UpdateSound()
        {
            bool imaAktivnih = ContextClass.Instance.Alarms.Any(a => a.State == EAlarmState.Active);
            if (imaAktivnih)
                AlarmSoundManager.PlayLoop();
            else
                AlarmSoundManager.Stop();
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(AlarmsGrid.SelectedItem is Alarm alarm))
            {
                MessageBox.Show("Izaberite alarm koji želite aktivirati.", "Alarmi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            alarm.State = EAlarmState.Active;

            // Upis u tabelu aktiviranih alarma (kao što DataConcentrator radi pri realnom alarmu)
            ContextClass.Instance.ActivatedAlarms.Add(new ActivatedAlarm
            {
                AlarmId = alarm.Id,
                TagName = alarm.TagName,
                Message = alarm.Message,
                Timestamp = DateTime.Now
            });
            ContextClass.Instance.SaveChanges();

            Logger.Log($"Alarm '{alarm.Message}' (tag '{alarm.TagName}') je aktiviran.");

            RefreshGrid();
            UpdateSound();
        }

        private void AcknowledgeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(AlarmsGrid.SelectedItem is Alarm alarm))
            {
                MessageBox.Show("Izaberite alarm koji želite prihvatiti.", "Alarmi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (alarm.State != EAlarmState.Active)
            {
                MessageBox.Show("Prihvatiti se može samo aktivan alarm.", "Alarmi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            alarm.State = EAlarmState.Acknowledged;
            ContextClass.Instance.SaveChanges();

            Logger.Log($"Alarm '{alarm.Message}' (tag '{alarm.TagName}') je prihvaćen (acknowledge).");

            RefreshGrid();
            UpdateSound();
        }
    }
}
