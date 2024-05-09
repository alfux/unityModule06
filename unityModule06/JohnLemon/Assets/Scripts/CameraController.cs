using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlfuxMath;
using System;

public class CameraController : MonoBehaviour
{
    public float cameraSensitivity = 1;
    public PlayerController player = null;
    public GameObject model = null;

    [SerializeField]private new SphereCollider collider = null;
    [SerializeField]private Vector3 headHeight = Vector3.zero;
    [SerializeField]private Vector3 rotationPoint = Vector3.zero;
    [SerializeField]private Vector3 lastPlayerPos = Vector3.zero;
    [SerializeField]private Vector3 lastContact = Vector3.zero;
    [SerializeField]private Vector3 lastContactNormal = Vector3.zero;
    [SerializeField]private float mouseX = 0;
    [SerializeField]private float mouseY = 0;
    [SerializeField]private float distance = 0;
    [SerializeField]private float rayon = 0;
    [SerializeField]private bool hitSomething = false;
    [SerializeField]private bool firstPerson = false;
    [SerializeField]private bool autoFirstPerson = false;
    [SerializeField]private bool noRepeat = false;

    void Start()
    {
        Transform head = GameObject.Find("Head").transform;
        Transform headEnd = GameObject.Find("HeadEND").transform;

        this.cameraSensitivity *= 1000;
        this.collider = this.GetComponent<SphereCollider>();
        this.headHeight = 
            ((head.position + headEnd.position) / 2)
            - this.player.transform.position;
        this.rotationPoint = this.player.transform.position + this.headHeight;
        this.lastPlayerPos = this.player.transform.position;
        this.lastContact = Vector3.zero;
        this.lastContactNormal = Vector3.zero;
        this.mouseX = 0;
        this.mouseY = 0;
        this.distance =
            (this.rotationPoint - this.transform.position).magnitude;
        this.rayon = Vector3.Distance(
            this.rotationPoint, this.transform.position
        );
        this.hitSomething = false;
        this.firstPerson = false;
        this.autoFirstPerson = false;
        this.noRepeat = false;
    }

    void Update()
    {
        this.ManageKeySwap();
        this.ManageAutoSwap();
        if (this.firstPerson || this.autoFirstPerson)
        {
            this.FPControls();
        }
        else
        {
            this.TPControls();
        }
    }

