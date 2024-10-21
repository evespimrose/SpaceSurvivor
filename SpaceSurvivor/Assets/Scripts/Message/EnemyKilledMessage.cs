using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKilledMessage : IEventMessage
{
    public GameObject Sender { get; private set; }

    public EnemyKilledMessage(GameObject enemy)
    {
        Sender = enemy;
    }
}

