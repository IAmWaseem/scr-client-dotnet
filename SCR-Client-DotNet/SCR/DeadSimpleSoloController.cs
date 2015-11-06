using System;

namespace SCR
{
	public class DeadSimpleSoloController : Controller
	{
		readonly double TargetSpeed = 15;
		public override Action Control(ISensorModel sensorModel)
		{
			Action action = new Action();
			if(sensorModel.GetSpeed() < TargetSpeed)
			{
				action.Accelerate = 1;
			}
			if(sensorModel.GetAngleToTrackAxis() < 0)
			{
				action.Steering = -0.1f;
			}
			else
			{
				action.Steering = 0.1f;
			}
			action.Gear = 1;
			return action;
		}

		public override void Reset()
		{
			Console.WriteLine("Restarting the Race!");
		}

		public override void Shutdown()
		{
			Console.WriteLine("Bye Bye!");
		}
	}
}
