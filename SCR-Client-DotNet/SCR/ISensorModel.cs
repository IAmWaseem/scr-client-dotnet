namespace SCR
{
	public interface ISensorModel
	{
		double GetSpeed();
		double GetAngleToTrackAxis();
		double[] GetTrackEdgeSensors();
		double[] GetFocusSensors();
		double GetTrackPosition();
		int GetGear();

		double[] GetOpponentSensors();
		int GetRacePosition();
		double GetLateralSpeed();
		double GetCurrentLapTime();
		double GetDamage();
		double GetDistanceFromStartLine();
		double GetDistanceRaced();
		double GetFuelLevel();
		double GetLastLapTime();
		double GetRPM();
		double[] GetWheelSpinVelocity();
		double GetZSpeed();
		double GetZ();
		string GetMessage();
	}
}
