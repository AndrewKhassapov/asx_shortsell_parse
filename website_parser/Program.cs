using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace website_parser
{

    public class Program
    {
        ///<summary>URL to parse data from.</summary>
        const string FETCH_URL = "https://www.asx.com.au/data/shortsell.txt";

        public static void Main()
        {
            Console.WriteLine(ParseHTML());
        }

        protected static string ParseHTML()
        {
            string htmlBody = GetHTML().Result;
            string[] parsedHTMLArr = htmlBody.Split('\n');
            string parsedHTML = String.Empty;

            Data[] htmlData = new Data[parsedHTMLArr.Length];

            for(int i = 0; i < parsedHTMLArr.Length; i++)
            {
                htmlData[i] = new Data(parsedHTMLArr[i]);
            }

            Array.Sort(htmlData);

            foreach(Data i in htmlData) {
                parsedHTML += i.Print();
            }

            return parsedHTML;
        }

        protected static async Task<String> GetHTML()
        {
            var client = new HttpClient();
            // Create the HttpContent for the form to be posted.
            var requestContent = new FormUrlEncodedContent(new[] {
    new KeyValuePair<string, string>("text", "Text block"),
    });

            // Get the response.
            HttpResponseMessage response = await client.PostAsync(FETCH_URL, requestContent);

            // Get the response content.
            HttpContent responseContent = response.Content;

            // Get the stream of the content.
            using(var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                // Write the output.
                return (await reader.ReadToEndAsync());
            }
        }
    }

    public class Data : IComparable<Data>
    {
        protected string ticker { get; private set; }
        protected double percentShorted { get; private set; }
        protected double volume { get; private set; }
        protected double capital { get; private set; }
        protected MatchCollection matches { get; private set; }

        public Data(string line)
        {
            matches = ParseInput(line);
            ticker = ReadStringAtIndex(0);
            percentShorted = ReadDoubleAtIndex(-1);
            volume = ReadDoubleAtIndex(-3);
            capital = ReadDoubleAtIndex(-2);
        }

        protected MatchCollection ParseInput(string line) {
            // Set the regular expression for parsing digits and numbers.
            string regex = @"[\.\,\:a-zA-Z0-9_]+";
            return Regex.Matches(line, regex, RegexOptions.ECMAScript);
        }

        protected string ReadStringAtIndex(int index)
        {
            if(index < 0)
            {
                index = matches.Count + index;
            }

            if((index >= 0) && (index < matches.Count))
            {
                return matches[index].Value;
            }
            return ("No matches at line " + index.ToString());
        }

        protected double ReadDoubleAtIndex(int index)
        {
            double parsedString = 0.0;

            if(index < 0)
            {
                index = matches.Count + index;
            }

            if((index >= 0) && (index < matches.Count))
            {
                double.TryParse(matches[index].Value, System.Globalization.NumberStyles.Number, System.Globalization.NumberFormatInfo.InvariantInfo, result: out parsedString);
                return parsedString;
            }
            return 0.0;
        }

        public int Compare(Data a, Data b)
        {
            if(a.percentShorted > b.percentShorted)
            {
                return 1;
            } else if(a.percentShorted < b.percentShorted)
            {
                return -1;
            } else
            {
                return 0;
            }
        }

        public int CompareTo(Data obj)
        {
            if(obj != null){
                return this.percentShorted.CompareTo(obj.percentShorted);
            }
            else{
                throw new ArgumentException("Object is not a " + obj.GetType());
            }
        }

        ///<summary>Prints data information as string</sumamry>
        public string Print()
        {
            //return string.Format("Matches:[1}", matches);
            return string.Format("Ticker:{0}\nPercent Shorted:{1}%\n\n", ticker, percentShorted);
        }
    }
}
