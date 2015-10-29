using UnityEngine;
using System.Collections;

[System.Serializable]
public class TimerInfo {

	public string name;
	//getting the timing like this from the composer
	public float min; 
	public float sec;
	public float ms;

	public float D1Power; //max flight power, for letting the program control our performance
	public float D2Power;

	public int musicState; //controlling the music track
	public string drone1; //instruction for drones
	public string drone2;
	public float length; //precalculated on startup

	public float startTime; //should be set when starting

	public float GetTime()
	{
		return min*60f+sec+ms/1000f;
	}
	
	public float Pct(float time)
	{
		return (time-startTime)/length;
	}
		
}
