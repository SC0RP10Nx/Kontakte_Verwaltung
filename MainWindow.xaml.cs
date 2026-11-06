using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kontaktverwaltung
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LadeKontakte();
        }


        // Liest die Kontakte passend zum aktuellen Suchtext und zeigt sie im DataGrid an.
        private void LadeKontakte()
        {
            try
            {
                Modell modell = Modell.getInstanz();
                Dg_Kontakte.ItemsSource = modell.ReadData(Tbx_Suche.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Die Kontakte konnten nicht geladen werden.\n\n" + ex.Message, "Datenbankfehler",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Btn_Insert_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("--- Insert-Button geklickt ---");

            try
            {
                Modell modell = Modell.getInstanz();
                modell.InsertData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Die Testdaten konnten nicht eingefügt werden.\n\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LadeKontakte();
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("--- Delete-Button geklickt ---");

            if (Dg_Kontakte.SelectedItem is not Kontakt ausgewaehlt)
            {
                MessageBox.Show("Bitte zuerst eine Zeile in der Tabelle markieren.",
                    "Kein Kontakt ausgewählt", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult antwort = MessageBox.Show(
                $"Soll der Kontakt \"{ausgewaehlt.Name}\" wirklich gelöscht werden?",
                "Löschen bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (antwort != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                Modell modell = Modell.getInstanz();
                modell.DeleteData(ausgewaehlt.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Der Kontakt konnte nicht gelöscht werden.\n\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LadeKontakte();
        }

        private void Btn_Neu_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("--- Neu-Button geklickt ---");
            EditWindow fenster = new EditWindow("Neuen Kontakt anlegen");
            if (fenster.ShowDialog() != true)
            {
                return;
            }
            try
            {
                Modell modell = Modell.getInstanz();

                if (modell.KontaktExistiert(fenster.KontaktName, fenster.KontaktTelefon, 0))
                {
                    MessageBox.Show("Dieser Kontakt ist bereits vorhanden.",
                        "Doppelter Kontakt", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                
                modell.InsertKontakt(fenster.KontaktName, fenster.KontaktTelefon);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Der Kontakt konnte nicht gespeichert werden.\n\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LadeKontakte();
        }

        private void Tbx_Suche_TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine("Suche geändert: " + Tbx_Suche.Text);
            String suche = Tbx_Suche.Text;
            LadeKontakte();
        }


        private void Dg_Kontakte_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Dg_Kontakte.SelectedItem is not Kontakt ausgewaehlt)
            {

                return;
            }
            Debug.WriteLine($"--- Doppelklick auf Kontakt: {ausgewaehlt.Name} ---");
            EditWindow fenster = new EditWindow("Kontakt bearbeiten", ausgewaehlt.Name, ausgewaehlt.Telefon);

            if (fenster.ShowDialog() != true)
            {
                return;
            }
            try
            {
                Modell modell = Modell.getInstanz();
                if (modell.KontaktExistiert(fenster.KontaktName, fenster.KontaktTelefon, ausgewaehlt.Id))
                {
                    MessageBox.Show("Ein anderer Kontakt mit diesem Namen und dieser Nummer ist bereits vorhanden.",
                        "Doppelter Kontakt", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                modell.UpdateData(ausgewaehlt.Id, fenster.KontaktName, fenster.KontaktTelefon);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Die Änderungen konnten nicht gespeichert werden.\n\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LadeKontakte();
        }
    }
}
