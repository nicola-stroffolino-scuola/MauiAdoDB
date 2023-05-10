using System;
namespace ADO_SQLITE
{
	public class DTO_Details
	{
		public Int64 ISBN { get; set; }
		public string Titolo { get; set; }
		public int Anno { get; set; }
		public float Prezzo { get; set; }
		public string Autori { get; set; }
		public string Editore { get; set; }
		public string GenereNarrativo { get; set; }

		public DTO_Details()
		{
		}
	}
}