    void ManageKeySwap()
    {
        if (this.noRepeat)
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                this.noRepeat = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            this.firstPerson = !this.firstPerson;
            this.noRepeat = true;
        }
    }

    void ManageAutoSwap()
    {
        if ((this.rotationPoint - this.transform.position).magnitude < 0.4)
        {
            if (this.model.activeSelf)
            {
                this.model.SetActive(false);
            }
            if (!this.FarFromLastContact())
            {
                this.autoFirstPerson = true;
            }
            else
            {
                this.autoFirstPerson = false;
            }
        }
        else if (!this.model.activeSelf)
        {
            this.model.SetActive(true);
        }
    }

    void FPControls()
    {
        float dot = Vector3.Dot(this.transform.forward, Vector3.up);
        this.mouseX = Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1);
        this.mouseY = Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1);

        this.rotationPoint = this.player.transform.position + this.headHeight;
        this.transform.position = this.rotationPoint;
        this.lastPlayerPos = this.player.transform.position;
        if (mouseX != 0)
        {
            this.RotateCameraByAxis(Vector3.up, mouseX);
        }
        if ((mouseY > 0 && dot < 0.9) || (mouseY < 0 && dot > -0.9))
        {
            this.RotateCameraByAxis(this.transform.right, -mouseY);
            dot = Vector3.Dot(this.transform.forward, Vector3.up);
            if (dot > 0.9 || dot < -0.9)
            {
                this.RotateCameraByAxis(this.transform.right, mouseY);
            }
        }
    }

    void RotateCameraByAxis(Vector3 axis, float theta)
    {
        this.transform.RotateAround(
            this.rotationPoint,
            axis,
            theta * Time.deltaTime * this.cameraSensitivity
        );
    }

    void TPControls()
    {
        Vector3 deltaPos = this.player.transform.position - this.lastPlayerPos;
        float dot = Vector3.Dot(this.transform.forward, Vector3.up);
        this.mouseX = Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1);
        this.mouseY = Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1);

        this.rotationPoint = this.player.transform.position + this.headHeight;
        this.transform.Translate(deltaPos, Space.World);
        this.lastPlayerPos = this.player.transform.position;
        if (mouseX != 0)
        {
            this.RotateCameraByAxis(Vector3.up, mouseX);
        }
        if ((mouseY > 0 && dot < 0.4) || (mouseY < 0 && dot > -0.6))
        {
            this.RotateCameraByAxis(this.transform.right, -mouseY);
            dot = Vector3.Dot(this.transform.forward, Vector3.up);
            if (dot > 0.4 || dot < -0.6)
            {
                this.RotateCameraByAxis(this.transform.right, mouseY);
            }
        }
        this.Reposition();
        this.transform.LookAt(this.rotationPoint, Vector3.up);
    }

    void Reposition()
    {
        if (this.hitSomething)
        {
            this.AvoidCollision();
        }
        this.rayon = Vector3.Distance(
            this.rotationPoint,
            this.transform.position
        );
        Physics.Raycast(
            this.rotationPoint,
            -this.transform.forward,
            out RaycastHit obstacle,
            Mathf.Infinity,
            ~((1 << 2) | (1 << 3) | (1 << 6))
        );
        if (obstacle.distance < this.rayon)
        {
            this.CrossObstacle(obstacle);
            this.rayon = Vector3.Distance(
                this.rotationPoint,
                this.transform.position
            );
        }
        this.StayInMaxRange();
    }

    void AvoidCollision()
    {
        AxisPlaneIntersection solve = new(
            this.transform.position,
            this.transform.forward,
            this.lastContact + this.lastContactNormal * this.collider.radius,
            this.lastContactNormal
        );
        if (
            !float.IsNaN(solve.Solution.x) &&
            !float.IsNaN(solve.Solution.y) &&
            !float.IsNaN(solve.Solution.z)
        )
        {
            this.transform.Translate(
                solve.Solution - this.transform.position,
                Space.World
            );
        }
    }

    void CrossObstacle(RaycastHit obstacle)
    {
        float distanceToCross =
            this.collider.radius
            / Mathf.Abs(
                Mathf.Sin(
                    (Mathf.PI / 2)
                    - Mathf.Acos(
                        Mathf.Abs(
                            Vector3.Dot(
                                obstacle.normal,
                                this.transform.forward
                            )
                        )
                    )
                )
            )
        ;
        this.transform.Translate(
            obstacle.point - this.transform.position
            + distanceToCross * this.transform.forward,
            Space.World
        );
    }   

    void StayInMaxRange()
    {
        float difference = this.distance - this.rayon;

        if (difference > 0.01)
        {
            if (this.FarFromLastContact())
            {
                this.transform.Translate(
                    -5 * Time.deltaTime * this.transform.forward, Space.World
                );
            }
        }
        else if (difference < -0.01)
        {
            this.transform.Translate(
                -difference * this.transform.forward, Space.World
            );
        }
    }

    bool FarFromLastContact()
    {
        Vector3 v = this.transform.position - this.lastContact;

        v = Vector3.Dot(this.transform.forward, v) * this.transform.forward - v;
        return (v.magnitude > 0.4);
    }

    void OnCollisionStay(Collision col)
    {
        ContactPoint[] allContacts = new ContactPoint[col.contactCount];
    
        this.lastContact = Vector3.zero;
        col.GetContacts(allContacts);
        foreach (ContactPoint contact in allContacts)
        {
            this.lastContact += contact.point;
        }
        this.lastContact /= allContacts.Length;
        this.lastContactNormal = allContacts[0].normal;
        this.hitSomething = true;
    }

    void OnCollisionExit(Collision col)
    {
        this.hitSomething = false;
    }
}