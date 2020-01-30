using CurrencyValueNBP;
using System;
using System.IO;
using System.Linq;
using System.Text;


namespace NBPWaluty
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: We need to check user unput
            if (args.Length == 3)
            {
                string currencyCode = args[0].ToUpper();
                if (DateTime.TryParse(args[1], out DateTime from))
                {
                    if (DateTime.TryParse(args[2], out DateTime to))
                    {
                        if (from > to) // if user reverse period points
                        {
                            var t = from;
                            from = to;
                            to = t;
                        }
                        if (currencyCode.Length < 3 && currencyCode.Length > 3)
                        {
                            invalidCurrency();
                            return;
                        }
                        var now = DateTime.Now;
                        if(from.Date == now.Date)
                        {
                            Console.WriteLine("Todays rates will be availabe tomorrow!!");
                            return;
                        }
                        
                        if(from.Date == now.AddDays(-1).Date)
                        {
                            if(now.Hour < 8 || (now.Hour == 8 && now.Minute < 15))
                            {
                                Console.WriteLine("Yesterday rates will be availabe after 8:15 am");
                                return;
                            }
                        }
                        NBPCurrencyInformation currencyInformation = null;
                        try
                        {
                            currencyInformation = new CurrencyValueNBP.NBPCurrencyInformation(currencyCode, from, to);
                        }
                        catch (InvalidDataException)
                        {
                            invalidCurrency();
                            return;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            invalidRange();
                            return;
                        }
                        currencyInformation.DataChanged += DisplayCurrenyValues;
                        currencyInformation.ProgressChanged += DataParsingProgressChanged;
                        createInterface(currencyCode);
                        currencyInformation.StartParsingDataAsync().Wait();
                        // Lastest date display
                        createInterface(currencyCode);
                        DisplayCurrenyValues(null, currencyInformation);
                        PrintAt("Naciśnij dowolny klawisz, aby zakończyć...", 0, Console.WindowHeight - 2);
                        Console.Read();
                        return;
                    }
                }
                Console.WriteLine("Bad date format! :(");
                Console.Read();
            }
            else
            {
                Console.WriteLine("Bad arguments passed :(");
                Console.Read();
            }
        }
        private static void invalidCurrency()
        {
            Console.WriteLine("Invalid currency code :(");
            Console.Read();
        }
        private static void invalidRange()
        {
            Console.WriteLine("Invalid range feeded :(");
            Console.Read();
        }
        private static void createInterface(string currencyCode)
        {
            Console.Clear();
            Console.WriteLine($"-----Statystyki dla waluty {currencyCode}-----\n");
            Console.WriteLine("Średnia arytmetyczna dla kursu sprzedaży: "); // 1, 42
            Console.WriteLine("Średnia arytmetyczna dla kursu kupna: "); // 2, 39
            Console.WriteLine();
            Console.WriteLine("Odchylenie standardowe dla kursu sprzedaży: "); // 4, 45
            Console.WriteLine("Odchylenie standardowe dla kursu kupna: "); // 5, 40
            Console.WriteLine();
            Console.WriteLine("Minimalny kurs sprzedaży: "); // 7, 27
            Console.WriteLine("Maksymalny kurs sprzedaży: "); // 8, 28
            Console.WriteLine();
            Console.WriteLine("Minimalny kurs kupna: "); // 9, 22
            Console.WriteLine("Maksymalny kurs kupna: "); // 10, 23
            Console.WriteLine();
            Console.WriteLine("Największa różnica kursowa wynosiła: "); // 12, 41
            Console.WriteLine("Miała ona miejsce w dniu/dniach: "); // 14, 0
        }

        private static void PrintAt(string str, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(str);
        }

        private static void DataParsingProgressChanged(object sender, CurrencyInformationProgressEventArgs e) => PrintAt($"UWAGA!! Zbieranie danych trwa!! Postęp: {e.Progress}%     ", 0, Console.WindowHeight - 1);

        private static void DisplayCurrenyValues(object sender, NBPCurrencyInformation e)
        {
            Console.CursorVisible = false;
            int valuesPaddingLeft = 57;
            PrintAt($"{ e.AverageSellingRate:0.#####0} zł", valuesPaddingLeft, 2);
            PrintAt($"{ e.AverageBuyingRate:0.#####0} zł", valuesPaddingLeft, 3);
            PrintAt($"{e.StandardDeviationSellingRate:0.#####0} zł", valuesPaddingLeft, 5);
            PrintAt($"{e.StandardDeviationBuyingRate:0.#####0} zł", valuesPaddingLeft, 6);

            CurrencyData current = e.MinimimSellingRate;
            PrintAt($"{current.SellingRate:0.#####0} zł ({current.ListingDateString})", valuesPaddingLeft, 8);
            current = e.MaximumSellingRate;
            PrintAt($"{current.SellingRate:0.#####0} zł ({current.ListingDateString})", valuesPaddingLeft, 9);
            current = e.MinimimBuyingRate;
            PrintAt($"{current.BuyingRate:0.#####0} zł ({current.ListingDateString})", valuesPaddingLeft, 11);
            current = e.MaximumBuyingRate;
            PrintAt($"{current.BuyingRate:0.#####0} zł ({current.ListingDateString})", valuesPaddingLeft, 12);
            var diffrences = e.BiggestDiference;
            PrintAt($"{diffrences.First().DifferenceBeetweenRates:0.#####0 zł}", valuesPaddingLeft, 14);
            StringBuilder line = new StringBuilder();
            int howManyCouldFit = Console.WindowWidth / 12; //bo data ma 12 znaków z spacją i przecinkiem
            int begginingLine = 16;
            for (int i = 0; i < diffrences.Length; ++i)
            {
                int j = 0;
                for (; j < howManyCouldFit; ++j)
                {
                    int index = i + j;
                    if (index >= diffrences.Length)
                        break;
                    line.Append(diffrences[i + j].ListingDateString);
                    if (j != howManyCouldFit - 1 && (index + 1) < diffrences.Length)
                    {
                        line.Append(", ");
                    }
                }
                i += j;
                PrintAt(line.ToString(), 0, begginingLine++);
                line.Clear();
            }
            Console.CursorVisible = true;
        }
    }
}
