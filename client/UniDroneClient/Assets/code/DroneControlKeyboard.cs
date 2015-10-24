using UnityEngine;
using UnityOSC;
using System;
using System.Net;

public class DroneControlKeyboard : MonoBehaviour
{

	public string Host = "127.0.0.1";
	public int droneIndex = 0;
	private int port = 12001;
	private OSCClient client;
	private bool isInAir = false, doingAflip = false;
	private float timer = 0, timerNoPress = 0;

	// Use this for initialization
	void Start ()
	{
		client = new OSCClient (IPAddress.Parse (Host), port + droneIndex);
	}

	// Update is called once per frame
	void Update ()
	{

		timer += Time.deltaTime;
		timerNoPress += Time.deltaTime;

		if (!doingAflip) {
			SendToNode ("/stop", 0);

			float extraSpeed = 0;

			if (Input.GetKey (KeyCode.LeftShift))
				extraSpeed = 0.05f;

			if (timer > 0.01f) {
				if (Input.GetKey (KeyCode.LeftArrow)) {
					timerNoPress = 0;
					SendToNode ("/left", 0.04f + extraSpeed);
				} else if (Input.GetKey (KeyCode.RightArrow)) {
					timerNoPress = 0;
					SendToNode ("/right", 0.04f + extraSpeed);
				} else {
					SendToNode ("/left", 0);
					SendToNode ("/right", 0);
				}

				if (Input.GetKey (KeyCode.UpArrow)) {
					timerNoPress = 0;
					SendToNode ("/front", 0.02f + extraSpeed);
				} else if (Input.GetKey (KeyCode.DownArrow)) {
					timerNoPress = 0;
					SendToNode ("/back", 0.02f + extraSpeed);
				} else {
					SendToNode ("/front", 0);
					SendToNode ("/back", 0);
				}
			}

		}

		if (Input.GetKey (KeyCode.Comma)) {
			timerNoPress = 0;
			if (timer > 0.01f) {
				SendToNode ("/counterClockwise", 0.5f);
			}
		} else if (Input.GetKeyUp (KeyCode.Comma)) {
			SendToNode ("/counterClockwise", 0);
		}
		if (Input.GetKey (KeyCode.Period)) {

			timerNoPress = 0;
			if (timer > 0.01f) {
				SendToNode ("/clockwise", 0.5f);
			}
		} else if (Input.GetKeyUp (KeyCode.Period)) {
			SendToNode ("/clockwise", 0);
		}

		if (Input.GetKey (KeyCode.D)) {

			timerNoPress = 0;
			if (timer > 0.01f) {
				SendToNode ("/down", 0.5f);
			}
		} else if (Input.GetKeyUp (KeyCode.D)) {
			SendToNode ("/down", 0);
		}

		if (Input.GetKey (KeyCode.U)) {

			timerNoPress = 0;
			if (timer > 0.01f) {
				SendToNode ("/up", 0.5f);
			}
		} else if (Input.GetKeyUp (KeyCode.U)) {
			SendToNode ("/up", 0);
			SendToNode ("/stop", 0);
		}
		if (Input.GetKeyUp (KeyCode.Escape)) {
			isInAir = false;
			SendToNode ("/land", 1);
		}
		if (Input.GetKeyUp (KeyCode.F)) {

		}
		if (Input.GetKeyUp (KeyCode.W)) {
			doingAflip = true;
			SendToNode ("/wave", 1);
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			isInAir = false;
			SendToNode ("/land", 1);
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (isInAir) {
				isInAir = false;
				SendToNode ("/land", 1);
			} else {
				isInAir = true;
				SendToNode ("/startdrone", 1);
			}
		}
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
		OSCMessage message = new OSCMessage (address, val);
		client.Send (message);
	}

	void OnApplicationQuit ()
	{
		try {
			client.Close ();
		} catch (Exception e) {
		}
	}
}
