using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        PlayerController player;

        if (col.gameObject.TryGetComponent<PlayerController>(out player))
        {
            player.GetKey(this.gameObject);
            this.gameObject.SetActive(false);
        }
    }
}
