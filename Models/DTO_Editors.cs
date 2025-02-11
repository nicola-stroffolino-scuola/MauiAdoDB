﻿using Microsoft.Data.Sqlite;

namespace ADO_SQLITE.Models {
    public class DTO_Editors {
        public int Id { get; }
        public string Nome { get; }        
        public string Titoli { get; }
        public DTO_Editors(SqliteDataReader r) {
            Id = r.GetInt32(0);
            Nome = r.GetString(1);
            Titoli = r.GetString(2).Replace('@', '\n');
        }
    }
}
