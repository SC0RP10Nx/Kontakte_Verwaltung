using System.Data.SQLite;
using System.Diagnostics;

namespace Kontaktverwaltung
{
    internal class Modell
    {
        // Verbindungszeichenfolge an einer Stelle, damit sie nicht in jeder Methode wiederholt wird
        private const string Verbindungszeichenfolge = "Data Source=database.db; Version=3;";

        private static Modell? ModellInstanz;

        private Modell()
        {
            Debug.WriteLine("--- Modell initialisiert ---");
        }

        // Singleton-Instanz der Klasse Modell zurückgeben
        public static Modell getInstanz()
        {
            if (ModellInstanz == null)
            {
                Modell instanz = new Modell(); // Wenn die Instanz noch nicht existiert, wird sie erstellt
                instanz.starteDb();            // Tabelle anlegen, falls noch nicht vorhanden
                ModellInstanz = instanz;       // erst danach veröffentlichen
            }

            return ModellInstanz;
        }

        private void starteDb()
        {
            using SQLiteConnection conn = CreateConnection();
            CreateTable(conn);
        }

        private SQLiteConnection CreateConnection()
        {
            SQLiteConnection conn = new SQLiteConnection(Verbindungszeichenfolge);
            conn.Open();
            return conn;
        }

        private void CreateTable(SQLiteConnection conn)
        {
            using SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText =
                "CREATE TABLE IF NOT EXISTS Kontakte (" +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "name VARCHAR(40), " +
                "tel VARCHAR(20))";
            cmd.ExecuteNonQuery();
        }

        // Legt Testkontakte an
        public void InsertData()
        {
            using SQLiteConnection conn = CreateConnection();
            using SQLiteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO Kontakte (name, tel) VALUES (@name, @tel)";
            cmd.Parameters.Add(new SQLiteParameter("@name"));
            cmd.Parameters.Add(new SQLiteParameter("@tel"));

            InsertEinen(cmd, "Leon Rohrreiniger", "123456789");
            InsertEinen(cmd, "Andre Tryhardt", "987654321");
            InsertEinen(cmd, "Joe Mama", "555555555");
            InsertEinen(cmd, "Maggus Ukrainer", "111111111");
            InsertEinen(cmd, "Christian Werner-Röhrich", "989898989");
        }

        // Füllt die beiden Parameter des übergebenen Befehls und führt ihn aus
        private void InsertEinen(SQLiteCommand cmd, string name, string telefon)
        {
            if (KontaktExistiert(name, telefon, 0))
            {
                return;
            }

            cmd.Parameters["@name"].Value = name;
            cmd.Parameters["@tel"].Value = telefon;
            cmd.ExecuteNonQuery();
        }

        // Liefert alle Kontakte, deren Name den Suchstring enthält.
        // Ein leerer Suchstring liefert alle Kontakte.
        public List<Kontakt> ReadData(string suchstring)
        {
            List<Kontakt> kontakte = new List<Kontakt>();

            using SQLiteConnection conn = CreateConnection();
            using SQLiteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT id, name, tel FROM Kontakte WHERE name LIKE @suchstring ORDER BY name";
            cmd.Parameters.AddWithValue("@suchstring", $"%{suchstring}%");

            using SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                kontakte.Add(new Kontakt(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2)
                ));
            }

            return kontakte;
        }

        // Löscht den Kontakt mit der angegebenen id und liefert die Anzahl gelöschter Zeilen
        public int DeleteData(int id)
        {
            using SQLiteConnection conn = CreateConnection();
            using SQLiteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "DELETE FROM Kontakte WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery();
        }

        // Aktualisiert Name und Telefonnummer des Kontakts mit der angegebenen id
        public int UpdateData(int id, string name, string telefon)
        {
            using SQLiteConnection conn = CreateConnection();
            using SQLiteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE Kontakte SET name = @name, tel = @tel WHERE id = @id";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@tel", telefon);
            cmd.Parameters.AddWithValue("@id", id);

            return cmd.ExecuteNonQuery();
        }

        public int InsertKontakt(string name, string telefon)
        {
            using SQLiteConnection conn = CreateConnection();
            using SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Kontakte (name, tel) VALUES (@name, @tel)";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@tel", telefon);
            return cmd.ExecuteNonQuery();
        }
        // Prüft, ob Kontakt mit gleichem Namen und gleicher Telefonnummer gibt.
        public bool KontaktExistiert(string name, string telefon, int ausserId)
        {
            using SQLiteConnection conn = CreateConnection();
            using SQLiteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT COUNT(*) FROM Kontakte WHERE name = @name AND tel = @tel AND id <> @id";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@tel", telefon);
            cmd.Parameters.AddWithValue("@id", ausserId);

            long anzahl = (long)cmd.ExecuteScalar();
            return anzahl > 0;
        }
    }
}
