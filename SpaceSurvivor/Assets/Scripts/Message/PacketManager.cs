using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PacketManager : MonoBehaviour
{
    // Singleton instance
    public static PacketManager Instance { get; private set; }

    private Queue<IEventMessage> messageQueue = new Queue<IEventMessage>();
    private List<Observer> observers = new List<Observer>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer) 
    { 
        observers.Remove(observer); 
    }

    public void SendMessage(IEventMessage message)
    {
        messageQueue.Enqueue(message);
    }

    private void Update()
    {
        while (messageQueue.Count > 0)
        {
            IEventMessage message = messageQueue.Dequeue();
            for (int i = 0; i < observers.Count; i++)
            {
                observers[i].OnNotify(message);
            }
        }
    }
}
