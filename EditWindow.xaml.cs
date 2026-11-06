using System.Windows;

namespace Kontaktverwaltung
{
    /// <summary>
    /// Interaktionslogik für EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public string KontaktName => Tbx_Name.Text.Trim();
        public string KontaktTelefon => Tbx_Telefon.Text.Trim();

        public EditWindow(string titel, string name, string telefon) : this(titel)
        {
            Tbx_Name.Text = name;
            Tbx_Telefon.Text = telefon;
        }

        public EditWindow()
        {
            InitializeComponent();
        }
        public EditWindow(string titel)
        {
            InitializeComponent();
            Lbl_Titel.Content = titel;
        }


        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (KontaktName.Length < 2)
            {
                MessageBox.Show("Der Name muss mindestens 2 Zeichen haben.",
                    "Eingabe unvollständig", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (IsValidName(KontaktName) == false)
            {
                MessageBox.Show("Der Name darf nur Buchstaben,Leerzeichen,Bindestriche oder Apostrophe enthalten.",
                    "Eingabe fehlerhaft", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IsValidTelNumber(KontaktTelefon) == false)
            {
                MessageBox.Show("Die Telefonnummer darf nur Zahlen, '+', '/', Leerzeichen oder Bindestriche enthalten.",
                    "Eingabe fehlerhaft", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (KontaktTelefon.Length < 3)
            {
                MessageBox.Show("Die Telefonnummer muss mindestens 3 Zeichen haben.",
                    "Eingabe unvollständig", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            DialogResult = true;
        }

        private bool IsValidName(string name)
        {
            foreach (char zeichen in name)
            {
                if (!char.IsLetter(zeichen) && zeichen != ' ' && zeichen != '-' && zeichen != '\'')
                {
                    return false;
                }
            }
            return true;
        }
        private bool IsValidTelNumber(string tel)
        {
            foreach (char zeichen in tel)
            {
                if (!char.IsDigit(zeichen) && zeichen != '-' && zeichen != '+' && zeichen != ' ' && zeichen != '/')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
