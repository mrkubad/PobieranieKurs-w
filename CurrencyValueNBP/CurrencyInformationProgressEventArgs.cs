namespace CurrencyValueNBP
{
    public class CurrencyInformationProgressEventArgs
    {
        public int FilesParsed { get; set; }
        public int FilesOverall { get; set; }
        public int Progress { get => (FilesParsed * 100) / (FilesOverall > 0 ? FilesOverall : 1); }
    }
}
