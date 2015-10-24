using UnityEngine;
using UnityOSC;
using System;
using System.Net;

public class DroneControlKeyboard : MonoBehaviour {

    public string Host = "127.0.0.1";
    public int Port = 12001;

    private OSCClient client;

    private bool isInAir = false, doingAflip = false;
    private float timer = 0, timerNoPress = 0;

    // Use this for initialization
    void Start () {
        client = new OSCClient(IPAddress.Parse(Host), Port);
    }

    // Update is called once per frame
    void Update () {

        timer += Time.deltaTime;
        timerNoPress += Time.deltaTime;

        if(!doingAflip)
        {
            SendToDrone("/stop", 0);

            float extraSpeed = IdiotsGame.instance.droneState == DroneStates.Angry ? 0.1f : 0;

            if ( Input.GetKey( KeyCode.LeftShift )) extraSpeed = 0.05f;

            if(timer > 0.01f){
                if( Input.GetKey(KeyCode.LeftArrow ) ){
                    timerNoPress = 0;
                    SendToDrone("/left", 0.04f + extraSpeed);
                }
                else if( Input.GetKey(KeyCode.RightArrow) ){
                    timerNoPress = 0;
                    SendToDrone("/right", 0.04f + extraSpeed);
                }else{
                    SendToDrone("/left", 0);
                    SendToDrone("/right", 0);
                }

                if( Input.GetKey(KeyCode.UpArrow) ){
                    timerNoPress = 0;
                    SendToDrone("/front", 0.02f + extraSpeed);
                }
                else if( Input.GetKey(KeyCode.DownArrow) ){
                    timerNoPress = 0;
                    SendToDrone("/back", 0.02f + extraSpeed);
                }else{
                    SendToDrone("/front", 0);
                    SendToDrone("/back", 0);
                }
            }

        }

        if( Input.GetKey(KeyCode.Comma) ){
            timerNoPress = 0;
            if(timer > 0.01f){
                SendToDrone("/counterClockwise", 0.5f);
            }
        }
        else if( Input.GetKeyUp(KeyCode.Comma) ){
            SendToDrone("/counterClockwise", 0);
        }
        if( Input.GetKey(KeyCode.Period) ){

            timerNoPress = 0;
            if(timer > 0.01f){
                SendToDrone("/clockwise", 0.5f);
            }
        }
        else if( Input.GetKeyUp(KeyCode.Period) ){
            SendToDrone("/clockwise", 0);
        }

        if( Input.GetKey(KeyCode.D) ){

            timerNoPress = 0;
            if(timer > 0.01f){
                SendToDrone("/down", 0.5f);
            }
        }
        else if( Input.GetKeyUp(KeyCode.D) ){
            SendToDrone("/down", 0);
        }

        if( Input.GetKey(KeyCode.U) ){

            timerNoPress = 0;
            if(timer > 0.01f){
                SendToDrone("/up", 0.5f);
            }
        }
        else if( Input.GetKeyUp(KeyCode.U) ){
            SendToDrone("/up", 0);
            SendToDrone("/stop", 0);
        }
        if( Input.GetKeyUp(KeyCode.Escape) ){
            isInAir = false;
            SendToDrone("/land", 1);
        }
        if( Input.GetKeyUp(KeyCode.F) ){

        }
        if( Input.GetKeyUp(KeyCode.W) ){
            doingAflip = true;
            SendToDrone("/wave", 1);
        }
        /*
        if(!doingAflip && timerNoPress > 0.5f){
            timerNoPress = 0;
            SendToDrone("/stop", 0);

        }
        */
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isInAir = false;
            SendToDrone("/land", 1);
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if(isInAir){
                isInAir = false;
                SendToDrone("/land", 1);
            }else{
                isInAir = true;
                SendToDrone("/startdrone", 1);
            }
        }

        if(timer > 0.2f){
            /*if(IdiotsGame.instance.droneState == DroneStates.Angry){
                SendToDrone("/red", 1);
            }
            else if(IdiotsGame.instance.droneState == DroneStates.Happy){
                SendToDrone("/green", 1);
            }*/
            timer = 0;
        }
    }
    public void MakeRed(){
            SendToDrone("/red", 1);
    }
    public void MakeGreen(){
            SendToDrone("/green", 1);
    }
    public void Flip(){
        if(!doingAflip){
            doingAflip  = true;
            SendToDrone("/flipBehind", 1);
            Wait.Until(1f, ()=>{
                doingAflip  = false;
            });
        }
    }
    // Update is called once per frame
    public void SendToDrone ( string address, object val ) {

        OSCMessage message = new OSCMessage(address, val);
        client.Send(message);

    }

    void OnApplicationQuit(){
        try{
            client.Close();
            }catch(Exception e){}
    }
}
