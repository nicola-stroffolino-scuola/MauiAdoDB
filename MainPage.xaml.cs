using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;

namespace ADO_SQLITE;

public partial class MainPage : ContentPage {
    public string BundleFileName { get; }
    public string FileName { get; }
    public string TargetFile { get; }
    ObservableCollection<DTO_Book> Books { get; }

    public MainPage()
	{
		InitializeComponent();
        BundleFileName = "DB_Libri.db";
        FileName = "DB_Libri_App.db";
        TargetFile = Path.Combine(FileSystem.Current.AppDataDirectory, FileName);
        Books = new ObservableCollection<DTO_Book>();
    }

    public void ValidateDatabase() {
        if (!File.Exists(TargetFile)) {
            //Se il file non esiste nel file system della app, lo copia dal bundle
            try {
                using FileStream outputStream = File.OpenWrite(TargetFile);
                using Stream fs = FileSystem.Current.OpenAppPackageFileAsync(BundleFileName).Result;

                using BinaryWriter writer = new BinaryWriter(outputStream);
                using (BinaryReader reader = new BinaryReader(fs)) {
                    var bytesRead = 0;
                    int bufferSize = 1024;
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
                DisplayAlert("Errore Validazione", ex.Message, "OK");
            }
        }
    }

    private async void LoadDataClicked(object sender, EventArgs e) {
        ValidateDatabase();
        ShowData.ItemsSource = null;
        try {
            string connectionTarget = $"Data Source={TargetFile}";
            using (var connection = new SqliteConnection(connectionTarget)) {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                SELECT ISBN, Titolo, Anno
                FROM Book
                ";

                using (var reader = command.ExecuteReader()) {
                    if (reader.HasRows) {
                        while (reader.Read()) {
                            DTO_Book b = new DTO_Book(reader);

                            Books.Add(b);
                        }
                    }   
                }
                connection.Close();
            }
            ShowData.ItemsSource = Books;
        } catch (Exception ex) {
            await DisplayAlert("Errore Query", ex.Message, "OK");
        }
    }

    void ShowData_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (e.CurrentSelection[0] is DTO_Book b) {
            Int64 isbn = Convert.ToInt64(b.ISBN);
            Navigation.PushAsync(new DetailsPage(isbn, TargetFile));
        }
    }

    private void LoadEditorsBtn_Clicked(object sender, EventArgs e) {
        Navigation.PushAsync(new EditorsPage());
    }

    private void LoadGenresBtn_Clicked(object sender, EventArgs e) {
        Navigation.PushAsync(new GenresPage());
    }

    private void CopyClipboardBtn_Clicked(object sender, EventArgs e) {
        try {
            Clipboard.Default.SetTextAsync(FileSystem.Current.AppDataDirectory);
        } catch (Exception ex) {
            DisplayAlert("Errore", ex.Message, "Ok");
            return;
        }
        DisplayAlert("Directory copiata nella clipboard", "", "Ok");
    }
}


