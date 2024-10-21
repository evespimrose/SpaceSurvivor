using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestEnemyMessage : IEventMessage
{
    public GameObject Sender { get; private set; }

    public ClosestEnemyMessage(GameObject sender)
    {
        Sender = sender;
    }
}
