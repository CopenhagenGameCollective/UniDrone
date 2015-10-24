using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneControl : MonoBehaviour {


    public static float[] speedLimiter = {1,1};

    public string Host = "127.0.0.1";
    public int Port = 12001;

    public int index = 0;
    private Color[] colors = {Color.blue, Color.red };
    private string[] controllernames = {"/d0_", "/d1_"};

    private OSC.NET.OSCMessage Message;
    private OSC.NET.OSCTransmitter Transmitter;

    private bool[] isInAir = {false,false}, doingAflip = {false,false};
    private float[] prevTrigger = {0, 0};


    // We save a list of Move controllers.
    private static List<UniMoveController> moves = new List<UniMoveController>();


	// Use this for initialization
	void Start () {


	   Transmitter = new OSC.NET.OSCTransmitter( Host, Port );

        Time.maximumDeltaTime = 0.1f;
        int count = UniMoveController.GetNumConnected();
        // Iterate through all connections (USB and Bluetooth)
        if(moves.Count == 0)
            for (int i = 0; i < count; i++)
            {
                UniMoveController move = gameObject.AddComponent<UniMoveController>();  // It's a MonoBehaviour, so we can't just call a constructor

                // Remember to initialize!
                if (!move.Init(i))
                {
                    Destroy(move);  // If it failed to initialize, destroy and continue on
                    continue;
                }

                // This example program only uses Bluetooth-connected controllers
                PSMoveConnectionType conn = move.ConnectionType;
                if (conn == PSMoveConnectionType.Unknown || conn == PSMoveConnectionType.USB)
                {
                    Destroy(move);
                }
                else
                {
                    moves.Add(move);
                    move.OnControllerDisconnected += HandleControllerDisconnected;
                }
            }
	}
    private float timer = 0;
	// Update is called once per frame
	void Update () {
        if(moves.Count <= index) return;

        UniMoveController move = moves[index];
        timer += Time.deltaTime;

        if(move.Trigger > 0.9f && !doingAflip[index])
        {
            move.SetLED(colors[index]);
            if(!isInAir[index]){
                isInAir[index] = true;
                Debug.Log("Lift off");
                SendToNode(controllernames[index]+"startdrone", 1);

            }
            if(timer > 0.2f){
               if(move.Acceleration.x > 0.01f){
                    SendToNode(controllernames[index]+"left", move.Acceleration.x * move.Acceleration.x * 2 * speedLimiter[index]);
                }
                else if(move.Acceleration.x < -0.01f){
                    SendToNode(controllernames[index]+"right", (move.Acceleration.x * move.Acceleration.x * 2 * speedLimiter[index]));
                }else{
                    SendToNode(controllernames[index]+"left", 0);
                    SendToNode(controllernames[index]+"right", 0);
                }

                if(move.Acceleration.y < -0.01f){
                    SendToNode(controllernames[index]+"front", move.Acceleration.y * move.Acceleration.y * 2 * speedLimiter[index]);
                }
                else if(move.Acceleration.y > 0.01f){
                    SendToNode(controllernames[index]+"back", move.Acceleration.y * move.Acceleration.y * 2 * speedLimiter[index]);
                }else{
                    SendToNode(controllernames[index]+"front", 0);
                    SendToNode(controllernames[index]+"back", 0);
                }
            }

        }

        if(move.IsButtonDown(PSMoveButton.Square)){
            if(timer > 0.2f){
                SendToNode(controllernames[index]+"counterClockwise", 0.5f);
            }
        }
        else if(move.IsButtonReleased(PSMoveButton.Square)){
            SendToNode(controllernames[index]+"counterClockwise", 0);
        }
        else if(move.IsButtonDown(PSMoveButton.Triangle)){
            if(timer > 0.2f){
                SendToNode(controllernames[index]+"clockwise", 0.5f);
            }
        }
        else if(move.IsButtonReleased(PSMoveButton.Triangle)){
            SendToNode(controllernames[index]+"clockwise", 0);
        }

        else if(move.IsButtonDown(PSMoveButton.Cross)){
            if(timer > 0.2f){
                SendToNode(controllernames[index]+"down", 0.5f);
            }
        }
        else if(move.IsButtonReleased(PSMoveButton.Cross)){
            SendToNode(controllernames[index]+"down", 0);
        }
        else if(move.IsButtonDown(PSMoveButton.Circle)){
            if(timer > 0.2f){
                SendToNode(controllernames[index]+"up", 0.5f);
            }
        }
        else if(move.IsButtonReleased(PSMoveButton.Circle)){
            SendToNode(controllernames[index]+"up", 0);
            SendToNode(controllernames[index]+"stop", 0);
        }
        else if(move.IsButtonReleased(PSMoveButton.Move)){
            isInAir[index] = false;
            SendToNode(controllernames[index]+"land", 1);
        }
        else if(move.IsButtonReleased(PSMoveButton.Select)){
            SendToNode(controllernames[index]+"flip", 1);
        }
        else if(move.IsButtonReleased(PSMoveButton.Start)){
            SendToNode(controllernames[index]+"wave", 1);
        }
        else if(move.Trigger <= 0.8f && !doingAflip[index]){
            if(timer > 0.2f){
                SendToNode(controllernames[index]+"stop", 0);

            }
            move.SetLED(Color.black);
        }

        if(move.IsButtonReleased(PSMoveButton.PS)){
            isInAir[index] = false;
            //TODO: Make panic
            SendToNode(controllernames[index]+"land", 1);
        }
        prevTrigger[index] = move.Trigger ;


        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(isInAir[index]){
                isInAir[index] = false;
                SendToNode(controllernames[index]+"land", 1);
            }else{
                isInAir[index] = true;
                SendToNode(controllernames[index]+"startdrone", 1);
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isInAir[index] = false;
            SendToNode(controllernames[index]+"land", 1);
        }
        if(timer > 0.2f){
            timer = 0;
        }
	}

    // Update is called once per frame
    public void SendToNode ( string address, object val ) {
        //Debug.Log("sending stuff to node " + address + " " + val);

        Message = new OSC.NET.OSCMessage( address, val );
        Transmitter.Send( Message );

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
