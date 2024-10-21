using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMessage : IEventMessage
{
    public GameObject Sender { get; private set; }
    public GameObject Target { get; private set; }
    public float Damage { get; private set; }

    public PlayerMessage(GameObject sender, GameObject target, float damage)
    {
        Sender = sender;
        Target = target;
        Damage = damage;
    }
}
