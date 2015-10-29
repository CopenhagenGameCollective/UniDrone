using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Performance : MonoBehaviour {

	public DroneControlPSMove server;

	public List<TimerInfo> timings;
	public Text txtStateName;
	public Text txtTime;

	public Text txtD1;
	public Text txtD2;
	
	public DroneControlPSMove d1Controller;
	public DroneControlPSMove d2Controller;

	public Transform timer;

	int currentIndex = 0;
	float currentTime;
	bool didStart;

	float pctLeft;

	void Awake()
	{
		for(int i = 1, c = timings.Count; i<c; i++)
		{
			timings[i-1].length = timings[i].GetTime()-timings[i-1].GetTime();
		}
	}
	void UpdateTime()
	{
		currentTime += Time.deltaTime;
		if(currentTime>timings[currentIndex+1].GetTime())
		{
			
			currentIndex++;
			timings[currentIndex].startTime = Time.time;
			Debug.Log("starting new state: "+currentIndex);  
		}
		server.SendToNode("/musicState",timings[currentIndex].musicState);
		Debug.Log("state: "+timings[currentIndex].musicState);
		pctLeft = timings[currentIndex].Pct(currentTime);
		timer.localScale = new Vector3(1f,Mathf.Clamp01(1f-pctLeft),1f);


		d1Controller.power = timings[currentIndex].D1Power/100f;
		d2Controller.power = timings[currentIndex].D2Power/100f;
	}

	void UpdateVisuals()
	{
		txtStateName.text = timings[currentIndex].name;
		int min = Mathf.FloorToInt(currentTime/60f);
		float sec = currentTime%60;

		txtTime.text = string.Format("{0:00} : {1:00} ", min, sec);

		txtD1.text = timings[currentIndex].drone1;
		txtD2.text = timings[currentIndex].drone2;

	}
	// Update is called once per frame
	void Update () {

		server.SendToNode("/musicStarted",didStart?1:0);
		if(!didStart && Input.GetKeyDown(KeyCode.Space))
		{
			currentTime = 0f;
			didStart = true;

			//TODO: Send msuic started!
		}
		if(Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(0);
		}
		
		if(didStart)
		{
			UpdateTime();
			UpdateVisuals();
		}
		
		
	}
}
