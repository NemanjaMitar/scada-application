using DataConcentrator;
using DataConcentrator.Model;
using System;
using System.Linq;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace ScadaGUI
{
    public partial class AddTagWindow : Window
    {
        public Tag CreatedTag { get; private set; }
        public Alarm CreatedAlarm { get; private set; }

        public AddTagWindow()
        {
            InitializeComponent();
            Loaded += AddTagWindow_Loaded;
        }

        private void AddTagWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAnalogInputs();
        }

        private void LoadAnalogInputs()
        {
            var analogInputs = ContextClass.Instance.Tags.OfType<AnalogInput>().ToList();
            CbTagList.ItemsSource = analogInputs;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PanelAI == null) return;

            PanelAI.Visibility = Visibility.Collapsed;
            PanelAO.Visibility = Visibility.Collapsed;
            PanelDI.Visibility = Visibility.Collapsed;
            PanelDO.Visibility = Visibility.Collapsed;
            PanelAlarm.Visibility = Visibility.Collapsed;

            LblAddress.Visibility = Visibility.Visible;
            TxtAddress.Visibility = Visibility.Visible;

            if (RbAI.IsChecked == true) PanelAI.Visibility = Visibility.Visible;
            else if (RbAO.IsChecked == true) PanelAO.Visibility = Visibility.Visible;
            else if (RbDI.IsChecked == true) PanelDI.Visibility = Visibility.Visible;
            else if (RbDO.IsChecked == true) PanelDO.Visibility = Visibility.Visible;
            else if (RbAlarm.IsChecked == true)
            {
                PanelAlarm.Visibility = Visibility.Visible;
                LblAddress.Visibility = Visibility.Collapsed;
                TxtAddress.Visibility = Visibility.Collapsed;
                LoadAnalogInputs();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RbAlarm.IsChecked != true)
            {
                if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtAddress.Text))
                {
                    MessageBox.Show("Ime i adresa su obavezna polja!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(TxtName.Text))
                {
                    MessageBox.Show("Ime alarma je obavezno!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            try
            {
                if (RbAI.IsChecked == true) CreateAnalogInput();
                else if (RbAO.IsChecked == true) CreateAnalogOutput();
                else if (RbDI.IsChecked == true) CreateDigitalInput();
                else if (RbDO.IsChecked == true) CreateDigitalOutput();
                else if (RbAlarm.IsChecked == true) CreateAlarm();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška: {ex.Message}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateAnalogInput()
        {
            CreatedTag = new AnalogInput
            {
                Name = TxtName.Text.Trim(),
                Address = TxtAddress.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                LowLimit = ParseDouble(TxtAiLowLimit.Text, "Low limit"),
                HighLimit = ParseDouble(TxtAiHighLimit.Text, "High limit"),
                Units = TxtAiUnits.Text.Trim(),
                ScanTime = TimeSpan.FromSeconds(ParseDouble(TxtAiScanTime.Text, "Scan time")),
                IsOnScan = ChkAiOnScan.IsChecked ?? true,
                Deadband = ParseDouble(TxtAiDeadband.Text, "Deadband"),
                Hysteresis = ParseDouble(TxtAiHysteresis.Text, "Hysteresis"),
                Value = 0
            };
        }

        private void CreateAnalogOutput()
        {
            CreatedTag = new AnalogOutput
            {
                Name = TxtName.Text.Trim(),
                Address = TxtAddress.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                InitValue = ParseDouble(TxtAoInitValue.Text, "Initial value"),
                CurrentValue = ParseDouble(TxtAoInitValue.Text, "Initial value")
            };
        }

        private void CreateDigitalInput()
        {
            CreatedTag = new DigitalInput
            {
                Name = TxtName.Text.Trim(),
                Address = TxtAddress.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                ScanTime = TimeSpan.FromSeconds(ParseDouble(TxtDiScanTime.Text, "Scan time")),
                IsOnScan = ChkDiOnScan.IsChecked ?? true,
                Value = 0
            };
        }

        private void CreateDigitalOutput()
        {
            int val = ParseInt(TxtDoInitValue.Text, "Initial value");
            if (val != 0 && val != 1)
                throw new Exception("Digital output početna vrednost mora biti 0 ili 1.");

            CreatedTag = new DigitalOutput
            {
                Name = TxtName.Text.Trim(),
                Address = TxtAddress.Text.Trim(),
                Description = TxtDescription.Text.Trim(),
                InitValue = val,
                CurrentValue = val
            };
        }

        private void CreateAlarm()
        {
            if (CbTagList.SelectedValue == null)
            {
                throw new Exception("Morate izabrati AI tag nad kojim se kreira alarm.");
            }

            CreatedAlarm = new Alarm
            {
                LimitValue = ParseDouble(TxtAlarmLimit.Text, "Limit value"),
                Direction = CbDirection.SelectedIndex == 0 ? EAlarmDirection.Above : EAlarmDirection.Below,
                Message = TxtAlarmMessage.Text.Trim(),
                State = EAlarmState.Inactive,
                TagName = CbTagList.SelectedValue.ToString()
            };
        }

        private double ParseDouble(string text, string fieldName)
        {
            if (!double.TryParse(text, out double result))
                throw new Exception($"Polje '{fieldName}' mora biti validan broj.");
            return result;
        }

        private int ParseInt(string text, string fieldName)
        {
            if (!int.TryParse(text, out int result))
                throw new Exception($"Polje '{fieldName}' mora biti validan ceo broj.");
            return result;
        }
    }
}