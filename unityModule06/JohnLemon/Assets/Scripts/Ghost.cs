using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AlfuxMath;
using Unity.VisualScripting;

public class Ghost : MonoBehaviour
{
    public float randomAmplitude = 1;
    public float detectionFov = 60;
    public float maxChaseTime = 5;
    public List<Vector3> keyPositions = null;

    private delegate void Action();

    private NavMeshAgent nav = null;
    private Action behaviour = null;
    private Vector3 lastPosition = Vector3.zero;
    private int listIndex = 0;
    private float timer = 0;
    private bool isChasing = false;
    private GameObject target = null;

    void Start()
    {
        this.nav = this.GetComponent<NavMeshAgent>();
        this.detectionFov = Mathf.Cos(this.detectionFov * Mathf.PI / 360);
        if (this.keyPositions == null || this.keyPositions.Count == 0)
        {
            this.behaviour = RandomDestination;
            this.behaviour();
        }
        else
        {
            if (this.keyPositions.Count < 2)
            {
                this.keyPositions.Add(this.transform.position);
            }
            this.behaviour = Patrol;
            this.listIndex = 0;
            this.nav.SetDestination(this.keyPositions[this.listIndex]);
        }
        this.lastPosition = this.transform.position;
        this.timer = 0;
        this.isChasing = false;
        this.target = null;
    }

    void RandomDestination()
    {
        float theta = Random.value * Mathf.PI * 2;

        this.nav.SetDestination(
            this.transform.position
            + Random.value * this.randomAmplitude
            * new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta))
        );
    }

    void Patrol()
    {
        this.listIndex = (this.listIndex + 1) % this.keyPositions.Count;
        this.nav.SetDestination(this.keyPositions[this.listIndex]);
    }

    void Update()
    {
        if (this.nav.remainingDistance < 0.1 && !this.isChasing)
        {
            this.timer = 0;
            this.behaviour();
        }
        else if (this.isChasing)
        {
            this.Chase();
            this.timer += Time.deltaTime;
        }
    }

    void Chase()
    {
        if (this.timer < this.maxChaseTime)
        {
            if (!this.isChasing)
            {
                this.lastPosition = this.transform.position;
                this.isChasing = true;
            }
            this.nav.SetDestination(this.target.transform.position);
        }
        else if (this.isChasing)
        {
            this.nav.SetDestination(this.lastPosition);
            this.isChasing = false;
            this.target = null;
            this.listIndex = this.listIndex + this.keyPositions.Count - 1;
        }
    }

    void OnCollisionStay(Collision col)
    {
        GameManager.GameOver(-1);
        Time.timeScale = 0;
    }

    void OnTriggerStay(Collider col)
    {
        if (this.isChasing)
        {
            this.transform.LookAt(col.transform.position);
        }
        else if (this.ColIsSeen(ref col))
        {
            this.target = col.gameObject;
            this.Chase();
        }
    }

    bool ColIsSeen(ref Collider col)
    {
        return (
            Vector3.Dot(
                this.transform.forward,
                (col.transform.position - this.transform.position).normalized
            )
            > this.detectionFov
        );
    }

    public void Chase(GameObject target)
    {
        this.timer = 0;
        this.target = target;
        this.Chase();
    }
}
