using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace ADO_SQLITE;

public partial class MainPage : ContentPage
{
    string TargetFile;
    ObservableCollection<DTO_Book> Books;

    public MainPage()
	{
		InitializeComponent();
        Books = new ObservableCollection<DTO_Book>();
    }

    public void ValidateDatabase(string bundleFileName, string fileName) {
        //Recupero il path della cartella in cui si trova l'applicazione
        string mainDir = FileSystem.Current.AppDataDirectory;
        //Genero il path completo con anche il nome del file al nuovo file
        TargetFile = Path.Combine(mainDir, fileName);
        if (!File.Exists(TargetFile)) {
            //Se il file non esiste nel file system della app, lo copia dal bundle
            try {
                using FileStream outputStream = File.OpenWrite(TargetFile);
                using Stream fs = FileSystem.Current.OpenAppPackageFileAsync(bundleFileName).Result;

                using BinaryWriter writer = new BinaryWriter(outputStream);
                using (BinaryReader reader = new BinaryReader(fs)) {
                    var bytesRead = 0;
                    int bufferSize = 1024;
                    byte[] bytes;
                    var buffer = new byte[bufferSize];
                    using (fs) {
                        do {
                            buffer = reader.ReadBytes(bufferSize);
                            bytesRead = buffer.Count();
                            writer.Write(buffer);
                        }
                        while (bytesRead > 0);
                    }
                }
            } catch (Exception ex) {
                DisplayAlert("Errore1", ex.Message, "OK");
                TargetFile = null;
            }
        }
    }

    private async void LoadDataClicked(object sender, EventArgs e)
    {
        ValidateDatabase("DB_Libri.db", "DB_Libri_App.db");
        try
        {
            string connectionTarget = $"Data Source={TargetFile}";
            using (var connection = new SqliteConnection(connectionTarget))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT ISBN, Titolo, Anno
                FROM Book
                ";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DTO_Book b = new DTO_Book(reader);

                            Books.Add(b);
                        }
                    }   
                }
            }
            ShowData.ItemsSource = Books;
        }
        catch (Exception ex)
        {
            await DisplayAlert(ex.Message, "Errore2", "OK");
        }
    }

    void ShowData_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var b = e.CurrentSelection[0] as DTO_Book;
        if (b != null)
        {
            Int64 isbn = Convert.ToInt64(b.ISBN);
            Navigation.PushAsync(new DetailsPage(isbn));
        }
    }

    private void LoadEditorsBtn_Clicked(object sender, EventArgs e) {
        Navigation.PushAsync(new EditorsPage());
    }

    private void LoadGenresBtn_Clicked(object sender, EventArgs e) {
        Navigation.PushAsync(new GenresPage());
    }
}


