using System;
using Microsoft.Data.Sqlite;

namespace ADO_SQLITE
{
	public class DTO_Book
	{
		public Int64 ISBN { get; set; }
		public string Titolo { get; set; }
		public int Anno { get; set; }

		public DTO_Book(SqliteDataReader r)
		{
            ISBN = r.GetInt64(0);
            Titolo = r.GetString(1);
            Anno = r.GetInt32(2);
        }
    }

}

