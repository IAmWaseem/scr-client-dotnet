using System;
using System.Collections.Generic;
using System.Linq;

namespace SCR
{
	public class MessageParser
	{
		// Parses the message from the serverbot, and creates a table of
		// Associated names and values of the readings
		private Dictionary<string, object> table = new Dictionary<string, object>();
		private string message;

		public MessageParser(string message)
		{
			this.message = message;
			string[] words = this.message.Split('(');
			foreach (var word in words)
			{
				var reading = word;
				if (string.IsNullOrWhiteSpace(reading))
				{
					continue;
				}
				int endOfMessage = reading.IndexOf(')');
				if (endOfMessage > 0)
				{
					reading = reading.Substring(0, endOfMessage);
				}
				var rt = reading.Split(' ');
				if (rt.Count() < 2)
				{
					Console.WriteLine("Reading not recognized: " + reading);
				}
				else
				{
					string readingName = rt[0];
					object readingValue = "";
					if (readingName.Equals("opponents") || readingName.Equals("track") || readingName.Equals("wheelSpinVel") || readingName.Equals("focus"))
					{
						readingValue = new double[rt.Count()];
						int position = 0;
						for (int i = 1; i < rt.Count(); i++)
						{
							var nextToken = rt[i];
							try
							{
								((double[])readingValue)[position] = double.Parse(nextToken);
							}
							catch (Exception ex)
							{
								Console.WriteLine("Error parsing value '" + nextToken + "' for " + readingName + " using 0.0");
								Console.WriteLine("Message: " + message);
								((double[])readingValue)[position] = 0.0;
							}
							position++;
						}
					}
					else
					{
						string token = rt[1];
						try
						{
							if (readingName == "gear" || readingName == "racePos")
							{
								readingValue = new int();
								readingValue = int.Parse(token);
							}
							else
							{
								readingValue = double.Parse(token);
							}

						}
						catch (Exception e)
						{
							Console.WriteLine("Error parsing value '" + token + "' for " + readingName + " using 0.0");
							Console.WriteLine("Message: " + message);
							readingValue = 0.0f;
						}
					}
					table.Add(readingName, readingValue);
				}
			}
		}

		public void PrintAll()
		{
			foreach (var item in table)
			{
				Console.WriteLine(item.Key + ":  " + item.Value);
			}
		}

		public object GetReading(string key)
		{
			return table[key];
		}

		public string GetMessage()
		{
			return message;
		}
	}
}
