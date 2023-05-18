using System.Diagnostics;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;

namespace ADO_SQLITE;

public partial class MainPage : ContentPage
{
	int count = 0;
    public ObservableCollection<DTO_Book> Books = new ObservableCollection<DTO_Book>();
    
	public MainPage()
	{
		InitializeComponent();
	}

    public string ValidateDatabase(string bundleFileName, string fileName) {
        //Recupero il path della cartella in cui si trova l'applicazione
        string mainDir = FileSystem.Current.AppDataDirectory;
        //Genero il path completo con anche il nome del file al nuovo file
        string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, fileName);
        if (!File.Exists(targetFile)) {
            //Se il file non esiste nel file system della app, lo copia dal bundle
            try {
                using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
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
            }
        }
        return targetFile;
    }

    private async void LoadDataClicked(object sender, EventArgs e)
    {
        string targetFile = ValidateDatabase("DB_Libri.db", "DB_Libri_App.db");
        try
        {
            string connectionTarget = $"Data Source={targetFile}";
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
                            //b.ISBN = reader.GetInt64(0);
                            //b.Titolo = reader.GetString(1);
                            //b.Anno = reader.GetInt32(2);
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

    public async Task MoveFile(string sourceFile, string targetFileName)
    {
        // Read the source file
        using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync(sourceFile);
        using StreamReader reader = new StreamReader(fileStream);

        string content = await reader.ReadToEndAsync();

        // Write the file content to the app data directory
        string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);

        using FileStream outputStream = System.IO.File.OpenWrite(targetFile);
        using StreamWriter streamWriter = new StreamWriter(outputStream);

        await streamWriter.WriteAsync(content);
    }

    void ShowData_SelectionChanged(System.Object sender,
        Microsoft.Maui.Controls.SelectionChangedEventArgs e)
    {
        var b = e.CurrentSelection[0] as DTO_Book;
        if (b != null)
        {
            Int64 isbn = Convert.ToInt64(b.ISBN);
            Navigation.PushAsync(new DetailsPage(isbn));
        }
    }

    private void LoadEditorsBtn_Clicked(object sender, EventArgs e) {
        
    }

    private void LoadGenresBtn_Clicked(object sender, EventArgs e) {

    }
}


