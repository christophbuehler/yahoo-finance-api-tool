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
            listen();
        }

        private void listen()
        {
            Console.WriteLine("Symbol:");
            string symbol = Console.ReadLine();
            Console.WriteLine("Date from (yyyy-mm-dd):");
            string dateFrom = Console.ReadLine();
            Console.WriteLine("Date until (yyyy-mm-dd):");
            string dateUntil = Console.ReadLine();
            Console.WriteLine("Getting data..");
            var prices = getHistoricalPrice(symbol, Convert.ToDateTime(dateFrom), Convert.ToDateTime(dateUntil));
            string fileName = String.Format("{0}_{1}_{2}.csv", dateFrom, dateUntil, symbol);
            writeToFile(fileName, prices);
            Console.WriteLine(String.Format("Exported data to {0}", fileName));
            listen();
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

        private void writeToFile(string fileName, List<HistoryPrice> prices)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(String.Format("{0}\\{1}", Directory.GetCurrentDirectory(), fileName)))
            {
                file.WriteLine("date;open;close;adjClose;high;low;volume");

                foreach (HistoryPrice price in prices)
                {
                    file.WriteLine(String.Join(";", new string[7] {
                        price.Date.ToShortDateString(),
                        price.Open.ToString(),
                        price.Close.ToString(),
                        price.AdjClose.ToString(),
                        price.High.ToString(),
                        price.Low.ToString(),
                        price.Volume.ToString()
                    }));
                }
            }

        }
    }
}
