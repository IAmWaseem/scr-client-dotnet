using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR
{
	class Program
	{
		private static int UDP_TIMEOUT = 10000;
		private static int port;
		private static string host;
		private static string clientId;
		private static bool verbose;
		private static int maxEpisodes;
		private static int maxSteps;
		private static Controller.Stage stage;
		private static string trackName;
		static void Main(string[] args)
		{
			ParseParameters(args);
			SocketHandler mySocket = new SocketHandler(host, port, verbose);
			string inMsg;
			Controller driver = Load(args[0]);
			driver.Stage_ = stage;
			driver.TrackName = trackName;
			
			/* Build init string */
			float[] angles = driver.InitAngles();
			string initStr = clientId + "(init";
			for (int i = 0; i < angles.Length; i++)
			{
				initStr = initStr + " " + angles[i].ToString(".0");
			}
			initStr = initStr + ")";
			long curEpisode = 0;
			bool shutdownOccured = false;
			do
			{
				/*
				 * Client identification
				 */
				do
				{
					mySocket.Send(initStr);
					inMsg = mySocket.Receive(UDP_TIMEOUT);
				} while (inMsg == null || inMsg.IndexOf("***identified***") < 0);

				/*
				 * Start to drive
				 */
				long currStep = 0;
				while (true)
				{
					/*
					 * Receives game state from TORCS
					 */
					inMsg = mySocket.Receive(UDP_TIMEOUT);
					if (inMsg != null)
					{                   
						/*
						 * Check if race is ended (shutdown)
						 */
						if (inMsg.IndexOf("***shutdown***") >= 0)
						{
							shutdownOccured = true;
							Console.WriteLine("Server Shutdown!");
							break;
						}
						/*
						 * Check if race is restarted
						 */
						if (inMsg.IndexOf("***restart***") >= 0)
						{
							driver.Reset();
							if (verbose)
							{
								Console.WriteLine("Server Restarting!");
							}
							break;
						}
						Action action = new Action();
						if (currStep < maxSteps || maxSteps == 0)
						{
							action = driver.Control(new MessageBasedSensorModel(inMsg));
						}
						else
						{
							action.RestartRace = true;
						}
						currStep++;
						mySocket.Send(action.ToString());
					}
					else
					{
						Console.WriteLine("Server did not respond within the timeout");
					}
				}
			} while (++curEpisode < maxEpisodes && !shutdownOccured);
			/*
			 * Shutdown the controller
			 */
			driver.Shutdown();
			mySocket.Close();
			Console.WriteLine("Client Shutdown.");
			Console.WriteLine("Bye, bye!");
		}

		private static void ParseParameters(string[] args)
		{
			/*
			 * Set default values for the options
			 */
			port = 3001;
			host = "127.0.0.1";
			clientId = "SCR";
			verbose = false;
			maxEpisodes = 1;
			maxSteps = 0;
			stage = Controller.Stage.UNKNOWN;
			trackName = "unknown";
			for(int i = 1; i < args.Length; i++)
			{
				string[] st = args[i].Split(':');
				string entity = st[0];
				string value = st[1];
				if(entity.Equals("port"))
				{
					port = int.Parse(value);
				}
				if(entity.Equals("host"))
				{
					host = value;
				}
				if(entity.Equals("id"))
				{
					clientId = value;
				}
				if(entity.Equals("verbose"))
				{
					if(value.Equals("on"))
					{
						verbose = true;
					}
					else if(value.Equals(false))
					{
						verbose = false;
					}
					else
					{
						Console.WriteLine(entity + ":" + value + " is not a valid option");
						// Close
					}
				}
				if(entity.Equals("id"))
				{
					clientId = value;
				}
				if(entity.Equals("stage"))
				{
					stage = GetStage(int.Parse(value));
				}
				if(entity.Equals("trackName"))
				{
					trackName = value;
				}
				if(entity.Equals("maxEpisodes"))
				{
					maxEpisodes = int.Parse(value);
					if(maxEpisodes <= 0)
					{
						Console.WriteLine(entity + ":" + value + " is not a valid option");
						// Close
					}
				}
				if(entity.Equals("maxSteps"))
				{
					maxSteps = int.Parse(value);
					if(maxSteps < 0)
					{
						Console.WriteLine(entity + ":" + value + " is not a valid option");
						// Close
					}
				}
			}
		}

		private static Controller.Stage GetStage(int stage)
		{
			switch(stage)
			{
				case 0:
					return Controller.Stage.WARMUP;
				case 1:
					return Controller.Stage.QUALIFYING;
				case 2:
					return Controller.Stage.RACE;
				default:
					return Controller.Stage.UNKNOWN;
			}
		}

		private static Controller Load(string name)
		{
			Controller controller = null;
			try
			{
				controller = (Controller)Activator.CreateInstance(Type.GetType(name));
			}
			catch(Exception ex)
			{
				Console.WriteLine(name + " is not a clas name");
			}
			return controller;
		}
	}
}
