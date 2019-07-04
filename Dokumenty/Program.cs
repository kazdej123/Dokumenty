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
										ParseDateTime(GetContent(root, "DataWystawienia")),
										GetContentAsDecimal(root, "K_14")
										+ GetContentAsDecimal(root, "K_19")
										+ GetContentAsDecimal(root, "K_20")));
								} while (xmlReader.ReadToNextSibling(xmlDocumentNodeName));
							}
						}
					} else {
						using (var streamReader = new StreamReader(fileName)) {
							string numerKontrahenta = default;
							string nazwaKontrahenta = default;
							string numerDokumentu = default;
							DateTime dataDokumentu = default;
							decimal wartoscDokumentu = default;

							bool kon, fs, vat, pla;
							kon = fs = vat = pla = false;

							var linesCounter = 1;
							for (string line; (line = streamReader.ReadLine()) != null; linesCounter++) {
								using (var stringReader = new StringReader(line)) {
									var fieldsList = new StringList();

									for (string field; (field = NextField(stringReader, linesCounter)) != null;) {
										fieldsList.Add(field);
									}
									if (fieldsList.Count == 0) {
										fieldsList.Add(null);
									}
									try {
										switch (fieldsList[0]) {
											case "KON":
												numerKontrahenta = fieldsList[2];
												nazwaKontrahenta = fieldsList[3];
												kon = true;
												break;

											case "FS":
												numerDokumentu = fieldsList[3];
												dataDokumentu = ParseDateTime(fieldsList[2]);
												fs = true;
												break;

											case "VAT":
												vat = true;
												break;

											case "PLA":
												wartoscDokumentu = ParseDecimal(fieldsList[4]);
												pla = true;
												break;

											default:
												if (fieldsList[0] != null) {
													ThrowIncorrectFileException(linesCounter);
												}
												break;
										}
										if (kon && fs && vat && pla) {
											documentsList.Add(new Document(numerKontrahenta,
																		   nazwaKontrahenta,
																		   numerDokumentu,
																		   dataDokumentu,
																		   wartoscDokumentu));
											kon = fs = vat = pla = false;
										}
									} catch (ArgumentOutOfRangeException) {
										ThrowIncorrectFileException(linesCounter);
									}
								}
							}
						}
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
			=> ParseDecimal(GetContent(xmlNode, elementName));

		private static string GetContent(XmlNode xmlNode, string elementName)
			=> xmlNode[elementName].InnerText;

		private static decimal ParseDecimal(string str)
			=> decimal.Parse(str, System.Globalization.NumberFormatInfo.InvariantInfo);

		private static DateTime ParseDateTime(string str) => DateTime.Parse(str);

		private static string NextField(StringReader reader, int lineNumber)
		{
			const char quotationMark = '"';
			const char semicolon = ';';
			const int endOfLine = -1;

			var field = "";
			var c = reader.Read();
			switch (c) {
				case quotationMark:
					while ((c = reader.Read()) != quotationMark) {
						if (c == endOfLine) {
							ThrowIncorrectFileException(lineNumber);
						}
						field += (char)c;
					}
					while ((c = reader.Read()) != semicolon && c != endOfLine) { }
					return field;

				case semicolon:
					return field;

				case endOfLine:
					return null;

				default:
					do {
						field += (char)c;
					} while ((c = reader.Read()) != semicolon && c != endOfLine);
					return field;
			}
		}

		private static void ThrowIncorrectFileException(int lineNumber)
			=> throw new Exception("Zle sformatowany plik w wierszu " + lineNumber + ".");
	}
}