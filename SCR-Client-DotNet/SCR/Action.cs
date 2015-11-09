using System;

namespace SCR
{
	public class Action
	{
		public double Accelerate { get; set; }		     // 0..1
		public double Brake { get; set; }		        // 0..1
		public double Clutch { get; set; }		        // 0..1
		public int Gear { get; set; }			        // -1..6
		public double Steering { get; set; }	       // -1..1
		public bool RestartRace { get; set; }
		public int Focus { get; set; }			           // ML Desired focus angle in degrees [-90; 90], set to 360 if no focusing is desired!

		public Action()
		{
			Accelerate = 0;
			Brake = 0;
			Clutch = 0;
			Gear = 0;
			Steering = 0;
			RestartRace = false;
			Focus = 360;
		}
		
		public override string ToString()
		{
			LimitValues();
			return "(accel " + Accelerate + ") " +
			   "(brake " + Brake + ") " +
			   "(clutch " + Clutch + ") " +
			   "(gear " + Gear + ") " +
			   "(steer " + Steering + ") " +
			   "(meta " + (RestartRace ? 1 : 0)
			   + ") " + "(focus " + Focus //ML
			   + ")";
		}

		public void LimitValues()
		{
			Accelerate = Math.Max(0, Math.Min(1, Accelerate));
			Brake = Math.Max(0, Math.Min(1, Brake));
			Clutch = Math.Max(0, Math.Min(1, Clutch));
			Steering = Math.Max(-1, Math.Min(1, Steering));
			Gear = Math.Max(-1, Math.Min(6, Gear));
		}
	}
}
