using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gargoyle : MonoBehaviour
{
    public List<Ghost> ghosts = null;
    public float detectionFOV = 60;

    private Animator anim = null;
    private AudioSource alarm = null;
    private float detect = 0;

    void Start()
    {
        this.anim = this.GetComponent<Animator>();
        this.alarm = this.GetComponent<AudioSource>();
        this.detect = Mathf.Cos(Mathf.PI * this.detectionFOV / 360);
    }

    void OnTriggerStay(Collider col)
    {
        Vector3 playerDirection =
            (col.transform.position - this.transform.position).normalized;
        
        if (Vector3.Dot(playerDirection, this.transform.forward) >= this.detect)
        {
            this.anim.speed = 10;
            if (!this.alarm.isPlaying)
            {
                this.alarm.Play();
            }
            this.transform.forward = playerDirection;
            foreach (Ghost g in ghosts)
            {
                g.Chase(col.gameObject);
            }
        }
        else
        {
            this.anim.speed = 1;
            this.alarm.Stop();
        }
    }

    void OnTriggerExit(Collider col)
    {
        this.anim.speed = 1;
        this.alarm.Stop();
    }
}
