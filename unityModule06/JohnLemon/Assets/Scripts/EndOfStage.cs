using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfStage : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        PlayerController player;

        if (col.TryGetComponent<PlayerController>(out player))
        {
            Debug.Log("You win!");
        }
    }
}
