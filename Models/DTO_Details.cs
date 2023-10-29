using Microsoft.Data.Sqlite;
using System.Reflection.PortableExecutable;

namespace ADO_SQLITE
{
	public class DTO_Details
	{
		public Int64 ISBN { get; }
		public string Titolo { get; }
		public int Anno { get; }
		public float Prezzo { get; }
		public string Editore { get; }
		public string GenereNarrativo { get; }

		public DTO_Details(SqliteDataReader r) {
            ISBN = r.GetInt64(0);
            Titolo = r.GetString(1);
            Anno = r.GetInt32(2);
            Prezzo = r.GetFloat(3);
            Editore = r.GetString(4);
            GenereNarrativo = r.GetString(5);
        }
	}
}

