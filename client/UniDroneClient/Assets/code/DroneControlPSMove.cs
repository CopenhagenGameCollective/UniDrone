using UnityEngine;
using UnityOSC;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;

public class DroneControlPSMove : MonoBehaviour
{
	
	public string Host = "127.0.0.1";
	public int droneIndex = 0;
	public Color MoveControllerColor = Color.blue;

	private int port = 12001;
	private OSCClient client;
	private bool isInAir = false, doingAflip = false;
	private float timer = 0, timerNoPress = 0;
	private float prevTrigger = 0;
	public float power = 1; //1f is max


	// We save a list of Move controllers statically, so we only have one list
	private static List<UniMoveController> moves = new List<UniMoveController>();
	
	// Use this for initialization
	void Awake ()
	{
		client = new OSCClient (IPAddress.Parse (Host), port + droneIndex);
		Time.maximumDeltaTime = 0.1f;
		int count = UniMoveController.GetNumConnected();
		Debug.Log (count);
		// Iterate through all connections (USB and Bluetooth)
		if (moves.Count == 0) {
			for (int i = 0; i < count; i++) {
				UniMoveController move = gameObject.AddComponent<UniMoveController> ();  // It's a MonoBehaviour, so we can't just call a constructor
			
				// Remember to initialize!
				if (!move.Init (i)) {
					Destroy (move);  // If it failed to initialize, destroy and continue on
					continue;
				}
			
				// This example program only uses Bluetooth-connected controllers
				PSMoveConnectionType conn = move.ConnectionType;
				if (conn == PSMoveConnectionType.Unknown || conn == PSMoveConnectionType.USB) {
					Destroy (move);
				} else {
					moves.Add (move);
					move.OnControllerDisconnected += HandleControllerDisconnected;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{

		if(droneIndex >= moves.Count) return;

		UniMoveController move = moves[droneIndex];

		timer += Time.deltaTime;
		timerNoPress += Time.deltaTime;

		// Trigger is held down, do control
		if (move.Trigger > 0.9f && !doingAflip) {
			move.SetLED (MoveControllerColor);
			if(!isInAir){
				isInAir = true;
				Debug.Log("Lift off");
				SendToNode("/startdrone", 1);
			}
			if(timer > 0.2f){
				if(move.Acceleration.x > 0.02f){
					SendToNode("/left", move.Acceleration.x * move.Acceleration.x * 2.2f * power);
				}
				else if(move.Acceleration.x < -0.02f){
					SendToNode("/right", (move.Acceleration.x * move.Acceleration.x * 2.2f *power));
				}else{
					SendToNode("/left", 0);
					SendToNode("/right", 0);
				}
				
				if(move.Acceleration.y < -0.02f){
					SendToNode("/front", move.Acceleration.y * move.Acceleration.y * 2.2f * power);
				}
				else if(move.Acceleration.y > 0.02f){
					SendToNode("/back", move.Acceleration.y * move.Acceleration.y * 2.2f * power );
				}else{
					SendToNode("/front", 0);
					SendToNode("/back", 0);
				}
			}
		}
		if(move.GetButtonDown(PSMoveButton.Square)){
			if(timer > 0.2f){
				SendToNode("/counterClockwise", 0.5f);
			}
		}
		else if(move.GetButtonUp(PSMoveButton.Square)){
			SendToNode("/counterClockwise", 0);
		}
		else if(move.GetButtonDown(PSMoveButton.Triangle)){
			if(timer > 0.2f){
				SendToNode("/clockwise", 0.5f);
			}
		}
		else if(move.GetButtonUp(PSMoveButton.Triangle)){
			SendToNode("/clockwise", 0);
		}
		
		else if(move.GetButtonDown(PSMoveButton.Cross)){
			if(timer > 0.2f){
				SendToNode("/down", 0.2f);
			}
		}
		else if(move.GetButtonUp(PSMoveButton.Cross)){
			SendToNode("/down", 0);
		}
		else if(move.GetButtonDown(PSMoveButton.Circle)){
			if(timer > 0.2f){
				SendToNode("/up", 0.2f);
			}
		}
		else if(move.GetButtonUp(PSMoveButton.Circle)){
			SendToNode("/up", 0);
			SendToNode("/stop", 0);
		}
		else if(move.GetButtonUp(PSMoveButton.Move)){
			SendToNode("/land", 1);
		}
		else if(move.GetButtonUp(PSMoveButton.Select)){
			SendToNode("/flip", 1);
		}
		else if(move.GetButtonUp(PSMoveButton.Start)){
			SendToNode("/wave", 1);
		}
		else if(move.Trigger <= 0.8f && !doingAflip){
			if(timer > 0.2f){
				SendToNode("/stop", 0);
				
			}
			move.SetLED(MoveControllerColor * 0.2f);
		}
		
		if(move.GetButtonUp(PSMoveButton.PS)){
			isInAir = false;
			//TODO: Make panic
			SendToNode("/land", 1);
		}
		prevTrigger = move.Trigger;
	}
	
	public void MakeRed ()
	{
		SendToNode ("/red", 1);
	}
	
	public void MakeGreen ()
	{
		SendToNode ("/green", 1);
	}
	
	public void Flip ()
	{
		if (!doingAflip) {
			doingAflip = true;
			SendToNode ("/flipBehind", 1);
			Wait.Until (1f, () => {
				doingAflip = false;
			});
		}
	}
	public void SendToNode (string address, object val)
	{
		try {
			OSCMessage message = new OSCMessage (address, val);
			client.Send (message);
		} catch (Exception e) {
		}

	}
	
	void OnApplicationQuit ()
	{
		try {
			client.Close ();
		} catch (Exception e) {
		}
	}

	void HandleControllerDisconnected(object sender, System.EventArgs e)
	{
		// We'd probably want to remove/destroy the controller here
		Debug.Log("Controller disconnected!");
		
		for (int i = 0; i < moves.Count; i++) {
			if(moves[i] == (UniMoveController)sender)
				moves.RemoveAt(i);
		}
	}
}