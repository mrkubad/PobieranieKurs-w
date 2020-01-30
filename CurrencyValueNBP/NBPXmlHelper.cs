using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace CurrencyValueNBP
{
    internal static class NBPXmlHelper
    {
        /// <summary>
        /// Finds elements and gets their values
        /// </summary>
        /// <param name="xmlDocument">StringReader with XML file content</param>
        /// <param name="elementName">name of the elements from which we want to extract data</param>
        /// <returns>IEnumerable of data inside elements</returns>
        internal static IEnumerable<string> GetElementValues(StringReader xmlDocument, string elementName)
        {
            using (XmlTextReader reader = new XmlTextReader(xmlDocument))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == elementName)
                    {
                        yield return reader.ReadString();
                    }
                }
            }
        }
        /// <summary>
        /// Finds ONE element and gets its value
        /// </summary>
        /// <param name="xmlDocument">StringReader with XML file content</param>
        /// <param name="elementName">name of the element from which we want to extract data</param>
        /// <returns>string of value inside element</returns>
        internal static string GetElementValue(StringReader xmlDocument, string ElementName)
        {
            using (XmlTextReader reader = new XmlTextReader(xmlDocument))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == ElementName)
                    {
                        return reader.ReadString();
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// Converts string date to DateTime struct
        /// </summary>
        /// <param name="date">string with date(yyyy-MM-dd)</param>
        /// <returns>DateTime struct</returns>
        internal static DateTime CreateDateTimeFromString(string date)
        {
            var splited = date.Split('-');
            return new DateTime(int.Parse(splited[0]), int.Parse(splited[1]), int.Parse(splited[2]));
        }
        /// <summary>
        /// This is a method for implementing progress mechanism, and wraps actual searching method
        /// </summary>
        /// <param name="downloadResult">StringReader with XML file content</param>
        /// <param name="currencyCode">Currency code for which we want to collect information</param>
        /// <param name="progress">IProgress interface for progress mechanism</param>
        internal static void GetGetCurrencyInformationFromXMLProgress(StringReader downloadResult, string currencyCode, IProgress<CurrencyData> progress)
        {
            progress.Report(GetCurrencyInformationFromXML(downloadResult, currencyCode));
        }
        /// <summary>
        /// Searches data for given currency code in XML file
        /// </summary>
        /// <param name="xmlDocument">StringReader with XML file content</param>
        /// <param name="currencyCode">Currency code for which we want to collect information</param>
        /// <returns>CurrencyData instance with data from XML</returns>
        private static CurrencyData GetCurrencyInformationFromXML(StringReader xmlDocument, string currencyCode)
        {
            CurrencyData result = new CurrencyData();
            using (XmlTextReader reader = new XmlTextReader(xmlDocument))
            {
                bool findingEnded = false, properCurrency = false;
                while (!findingEnded && reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "data_notowania":
                                result.ListingDate = CreateDateTimeFromString(reader.ReadString());
                                break;
                            case "kod_waluty":
                                properCurrency = (reader.ReadString() == (result.CurrencyCode = currencyCode));
                                break;
                            case "kurs_kupna":
                                if (properCurrency)
                                    result.BuyingRate = double.Parse(reader.ReadString());
                                break;
                            case "kurs_sprzedazy":
                                if (properCurrency)
                                {
                                    result.SellingRate = double.Parse(reader.ReadString());
                                    findingEnded = true;
                                }
                                break;
                        }
                        if (reader.Name != "pozycja" && reader.Name != "tabela_kursow")
                            reader.Skip();
                    }
                }
            }
            return result;
        }
    }
}
