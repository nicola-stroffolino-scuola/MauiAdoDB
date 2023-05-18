using ADO_SQLITE.Models;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace ADO_SQLITE;

public partial class EditorsPage : ContentPage
{
    string TargetFile;
    ObservableCollection<DTO_Editors> Editors;

    public EditorsPage() {
        InitializeComponent();
        TargetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "DB_Libri_App.db");
        Editors = new ObservableCollection<DTO_Editors>();
        GetQuery();
    }

    async void GetQuery() {
        try {
            string connectionTarget = $"Data Source={TargetFile}";
            using var connection = new SqliteConnection(connectionTarget);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
            SELECT Id, Nome, 
                COALESCE((
                    SELECT GROUP_CONCAT(Titolo, '@')
                    FROM Book
                    WHERE Book.Editore = Publisher.Id
                ), 'NESSUN LIBRO ASSOCIATO' ) AS Titoli
            FROM Publisher;
            ";

            using (var reader = command.ExecuteReader()) {
                if (reader.HasRows) {
                    while (reader.Read()) {
                        var b = new DTO_Editors(reader);
                        Editors.Add(b);
                    }
                }
                EditorsDisplay.ItemsSource = Editors;
            }

            connection.Close();
        } catch (Exception ex) {
            await DisplayAlert("Errore", ex.Message, "OK");
        }
    }
}
