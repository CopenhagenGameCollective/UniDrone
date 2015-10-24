using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

class WaitCaller : MonoBehaviour
{
    public float sinceAlive;

    public GameObject Caller;
    public float TimeToWait = float.MaxValue;
    public String FunctionToCall;
    public object Parameter;
	private bool pause = false;

    void Update()
    {
		if(pause) return;
        sinceAlive += Time.deltaTime;
        if (sinceAlive > TimeToWait)
        {
            if (Caller != null && Parameter != null) Caller.BroadcastMessage(FunctionToCall, Parameter, SendMessageOptions.DontRequireReceiver);
            else if (Caller != null) Caller.BroadcastMessage(FunctionToCall, SendMessageOptions.DontRequireReceiver);
			Stop();
        }
    }
	public void Stop(){
		Caller = null;
		TimeToWait = float.MaxValue;
        Destroy(gameObject);
	}
	
	public void Pause(){
		pause = true;
	}
	public void UnPause(){
		pause = false;
	}
}
class WaitCallerAction : MonoBehaviour
{
    public float sinceAlive;

    public float TimeToWait = float.MaxValue;
	public Action Action;
	private bool pause = false;

    void Update()
    {
		if(pause) return;
        sinceAlive += Time.deltaTime;
        if (sinceAlive > TimeToWait)
        {
            if (Action != null) Action(); 
			Stop();
        }
    }
	public void Stop(){
		Action = null;
		TimeToWait = float.MaxValue;
        Destroy(gameObject);
	}
	public void Pause(){
		pause = true;
	}
	public void UnPause(){
		pause = false;
	}
}

public delegate bool Condition(float elapsedSeconds);

public static class Wait
{
    public static GameObject Until(GameObject caller, float time, String functionToCall, object parameter)
    {
		GameObject go = new GameObject("Waiter");
		
        var w = go.AddComponent<WaitCaller>();
        w.Caller = caller;
        w.TimeToWait = time;
        w.FunctionToCall = functionToCall;
        w.Parameter = parameter;
		return go;
    }
    public static GameObject Until(GameObject caller, float time, String functionToCall)
    {
       return Wait.Until(caller, time, functionToCall, null);
    }
	
	public static GameObject Until(float time, Action action)
    {
        GameObject go = new GameObject("Waiter");
        var w = go.AddComponent<WaitCallerAction>();
        w.TimeToWait = time;
        w.Action = action;
		return go;
    }

}