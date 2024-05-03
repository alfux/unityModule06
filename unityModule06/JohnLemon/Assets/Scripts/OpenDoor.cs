using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public List<GameObject> keys = null;

    private Animator anim = null;

    void Start()
    {
        this.anim = this.GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider col)
    {
        PlayerController player;

        if (col.TryGetComponent<PlayerController>(out player))
        {
            if (player.HasKeys(this.keys) && !this.anim.enabled)
            {
                this.anim.enabled = true;
            }
        }
    }
}
