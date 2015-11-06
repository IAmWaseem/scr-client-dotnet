namespace SCR
{
	public class MessageBasedSensorModel : ISensorModel
	{
		private MessageParser messageParser;
		public MessageBasedSensorModel(MessageParser parser)
		{
			messageParser = parser;
		}

		public MessageBasedSensorModel(string message)
		{
			messageParser = new MessageParser(message);
		}
		public double GetAngleToTrackAxis()
		{
			return (double)messageParser.GetReading("angle");
		}

		public double GetCurrentLapTime()
		{
			return (double)messageParser.GetReading("curLapTime");
		}

		public double GetDamage()
		{
			return (double)messageParser.GetReading("damage");
		}

		public double GetDistanceFromStartLine()
		{
			return (double)messageParser.GetReading("distFromStart");
		}

		public double GetDistanceRaced()
		{
			return (double)messageParser.GetReading("distRaced");
		}

		public double[] GetFocusSensors()
		{
			return (double[])messageParser.GetReading("focus");
		}

		public double GetFuelLevel()
		{
			return (double)messageParser.GetReading("fuel");
		}

		public int GetGear()
		{
			return (int)messageParser.GetReading("gear");
		}

		public double GetLastLapTime()
		{
			return (double)messageParser.GetReading("lastLapTime");
		}

		public double GetLateralSpeed()
		{
			return (double)messageParser.GetReading("speedY");
		}

		public string GetMessage()
		{
			return messageParser.GetMessage();
		}

		public double[] GetOpponentSensors()
		{
			return (double[])messageParser.GetReading("opponents");
		}

		public int GetRacePosition()
		{
			return (int)messageParser.GetReading("racePos");
		}

		public double GetRPM()
		{
			return (double)messageParser.GetReading("rpm");
		}

		public double GetSpeed()
		{
			return (double)messageParser.GetReading("speedX");
		}

		public double[] GetTrackEdgeSensors()
		{
			return (double[])messageParser.GetReading("track");
		}

		public double GetTrackPosition()
		{
			return (double)messageParser.GetReading("trackPos");
		}

		public double[] GetWheelSpinVelocity()
		{
			return (double[])messageParser.GetReading("wheelSpinVel");
		}

		public double GetZ()
		{
			return (double)messageParser.GetReading("z");
		}

		public double GetZSpeed()
		{
			return (double)messageParser.GetReading("speedZ");
		}
	}
}
