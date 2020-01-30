using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CurrencyValueNBP
{
    /// <summary>
    /// It represents file from "dir" text file
    /// </summary>
    internal class NBPXmlFile
    {
        public int Index { get; private set; }
        public string FileName { get; private set; }
        internal NBPXmlFile(int index, string fileName)
        {
            Index = index;
            FileName = fileName;
        }
        internal NBPXmlFile(string fileName) : this(-1, fileName) { }
        /// <summary>
        /// Returns URL to file on remote server
        /// </summary>
        internal string Link { get => $"http://www.nbp.pl/kursy/xml/{FileName}.xml"; }
        /// <summary>
        /// Downloads xml files from server using WebClient
        /// </summary>
        /// <returns>string with XML file content</returns>
        private async Task<string> DownloadAsync()
        {
            using(var downloader = new WebClient())
            {
                return await downloader.DownloadStringTaskAsync(Link);
            }
        }
        private string Download()
        {
            using (var downloader = new WebClient())
            {
                return downloader.DownloadString(Link);
            }
        }
        public StringReader DownloadAsStringReader() => new StringReader(Download());
        public async Task<StringReader> DownloadAsStringReaderAsync() => new StringReader(await DownloadAsync());
    }
}