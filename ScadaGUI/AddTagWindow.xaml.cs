using System;
using System.Windows;
using System.Windows.Forms;
using DataConcentrator;
using DataConcentrator.Model;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ScadaGUI
{
    public partial class AddTagWindow : Window
    {
        public Tag CreatedTag { get; private set; }

        public AddTagWindow()
        {
            InitializeComponent();
        }

        // Ova funkcija se okida svaki put kada korisnik promeni selekciju Radio dugmeta
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Prvo proveravamo da li su paneli inicijalizovani (da kod ne pukne pri pokretanju prozora)
            if (PanelAI == null || PanelAO == null || PanelDI == null || PanelDO == null)
                return;

            // Sakrij sve panele pre nego što prikažeš odgovarajući
            PanelAI.Visibility = Visibility.Collapsed;
            PanelAO.Visibility = Visibility.Collapsed;
            PanelDI.Visibility = Visibility.Collapsed;
            PanelDO.Visibility = Visibility.Collapsed;

            // Prikaži samo onaj panel čije je dugme pritisnuto
            if (RbAI.IsChecked == true) PanelAI.Visibility = Visibility.Visible;
            else if (RbAO.IsChecked == true) PanelAO.Visibility = Visibility.Visible;
            else if (RbDI.IsChecked == true) PanelDI.Visibility = Visibility.Visible;
            else if (RbDO.IsChecked == true) PanelDO.Visibility = Visibility.Visible;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validacija zajedničkih polja
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Ime taga je obavezno polje!", "Validacija", (MessageBoxButtons)MessageBoxButton.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtAddress.Text))
            {
                MessageBox.Show("I/O Adresa je obavezno polje!", "Validacija", (MessageBoxButtons)MessageBoxButton.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validacija specifičnih polja na osnovu selekcije
            if (RbAI.IsChecked == true)
            {
                if (!double.TryParse(TxtAiLowLimit.Text, out double lowLimit) ||
                    !double.TryParse(TxtAiHighLimit.Text, out double highLimit))
                {
                    MessageBox.Show("Granice za AI moraju biti brojčane vrednosti!", "Validacija", (MessageBoxButtons)MessageBoxButton.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (lowLimit >= highLimit)
                {
                    MessageBox.Show("Donja granica mora biti manja od gornje granice!", "Validacija", (MessageBoxButtons)MessageBoxButton.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!double.TryParse(TxtAiScanTime.Text, out double scanSeconds) || scanSeconds <= 0)
                {
                    MessageBox.Show("Period osvežavanja mora biti pozitivan broj!", "Validacija", (MessageBoxButtons)MessageBoxButton.OK, MessageBoxIcon.Warning);
                    return;
                }

                CreatedTag = new AnalogInput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text
                };
            }
            else if (RbAO.IsChecked == true)
            {
                if (!double.TryParse(TxtAoInitValue.Text, out double initVal))
                {
                    MessageBox.Show("Početna vrednost za AO mora biti broj!", "Validacija", (MessageBoxButtons)MessageBoxButton.OK, MessageBoxIcon.Warning);
                    return;
                }

                CreatedTag = new AnalogOutput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text
                };
            }
            else if (RbDI.IsChecked == true)
            {
                if (!double.TryParse(TxtDiScanTime.Text, out double scanSeconds) || scanSeconds <= 0)
                {
                    MessageBox.Show("Period skeniranja za DI mora biti pozitivan broj!", "Validacija", (MessageBoxButtons)MessageBoxButton.OK, MessageBoxIcon.Warning);
                    return;
                }

                CreatedTag = new DigitalInput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text
                };
            }
            else if (RbDO.IsChecked == true)
            {
                if (!double.TryParse(TxtDoInitValue.Text, out double initVal) || (initVal != 0 && initVal != 1))
                {
                    MessageBox.Show("Početno stanje za DO mora biti isključivo 0 ili 1!", "Validacija", (MessageBoxButtons)MessageBoxButton.OK, MessageBoxIcon.Warning);
                    return;
                }

                CreatedTag = new DigitalOutput
                {
                    Name = TxtName.Text,
                    Address = TxtAddress.Text,
                    Description = TxtDescription.Text
                };
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
