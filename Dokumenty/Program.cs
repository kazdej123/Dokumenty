using System;
using System.Collections.Generic;
using System.IO;

namespace Documents
{
	internal sealed class Program
	{
		private static void Main(string[] args)
		{
			const string xmlFileExtension = ".xml";
			const string txtFileExtension = ".txt";

			var fileNamesList = new List<string>();

			if (args.Length == 0) {
				fileNamesList.AddRange(EnumerateFiles(xmlFileExtension));
				fileNamesList.AddRange(EnumerateFiles(txtFileExtension));
			} else {
				foreach (var fileName in args) {
					if (!File.Exists(fileName)) {
						WriteFileError(fileName, "Taki plik nie istnieje.");
						return;
					}
					var fileExtension = GetExtension(fileName);

					if (fileExtension != xmlFileExtension && fileExtension != txtFileExtension) {
						WriteFileError(fileName, "Ten plik nie ma wymaganego rozszerzenia ("
												 + xmlFileExtension + " lub " + txtFileExtension
												 + ").");
						return;
					}
				}
				fileNamesList.AddRange(args);
			}

			foreach (var fileName in fileNamesList) {
				if (GetExtension(fileName) == xmlFileExtension) {
					// TODO XML
				} else {
					try {
						using (var streamReader = new StreamReader(fileName)) {
							// TODO
						}
					} catch (Exception e) {
						WriteFileError(fileName, e.Message);
					}
				}
			}

			var documentsList = new List<Dokument>();

			foreach (var document in documentsList) {
				WriteLine(document.ToString());
			}
			_ = Console.Read();
		}

		private static IEnumerable<string> EnumerateFiles(string fileExtension)
			=> Directory.EnumerateFiles(".", "*" + fileExtension);

		private static void WriteFileError(string fileName, string errorMessage)
			=> WriteLine(fileName + ": " + errorMessage);

		private static void WriteLine(string message) => Console.WriteLine(message);

		private static string GetExtension(string fileName) => Path.GetExtension(fileName);
	}
}
