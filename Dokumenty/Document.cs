using System;

namespace Documents
{
	internal sealed class Document
	{
		private static readonly string _separator = "; ";

		internal Document(string numerKontrahenta, string nazwaKontrahenta, string numerDokumentu,
						  DateTime dataDokumentu, decimal wartoscDokumentu)
		{
			NumerKontrahenta = numerKontrahenta;
			NazwaKontrahenta = nazwaKontrahenta;
			NumerDokumentu = numerDokumentu;
			DataDokumentu = dataDokumentu;
			WartoscDokumentu = wartoscDokumentu;
		}

		private string NumerKontrahenta { get; }

		private string NazwaKontrahenta { get; }

		private string NumerDokumentu { get; }

		private DateTime DataDokumentu { get; }

		private decimal WartoscDokumentu { get; }

		public override string ToString()
			=> NumerKontrahenta + _separator + NazwaKontrahenta + _separator + NumerDokumentu
			+ _separator + DataDokumentu + _separator + WartoscDokumentu + Environment.NewLine;
	}
}