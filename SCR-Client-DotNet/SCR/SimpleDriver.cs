using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR
{
	public class SimpleDriver : Controller
	{
		/* Gear Changing Constants */
		readonly int[] GearUp = { 5000, 6000, 6000, 6500, 7000, 0 };
		readonly int[] GearDown = { 0, 2500, 3000, 3000, 3500, 3500 };
		
		/* Stuck constants */
		readonly int StuckTime = 25;
		readonly float StuckAngle = 0.523598775f; // PI/6

		/* Accel and Brake Constants */
		readonly float MaxSpeedDist = 70;
		readonly float MaxSpeed = 150;
		readonly float Sin5 = 0.08716f;
		readonly float Cos5 = 0.99619f;
		
		/* Steering constants */
		readonly float SteerLock = 0.366519f;
		readonly float SteerSensitivityOffset = 80.0f;
		readonly float WheelSensitivityCoeff = 1;
		
		/* ABS Filter Constants */
		readonly float[] WheelRadius = { 0.3306f, 0.3306f, 0.3276f, 0.3276f };
		readonly float AbsSlip = 2.0f;
		readonly float AbsRange = 3.0f;
		readonly float AbsMinSpeed = 3.0f;
		
		/* Clutching Constants */
		readonly float ClutchMax = 0.5f;
		readonly float ClutchDelta = 0.05f;
		readonly float ClutchRange = 0.82f;
		readonly float ClutchDeltaTime = 0.02f;
		readonly float ClutchDeltaRaced = 10;
		readonly float ClutchDec = 0.01f;
		readonly float ClutchMaxModifier = 1.3f;
		readonly float ClutchMaxTime = 1.5f;

		private int stuck = 0;

		// current clutch
		private float clutch = 0;




		private int GetGear(ISensorModel model)
		{
			int gear = model.GetGear();
			double rpm = model.GetRPM();
			
			// if gear is 0 (N) or -1 (R) just return 1 
			if (gear < 1)
			{
				return 1;
			}

			// check if the RPM value of car is greater than the one suggested 
			// to shift up the gear from the current one     
			if (gear < 6 && rpm >= GearUp[gear - 1])
			{
				return gear + 1;
			}
			else
			{
				// check if the RPM value of car is lower than the one suggested 
				// to shift down the gear from the current one
				if (gear > 1 && rpm <= GearDown[gear - 1])
				{
					return gear - 1;
				}
				else  // otherwhise keep current gear
				{
					return gear;
				}
			}
		}

		private float GetSteer(ISensorModel model)
		{
			// steering angle is compute by correcting the actual car angle w.r.t. to track 
			// axis [sensors.getAngle()] and to adjust car position w.r.t to middle of track [sensors.getTrackPos()*0.5]
			float targetAngle = (float)(model.GetAngleToTrackAxis() - model.GetTrackPosition() * 0.5f);
			
			// at high speed reduce the steering command to avoid loosing the control
			if (model.GetSpeed() > SteerSensitivityOffset)
			{
				return (float)(targetAngle / (SteerLock * (model.GetSpeed() - SteerSensitivityOffset) * WheelSensitivityCoeff));
			}
			else
			{
				return (targetAngle) / SteerLock;
			}
		}

		private float GetAccel(ISensorModel model)
		{
			// checks if car is out of track
			if (model.GetTrackPosition() < 1 && model.GetTrackPosition() > -1)
			{
				// reading of sensor at +5 degree w.r.t. car axis
				float rxSensor = (float)model.GetTrackEdgeSensors()[10];
				// reading of sensor parallel to car axis
				float sensor = (float)model.GetTrackEdgeSensors()[9];
				// reading of sensor at -5 degree w.r.t. car axis
				float sxSensor = (float)model.GetTrackEdgeSensors()[8];

				float targetSpeed;

				// track is straight and enough far from a turn so goes to max speed
				if (sensor > MaxSpeedDist || (sensor >= rxSensor && sensor >= sxSensor))
				{
					targetSpeed = MaxSpeed;
				}
				else
				{
					// approaching a turn on right
					if (rxSensor > sxSensor)
					{
						float h = sensor * Sin5;
						float b = rxSensor - sensor * Cos5;
						float sinAngle = b * b / (h * h + b * b);
						// estimate the target speed depending on turn and on how close it is
						targetSpeed = MaxSpeed * (sensor * sinAngle / MaxSpeedDist);
					}
					// approaching a turn on left
					else
					{
						// computing approximately the "angle" of turn
						float h = sensor * Sin5;
						float b = sxSensor - sensor * Cos5;
						float sinAngle = b * b / (h * h + b * b);
						// estimate the target speed depending on turn and on how close it is
						targetSpeed = MaxSpeed * (sensor * sinAngle / MaxSpeedDist);
					}
				}
				// accel/brake command is exponentially scaled w.r.t. the difference between target speed and current one
				return (float)(2 / (1 + Math.Exp(model.GetSpeed() - targetSpeed)) - 1);
			}
			else
			{
				return 0.3f; // when out of track returns a moderate acceleration command
			}
		}

		public override Action Control(ISensorModel sensorModel)
		{
			// check if car is currently stuck
			if (Math.Abs(sensorModel.GetAngleToTrackAxis()) > StuckAngle)
			{
				// update stuck counter
				stuck++;
			}
			else
			{
				// if not stuck reset stuck counter
				stuck = 0;
			}

			// after car is stuck for a while apply recovering policy
			if (stuck > StuckTime)
			{
				/* set gear and sterring command assuming car is 
				 * pointing in a direction out of track */

				// to bring car parallel to track axis
				float steer = (float)(-sensorModel.GetAngleToTrackAxis() / SteerLock);
				int gear = -1; // Gear R

				// if car is pointing in the correct direction revert gear and steer  
				if (sensorModel.GetAngleToTrackAxis() * sensorModel.GetTrackPosition() > 0)
				{
					gear = 1;
					steer = -steer;
				}
				clutch = Clutching(sensorModel, clutch);
				// build a CarControl variable and return it
				Action action = new Action();
				action.Gear = gear;
				action.Steering = steer;
				action.Accelerate = 1.0f;
				action.Brake = 0;
				action.Clutch = clutch;
				return action;
			}
			else // car is not stuck
			{
				// compute accel/brake command
				float accelAndBrake = GetAccel(sensorModel);
				// compute gear 
				int gear = GetGear(sensorModel);
				// compute steering
				float steer = GetSteer(sensorModel);

				// normalize steering
				if (steer < -1)
				{
					steer = -1;
				}
				if(steer > 1)
				{
					steer = 1;
				}

				// set accel and brake from the joint accel/brake command 
				float accel, brake;
				if(accelAndBrake > 0)
				{
					accel = accelAndBrake;
					brake = 0;
				}
				else
				{
					accel = 0;
					// apply ABS to brake
					brake = FilterAbs(sensorModel, -accelAndBrake);
				}

				clutch = Clutching(sensorModel, clutch);

				// build a CarControl variable and return it
				Action action = new Action();
				action.Gear = gear;
				action.Steering = steer;
				action.Accelerate = accel;
				action.Brake = brake;
				action.Clutch = clutch;
				return action;
			}
		}

		private float FilterAbs(ISensorModel model, float brake)
		{
			// convert speed to m/s
			float speed = (float)(model.GetSpeed() / 3.6f);
			// when speed lower than min speed for abs do nothing
			if (speed < AbsMinSpeed)
			{
				return brake;
			}

			// compute the speed of wheels in m/s
			float slip = 0.0f;
			for(int i = 0; i < 4; i++)
			{
				slip += (float)(model.GetWheelSpinVelocity()[i] * WheelRadius[i]);
			}
			// slip is the difference between actual speed of car and average speed of wheels
			slip = speed - slip / 4.0f;
			// when slip too high apply ABS
			if (slip > AbsSlip)
			{
				brake = brake - (slip - AbsSlip) / AbsRange;
			}
			if(brake < 0)
			{
				return 0;
			}
			else
			{
				return brake;
			}
		}

		private float Clutching(ISensorModel model, float clutch)
		{

			float maxClutch = ClutchMax;
			// Check if the current situation is the race start
			if (model.GetCurrentLapTime() < ClutchDeltaTime && Stage_ == Stage.RACE && model.GetDistanceRaced() < ClutchDeltaRaced)
			{
				clutch = maxClutch;
			}

			// Adjust the current value of the clutch
			if (clutch > 0)
			{
				double delta = ClutchDelta;
				if(model.GetGear() < 2)
				{
					// Apply a stronger clutch output when the gear is one and the race is just started
					delta /= 2;
					maxClutch *= ClutchMaxModifier;
					if(model.GetCurrentLapTime() < ClutchMaxTime)
					{
						clutch = maxClutch;
					}
				}

				// Check clutch is not bigger than maximum values
				clutch = Math.Min(maxClutch, clutch);
				
				// If clutch is not at max value decrease it quite quickly
				if (clutch != maxClutch)
				{
					clutch -= (float)delta;
					clutch = Math.Max(0.0f, clutch);
				}
				// if clutch is at max value decrease it very slowly
				else
				{
					clutch -= ClutchDec;
				}
			}
			return clutch;
		}

		public new float[] InitAngles()
		{
			/* set angles as {-90,-75,-60,-45,-30,-20,-15,-10,-5,0,5,10,15,20,30,45,60,75,90} */
			float[] angles = new float[19];
			for(int i = 0; i < 5; i++)
			{
				angles[i] = -90 + i * 15;
				angles[18 - i] = 90 - i * 15;
			}
			for(int i = 5; i < 9; i++)
			{
				angles[i] = -20 + (i - 5) * 5;
				angles[18 - i] = 20 - (i - 5) * 5;
			}
			angles[9] = 0;
			return angles;
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
