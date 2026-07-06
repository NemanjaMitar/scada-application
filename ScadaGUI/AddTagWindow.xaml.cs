using DataConcentrator;
using DataConcentrator.Model;
using System;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ScadaGUI
{
    public partial class AddTagWindow : Window
    {
        public Tag CreatedTag { get; private set; }
        public Alarm CreatedAlarm { get; private set; }

        public AddTagWindow()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PanelAI == null || PanelAO == null || PanelDI == null || PanelDO == null || PanelAlarm == null)
                return;

            PanelAI.Visibility = Visibility.Collapsed;
            PanelAO.Visibility = Visibility.Collapsed;
            PanelDI.Visibility = Visibility.Collapsed;
            PanelDO.Visibility = Visibility.Collapsed;
            PanelAlarm.Visibility = Visibility.Collapsed;

            if (RbAI.IsChecked == true) PanelAI.Visibility = Visibility.Visible;
            else if (RbAO.IsChecked == true) PanelAO.Visibility = Visibility.Visible;
            else if (RbDI.IsChecked == true) PanelDI.Visibility = Visibility.Visible;
            else if (RbDO.IsChecked == true) PanelDO.Visibility = Visibility.Visible;
            else if (RbAlarm.IsChecked == true) PanelAlarm.Visibility = Visibility.Visible;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtAddress.Text))
            {
                MessageBox.Show("Ime i Adresa su obavezna polja!");
                return;
            }

            if (RbAI.IsChecked == true)
            {
                CreatedTag = new AnalogInput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text,
                    LowLimit = double.Parse(TxtAiLowLimit.Text),
                    HighLimit = double.Parse(TxtAiHighLimit.Text),
                    ScanTime = TimeSpan.FromSeconds(double.Parse(TxtAiScanTime.Text))
                };
            }
            else if (RbAO.IsChecked == true)
            {
                CreatedTag = new AnalogOutput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text,
                    InitValue = double.TryParse(TxtAoInitValue.Text, out double val) ? val : 0 // Koristi InitValue
                };

            }
            else if (RbDI.IsChecked == true)
            {
                CreatedTag = new DigitalInput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text,
                    ScanTime = TimeSpan.FromSeconds(double.Parse(TxtDiScanTime.Text))
                };
            }
            else if (RbDO.IsChecked == true)
            {
                CreatedTag = new DigitalOutput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text,
                    InitValue = double.TryParse(TxtDoInitValue.Text, out double val) ? val : 0 // Koristi InitValue
                };
            }
            else if (RbAlarm.IsChecked == true) // Pod pretpostavkom da postoji RadioButton sa ovim imenom
            {
                if (!double.TryParse(TxtAlarmLimit.Text, out double limit))
                {
                    MessageBox.Show("Granica alarma mora biti broj!");
                    return;
                }

                CreatedAlarm = new Alarm // Koristimo novu promenljivu
                {
                    LimitValue = limit,
                    Direction = CbDirection.SelectedIndex == 0 ? EAlarmDirection.Above : EAlarmDirection.Below,
                    Message = TxtAlarmMessage.Text,
                    State = EAlarmState.Inactive,
                    TagName = CbTagList.SelectedValue.ToString()
                };
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}