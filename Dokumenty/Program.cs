using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Documents
{
	internal sealed class Program
	{
		private sealed class StringList : List<string> { }

		private static void Main(string[] args)
		{
			const string xmlExtension = ".xml";
			const string txtExtension = ".txt";

			var fileNamesList = new StringList();

			if (args.Length == 0) {
				AddFileNamesByExtension(fileNamesList, xmlExtension);
				AddFileNamesByExtension(fileNamesList, txtExtension);
			} else {
				foreach (var fileName in args) {
					if (!File.Exists(fileName)) {
						WriteFileError(fileName, "Taki plik nie istnieje.");
						return;
					}
					var fileExtension = GetExtension(fileName);
					if (fileExtension != xmlExtension && fileExtension != txtExtension) {
						WriteFileError(fileName, "Ten plik nie ma wymaganego rozszerzenia ("
												 + xmlExtension + " lub " + txtExtension + ").");
						return;
					}
				}
				fileNamesList.AddRange(args);
			}

			var documentsList = new List<Document>();

			foreach (var fileName in fileNamesList) {
				try {
					if (GetExtension(fileName) == xmlExtension) {
						using (var xmlReader = XmlReader.Create(fileName)) {
							const string xmlDocumentNodeName = "SprzedazWiersz";
							if (xmlReader.ReadToFollowing(xmlDocumentNodeName)) {
								do {
									var xmlDocument = new XmlDocument();
									xmlDocument.LoadXml(xmlReader.ReadOuterXml());

									var root = xmlDocument.FirstChild;
									documentsList.Add(new Document(
										GetContent(root, "NrKontrahenta"),
										GetContent(root, "NazwaKontrahenta"),
										GetContent(root, "DowodSprzedazy"),
										DateTime.Parse(GetContent(root, "DataWystawienia")),
										GetContentAsDecimal(root, "K_14")
										+ GetContentAsDecimal(root, "K_19")
										+ GetContentAsDecimal(root, "K_20")));
								} while (xmlReader.ReadToNextSibling(xmlDocumentNodeName));
							}
						}
					} else {
						// TODO
					}
				} catch (Exception e) {
					WriteFileError(fileName, e.Message);
					return;
				}
			}
			foreach (var document in documentsList) {
				WriteLine(document.ToString());
			}
		}

		private static void AddFileNamesByExtension(StringList fileNamesList, string fileExtension)
			=> fileNamesList.AddRange(Directory.EnumerateFiles(".", "*" + fileExtension));

		private static void WriteFileError(string fileName, string errorMessage)
			=> WriteLine(fileName + ": " + errorMessage);

		private static void WriteLine(object o) => Console.WriteLine(o);

		private static string GetExtension(string fileName) => Path.GetExtension(fileName);

		private static decimal GetContentAsDecimal(XmlNode xmlNode, string elementName)
			=> decimal.Parse(GetContent(xmlNode, elementName),
							 System.Globalization.NumberFormatInfo.InvariantInfo);

		private static string GetContent(XmlNode xmlNode, string elementName)
			=> xmlNode[elementName].InnerText;
	}
}