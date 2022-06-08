using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Test_Get_TWStockList
{
    class Program
    {
        static void Main(string[] args)
        {
            StockListTW stockListTW = new StockListTW();
            List<Stock> stock_listedCompany = stockListTW.GetListFromURL(stockListTW.Url_Twse_ListedCompany);
            List<Stock> stock_otc = stockListTW.GetListFromURL(stockListTW.Url_Twse_Otc);
        }
    }
    public class Stock
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public TwStockType Type { get; set; }
        public string Industry { get; set; }
    }
    public enum TwStockType { 上市, 上櫃 }
    public class StockListTW
    {
        public string Url_Twse_ListedCompany = "https://isin.twse.com.tw/isin/C_public.jsp?strMode=2";
        public string Url_Twse_Otc = "https://isin.twse.com.tw/isin/C_public.jsp?strMode=4";

        public List<Stock> GetListFromURL(string url)
        {
            HtmlWeb web = new HtmlWeb();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            web.OverrideEncoding = Encoding.GetEncoding("Big5");
            var doc = web.Load(url);
            var tables = doc.DocumentNode.SelectNodes("//table")[1];
            var rows = tables.SelectNodes("tr");
            bool isStart = false;
            List<Stock> stocks = new List<Stock>();
            foreach (var row in rows)
            {
                if (!isStart)
                {
                    if (row.InnerText.Trim() == "股票")
                        isStart = true;
                }
                else
                {
                    var col = row.SelectNodes("td");
                    if (col.Count() == 1)
                        break;
                    Stock stock = new Stock();
                    stock.ID = col[0].InnerText.Trim().Substring(0, 4);
                    stock.Name = col[0].InnerText.Trim().Substring(4).Trim();
                    stock.Type = TwStockType.上市;
                    if (col[3].InnerText == "上櫃")
                        stock.Type = TwStockType.上櫃;
                    stock.Industry = col[4].InnerText;
                    stocks.Add(stock);
                }
            }
            return stocks;
        }
    }
}
