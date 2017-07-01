using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceAPI;

namespace YahooFinanceAPITest
{
    class App
    {
        public App()
        {
            Console.WriteLine("Would you like to use manual entry (m) or bulk mode (b)?");
            switch (Console.ReadLine())
            {
                case "m":
                    manualMode();
                    break;
                case "b":
                    bulkMode();
                    break;
            }
        }

        private Config readConfig(string fileName)
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(fileName));
        }

        private void bulkMode()
        {
            string fileName;
            Console.WriteLine("Configuration file:");
            string configFileName = Console.ReadLine();
            Config config = readConfig(configFileName);
            string dateFrom = config.DateFrom;
            string dateUntil = config.DateUntil;
            
            // create the output folder
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), config.Dir));
            
            foreach (string symbol in config.Symbols)
            {
                Console.WriteLine("Getting data for {0}..", symbol);
                fileName = String.Format(config.FileNaming, dateFrom, dateUntil, symbol);
                var prices = getHistoricalPrice(symbol, Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateUntil));
                writeToFile(prices, String.Format("{0}\\{1}", config.Dir, fileName), symbol, config.DecimalSeparator.ToCharArray()[0]);
            }
            Console.WriteLine("Completed!");
            Console.ReadLine();
        }

        private void manualMode()
        {
            Console.WriteLine("Symbol:");
            string symbol = Console.ReadLine();
            Console.WriteLine("Date from:");
            string dateFrom = Console.ReadLine();
            Console.WriteLine("Date until:");
            string dateUntil = Console.ReadLine();
            Console.WriteLine("Getting data..");
            var prices = getHistoricalPrice(symbol, Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateUntil));
            string fileName = String.Format("Output\\{0}.csv", symbol);
            
            // create the output folder
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Output"));
            
            writeToFile(prices, fileName, symbol, ',');
            Console.WriteLine(String.Format("Exported data to {0}", fileName));
            manualMode();
        }

        private List<HistoryPrice> getHistoricalPrice(string symbol, DateTime from, DateTime until)
        {
            //first get a valid token from Yahoo Finance
            while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
            {
                Token.Refresh();
            }

            return Historical.Get(symbol, from, until);
        }

        private void writeToFile(List<HistoryPrice> prices, string fileName, string symbol, char decimalSeparator)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), fileName)))
            {
                file.WriteLine(symbol);
                file.WriteLine("date;open;high;low;close;volume;adjClose");

                foreach (HistoryPrice price in prices)
                {
                    file.WriteLine(String.Join(";", new string[7] {
                        price.Date.ToShortDateString(),
                        price.Open.ToString().Replace('.', decimalSeparator),
                        price.High.ToString().Replace('.', decimalSeparator),
                        price.Low.ToString().Replace('.', decimalSeparator),
                        price.Close.ToString().Replace('.', decimalSeparator),
                        price.Volume.ToString().Replace('.', decimalSeparator),
                        price.AdjClose.ToString().Replace('.', decimalSeparator),
                    }));
                }
            }

        }
    }

    class Config
    {
        public string Dir { get; set; }
        public string FileNaming { get; set; }
        public string DateFrom { get; set;  }
        public string DateUntil { get; set; }
        public List<string> Symbols { get; set; }
        public string DecimalSeparator { get; set; }
    }
}
