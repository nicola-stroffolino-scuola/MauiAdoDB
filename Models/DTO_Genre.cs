using Microsoft.Data.Sqlite;

namespace ADO_SQLITE.Models {
    public class DTO_Genre {
        public int Id { get; }
        public string Descrizione { get; }
        public string Titoli { get; }
        public DTO_Genre(SqliteDataReader r) {
            Id = r.GetInt32(0);
            Descrizione = r.GetString(1);
            Titoli = r.GetString(2).Replace('@', '\n');
        }
    }
}
