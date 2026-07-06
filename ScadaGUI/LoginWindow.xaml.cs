using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using DataConcentrator;
using DataConcentrator.Model;
using ScadaGUI.Services;

namespace ScadaGUI
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text;
            string password = TxtPassword.Password;

            var user = ContextClass.Instance.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                Logger.Log($"Korisnik '{username}' (Uloga: {user.UserRole}) se uspješno prijavio.");

                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                Logger.Log($"Neuspješan pokušaj prijave za korisničko ime: '{username}'.");
                MessageBox.Show("Pogrešno korisničko ime ili lozinka.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text;
            string password = TxtPassword.Password;

            if (!ValidatePassword(password))
            {
                MessageBox.Show("Lozinka mora imati tačno 15 karaktera, barem jedno veliko slovo, jedno malo slovo i jedan specijalni karakter.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Provjera jedinstvenosti lozinke u bazi (prema zahtjevu zadatka)
            if (ContextClass.Instance.Users.Any(u => u.Password == password))
            {
                MessageBox.Show("Ova lozinka se već koristi u bazi. Molimo izaberite drugu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ContextClass.Instance.Users.Any(u => u.Username == username))
            {
                MessageBox.Show("Korisničko ime već postoji.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Role selectedRole = (Role)CmbRole.SelectedIndex;

            User newUser = new User
            {
                Username = username,
                Password = password,
                UserRole = selectedRole
            };

            ContextClass.Instance.Users.Add(newUser);
            ContextClass.Instance.SaveChanges();

            Logger.Log($"Registriran novi korisnik: '{username}' sa ulogom {selectedRole}.");
            MessageBox.Show("Uspješna registracija. Možete se prijaviti.", "Uspjeh", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 15) return false;

            if (!password.Any(char.IsUpper)) return false; // Provera velikog slova
            if (!password.Any(char.IsLower)) return false; // Provera malog slova
                                                           // Provera za specijalni karakter (sve što nije slovo ili broj)
            if (!password.Any(ch => !char.IsLetterOrDigit(ch))) return false;

            return true;
        }
    }
}