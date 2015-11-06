namespace SCR
{
	public abstract class Controller
	{
		public enum Stage
		{
			WARMUP,
			QUALIFYING,
			RACE,
			UNKNOWN
		}

		public Stage Stage_ { get; set; }
		public string TrackName { get; set; }

		public float[] InitAngles()
		{
			float[] angles = new float[19];
			for(int i = 0; i < 19; ++i)
			{
				angles[i] = -90 + i * 10;
			}
			return angles;
		}

		public abstract Action Control(ISensorModel sensorModel);
		public abstract void Reset(); // Called at the beginning of each new trial
		public abstract void Shutdown();
	}
}
