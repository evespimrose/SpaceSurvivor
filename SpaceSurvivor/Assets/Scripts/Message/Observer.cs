using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour
{
    protected List<Observer> observers = new List<Observer>();

    public void RegisterObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void NotifyObservers(IEventMessage message)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(message);
        }
    }
}

public abstract class Observer : MonoBehaviour
{
    public abstract void OnNotify(IEventMessage message);
}

