using AngleSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {

            var config = Configuration.Default.WithDefaultLoader();

            var document = Task.Run(() => BrowsingContext.New(config).OpenAsync("http://www.stats.gov.cn/tjsj/tjbz/xzqhdm/201703/t20170310_1471429.html")).Result;

            var query = document.QuerySelector(".xilan_con .TRS_Editor .TRS_PreAppend");

            var childrens = query.Children.ToList();

            StringBuilder sb = new StringBuilder();
            StringBuilder sb_sql = new StringBuilder();

            var location = new Dictionary<string, string>();

            foreach (var item in childrens)
            {
                item.InnerHtml = item.InnerHtml.Replace("&nbsp;", null).Replace("\r", null).Replace("\t", null).Replace("\n", null).Replace("　", null).Replace(" ", null);

                var content = item.TextContent.Trim();

                var code = content.Substring(0, 6);
                var name = content.Substring(6).Trim();

                sb.AppendLine(content);

                sb_sql.AppendLine($"INSERT INTO location (code,name) VALUES ('{code}','{name}')");

                location[code] = name;

                Console.WriteLine($"{code}{name}");
            }

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "location.sql"), sb_sql.ToString());

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "location.txt"), sb.ToString());

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "location.json"), Newtonsoft.Json.JsonConvert.SerializeObject(location));


            Console.ReadKey();
        }
    }
}
