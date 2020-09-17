using UnityEngine;
using System.Collections;
using System;

public class SceneEvent
{
    public delegate void evenHandler();
    public event evenHandler OnEvent ;
    string eventName;

    public SceneEvent(string eventName)
    {
        //Debug.Log($"event {eventName} constructed");
        this.eventName = eventName;
    }

    public void CallEvent()
    {
        OnEvent?.Invoke();
    }

    public void UnSubscribeAll()
    {
        if (OnEvent != null)
        {
            //Debug.Log($"{eventName} unsubscribing.");
            Delegate[] clients = OnEvent.GetInvocationList();

            foreach (Delegate d in clients)
            {
                OnEvent -= (evenHandler)d;
            }
        }
    }
}