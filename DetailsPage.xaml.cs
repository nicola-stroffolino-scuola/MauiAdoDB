using System.Diagnostics;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Text;
using Microsoft.Maui.Storage;
using static System.Reflection.Metadata.BlobBuilder;
using System.Collections;

namespace ADO_SQLITE;

public partial class DetailsPage : ContentPage
{
    public string TargetFile { get; }  
    public DTO_Details BookDetails { get; private set; }
    
    public DetailsPage(Int64 id, string TargetFile) {
		InitializeComponent();
        this.TargetFile = TargetFile;
        GetQuery(id);
    }

	async void GetQuery(Int64 idLibro) {
        try {
            string connectionTarget = $"Data Source={TargetFile}";
            using (var connection = new SqliteConnection(connectionTarget)) {
                connection.Open();

                //Ricerca autori
                var getAutori = connection.CreateCommand();
                getAutori.CommandText =
                @"
                SELECT A.Nome, A.Cognome
                From Write as W 
                INNER JOIN Author as A on A.Id = W.Autore
                Where W.Libro = @idLibro
                ";
                getAutori.Parameters.AddWithValue("@idLibro", idLibro);

                using (var readerAutori = getAutori.ExecuteReader()) {
                    if (readerAutori.HasRows) {
                        StringBuilder elencoAutori = new();
                        while (readerAutori.Read()) {
                            elencoAutori.Append(readerAutori.GetString(0));
                            elencoAutori.Append(" ");
                            elencoAutori.Append(readerAutori.GetString(1));
                            elencoAutori.Append(", ");
                        }
                        lbl_Autori.Text = elencoAutori.ToString()[..(elencoAutori.Length - 2)];
                    }
                    //Ricerca libro
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                    SELECT B.ISBN, B.Titolo, B.Anno, B.prezzo, P.Nome as Editore,
                        G.Descrizione as GenereNarrativo
                    FROM Book as B
                    INNER JOIN Publisher as P ON B.Editore = P.Id
                    INNER JOIN Genre as G ON B.Genere = G.Id
                    WHERE B.ISBN = @idLibro
                    ";
                    command.Parameters.AddWithValue("@idLibro", idLibro);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.HasRows) {
                            reader.Read();
                            BookDetails = new(reader);
                        }
                    }

                    lbl_ISBN.Text = BookDetails.ISBN.ToString();
                    lbl_Titolo.Text = BookDetails.Titolo.ToString();
                    lbl_Anno.Text = BookDetails.Anno.ToString();
                    lbl_Prezzo.Text = BookDetails.Prezzo.ToString();
                    lbl_Editore.Text = BookDetails.Editore.ToString();
                    lbl_Genere.Text = BookDetails.GenereNarrativo.ToString();
                }
            }
        } catch (Exception ex) {
            await DisplayAlert("Errore Query Dettagli", ex.Message, "OK");
        }
    }

    async void SaveButton_Clicked(object sender, EventArgs e) {
        string isbn = lbl_ISBN.Text;
        string titolo = lbl_Titolo.Text;
        string anno = lbl_Anno.Text;
        string prezzo = lbl_Prezzo.Text;

        try {
            string connectionTarget = $"Data Source={TargetFile}";
            var connection = new SqliteConnection(connectionTarget);
            connection.Open();
            
            var command = connection.CreateCommand();
            command.CommandText =
            @"
            UPDATE Book
            SET Titolo = @TITOLO, Anno = @ANNO, prezzo = @PREZZO
            WHERE ISBN = @ISBN
            ";
            // Query Parametrizzate
            command.Parameters.AddWithValue("@TITOLO", titolo);
            command.Parameters.AddWithValue("@ANNO", anno);
            command.Parameters.AddWithValue("@PREZZO", prezzo);
            command.Parameters.AddWithValue("@ISBN", isbn);

            var changes = GetChanges(new() {
                { "Titolo", titolo },
                { "Anno", anno },
                { "Prezzo", prezzo }
            });

            if(changes.Item1 != 0) {
                if (await DisplayAlert("Warning", changes.Item2 + "\nConfermare?", "Ok", "Annulla")) {
                    command.ExecuteNonQuery();
                }
            }

            connection.Close();
        } catch (Exception ex) {
            await DisplayAlert("Errore", ex.Message, "OK");
        }
    }

    Tuple<int, string> GetChanges(Dictionary<string, string> parameters) {
        string message = "Effettuare i seguenti cambiamenti?\n\n";
        int changes = 0;

        if (BookDetails.Titolo != parameters.GetValueOrDefault("Titolo")) {
            message += $"Titolo: {BookDetails.Titolo} ⇒ {parameters.GetValueOrDefault("Titolo")}\n";
            changes++;
        }
            
        if (BookDetails.Anno.ToString() != parameters.GetValueOrDefault("Anno")) {
            message += $"Anno: {BookDetails.Anno} ⇒ {parameters.GetValueOrDefault("Anno")}\n";
            changes++;
        }
            
        if (BookDetails.Prezzo.ToString() != parameters.GetValueOrDefault("Prezzo")) {
            message += $"Prezzo: {BookDetails.Prezzo} ⇒ {parameters.GetValueOrDefault("Prezzo")}\n";
            changes++;
        }
        
        return new(changes, message);
    }
}
