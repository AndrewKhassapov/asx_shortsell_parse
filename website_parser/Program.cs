using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace website_parser{
	
public class Program
{
	///<summary>URL to parse data from.</summary>
	const string FETCH_URL = "https://www.asx.com.au/data/shortsell.txt";
	
	
	public static void Main()
	{
		Console.WriteLine(ParseHTML());
	}
	
	protected static string ParseHTML(){
		string htmlBody = GetHTML().Result;
		string[] parsedHTMLArr = htmlBody.Split('\n');
		string parsedHTML = String.Empty;
		foreach(string i in parsedHTMLArr){
			parsedHTML += i + "\n";
		}
		
		return parsedHTML;
	}

	protected static async Task<String> GetHTML(){
	var client = new HttpClient();
	// Create the HttpContent for the form to be posted.
	var requestContent = new FormUrlEncodedContent(new [] {
    new KeyValuePair<string, string>("text", "Text block"),
	});

	// Get the response.
	HttpResponseMessage response = await client.PostAsync(FETCH_URL, requestContent);

	// Get the response content.
	HttpContent responseContent = response.Content;

	// Get the stream of the content.
	using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
		{
    	// Write the output.
    	return (await reader.ReadToEndAsync());
		}
	}
}

public class Data{
	protected string ticker{get; private set;}
	protected float percentShorted{get; private set;}
	protected float volume{get; private set;}
	protected float capital{get; private set;}
	
	public Data(string line){

	}

	protected string ReadStringAtIndex(string line, int index){
			// Set the regular expression for parsing digits and numbers.
			Regex regex = new Regex("([.,:a-zA-Z0-9_])");

            MatchCollection matches = regex.Matches(line);
			

			if (index < 0){
				index = matches.Count + index; 
			}

            if (index < matches.Count) {
                return matches[index].ToString();
            }
            return "No matches at line " + index.ToString();
        }

	///<summary>Prints data information as string</sumamry>
	public string Print(){
		return String.Empty;
	}
}
}
