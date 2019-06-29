using System;

namespace Dokumenty
{
	internal sealed class Program
	{
		private static void Main(string[] args)
		{
			var listaDokumentow = new System.Collections.Generic.List<Dokument>();

			foreach (var dokument in listaDokumentow) {
				Console.WriteLine(dokument.ToString());
			}
			_ = Console.Read();
		}
	}
}
