Per utililzzare SQLite in un programma C# occorre utilizzare ADO (ActiveX Data
Object).

SQLite è una libreria scritta in C, per cui in codice non gestito.
Per codice gestito si intende quel codice che per essere eseguito necessita di
un runtime di controllo, come Java e .Net (CLR - Common Language Runtime).

ADO.NET è un set di classi che espongono servizi di accesso ai dati per i
programmatori .NET Framework. ADO.NET fornisce un set completo per la creazione
di applicazioni distribuite e abilitate alla condivisione dei dati. ADO.NET è
parte integrante di .NET Framework e consente l'accesso ai dati relazionali,
ai dati XML e ai dati dell'applicazione.

La prima cosa da fare è quindi aggiungere al progetto il pacchetto NuGet per
accedere ad un DB SQLite.
Esiste un data provider ADO.NET scritto da SQLite, ma non è compatibile con
Mac con processore Apple Silicon.
Pertanto è bene utilizzare il data provider Microsoft.

Tasto destro sul nome del progetto e Aggiungi pacchetti NuGet: il pacchetto da
aggiungere è

Microsoft.Data.Sqlite

A questo punto con una direttiva Using si crea una connessione con il DB.
Creata la connessione:
- la si apre,
- si crea un comando,
- si scrive il testo del comando (in questo caso una query di selezione),
- si valorizzano gli eventuali parametri nella stringa del comando,
- si esegue il comando,
- si processano i risultati una linea alla volta in base alle necessità. 

using (var connection = new SqliteConnection("Data Source=hello.db"))
{
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText =
    @"
        SELECT name
        FROM user
        WHERE id = $id
    ";
    command.Parameters.AddWithValue("$id", id);

    using (var reader = command.ExecuteReader())
    {
        while (reader.Read())
        {
            var name = reader.GetString(0);

            Console.WriteLine($"Hello, {name}!");
        }
    }
}