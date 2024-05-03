using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSensitivity = 1;
    public PlayerController player = null;
    public GameObject model = null;

    [SerializeField] private Vector3 headHeight = Vector3.zero;
    [SerializeField] private Vector3 rotationPoint = Vector3.zero;
    [SerializeField] private Vector3 lastPosition = Vector3.zero;
    [SerializeField] private Vector3 lastContact = Vector3.zero;
    [SerializeField] private new SphereCollider collider = null;
    [SerializeField] private bool hitSomething = false;
    [SerializeField] private float distance = 0;
    [SerializeField] private bool firstPerson = false;
    [SerializeField] private bool autoFirstPerson = false;
    [SerializeField] private bool noRepeat = false;

    void Start()
    {
        Transform head = GameObject.Find("Head").transform;
        Transform headEnd = GameObject.Find("HeadEND").transform;

        this.cameraSensitivity *= 1000;
        this.headHeight = ((head.position + headEnd.position) / 2)
            - this.player.transform.position;
        this.rotationPoint = this.player.transform.position + this.headHeight;
        this.lastPosition = this.player.transform.position;
        this.collider = this.GetComponent<SphereCollider>();
        this.hitSomething = false;
        this.distance =
            (this.rotationPoint - this.transform.position).magnitude;
        this.firstPerson = false;
        this.autoFirstPerson = false;
        this.noRepeat = false;
    }

    void RotateCameraByAxis(Vector3 axis, float theta)
    {
        this.transform.RotateAround(
            this.rotationPoint,
            axis,
            theta * Time.deltaTime * this.cameraSensitivity
        );
    }

    bool FarFromLastContact()
    {
        Vector3 v = this.transform.position - this.lastContact;

        v = Vector3.Dot(this.transform.forward, v) * this.transform.forward - v;
        return (v.magnitude > 0.4);
    }

    void Reposition()
    {
        Vector3 deltaPos = this.player.transform.position - this.lastPosition;
        Vector3 current = this.rotationPoint - this.transform.position;
        float difference = this.distance - current.magnitude;
        int layerMask = ~((1 << 2) | (1 << 3) | (1 << 6));

        Physics.Raycast(this.transform.position, this.transform.forward,
            out RaycastHit forwardHit, Mathf.Infinity, layerMask);
        this.transform.Translate(deltaPos, Space.World);
        this.lastPosition = this.player.transform.position;
        if (this.hitSomething || forwardHit.distance < current.magnitude)
        {
            this.transform.Translate(
                5 * Time.deltaTime * this.transform.forward, Space.World
            );
        }
        else if (difference > 0.01 && this.FarFromLastContact())
        {
            this.transform.Translate(
                -5 * Time.deltaTime * this.transform.forward, Space.World
            );
        }
    }

    void TPControls()
    {
        float dot = Vector3.Dot(this.transform.forward, Vector3.up);
        float mouseX = Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1);
        float mouseY = Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1);

        this.rotationPoint = this.player.transform.position + this.headHeight;
        this.Reposition();
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
        this.transform.LookAt(this.rotationPoint, Vector3.up);
        if (!this.model.activeSelf)
        {
            this.model.SetActive(true);
        }
    }

    void FPControls()
    {
        float dot = Vector3.Dot(this.transform.forward, Vector3.up);
        float mouseX = Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1);
        float mouseY = Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1);

        this.rotationPoint = this.player.transform.position + this.headHeight;
        this.transform.position = this.rotationPoint;
        this.lastPosition = this.player.transform.position;
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
        if (this.model.activeSelf)
        {
            this.model.SetActive(false);
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
            if (!this.FarFromLastContact())
            {
                this.autoFirstPerson = true;
            }
            else
            {
                this.autoFirstPerson = false;
            }
        }
    }

    void Update()
    {
        this.ManageKeySwap();
        this.ManageAutoSwap();
        if (this.autoFirstPerson)
        {
            this.FPControls();
            this.Reposition();
        }
        else if (this.firstPerson)
        {
            this.FPControls();
        }
        else
        {
            this.TPControls();
        }
    }

    void OnCollisionStay(Collision col)
    {
        ContactPoint contact = col.GetContact(0);
    
        this.lastContact = contact.point;
        this.hitSomething = true;
    }

    void OnCollisionExit(Collision col)
    {
        this.hitSomething = false;
    }
}