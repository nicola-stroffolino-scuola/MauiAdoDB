using ADO_SQLITE.Models;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace ADO_SQLITE;

public partial class GenresPage : ContentPage
{
    string TargetFile;
    ObservableCollection<DTO_Genre> Genres;

    public GenresPage()
	{   
		InitializeComponent();
        TargetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "DB_Libri_App.db");
        Genres = new ObservableCollection<DTO_Genre>();
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
            SELECT *, 
                COALESCE((
                    SELECT GROUP_CONCAT(Titolo, '@')
                    FROM Book
                    WHERE Book.Genere = Genre.Id
                ), 'NESSUN LIBRO ASSOCIATO' ) AS Titoli
            FROM Genre;
            ";

            using (var reader = command.ExecuteReader()) {
                if (reader.HasRows) {
                    while (reader.Read()) {
                        var b = new DTO_Genre(reader);
                        Genres.Add(b);
                    }
                }
                GenreDisplay.ItemsSource = Genres;
            }

            connection.Close();
        } catch (Exception ex) {
            await DisplayAlert("Errore", ex.Message, "OK");
        }
    }
}
