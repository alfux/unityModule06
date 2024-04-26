using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float cameraSensitivity = 1;
    public float walkAnimationSpeed = 1;
    public float turnAnimationSpeed = 1;

    private Animator anim = null;
    private Vector3 rotationPoint = Vector3.zero;
    private Vector3 lastPosition = Vector3.zero;
    private float walk = 0;
    private float turn = 0;
    private int walkID = 0;
    private int turnID = 0;

    private const float EPSILON = 0.1f;

    void Start()
    {
        Transform head = 
            this.transform.Find("Root/Hips/Spine1/Spine2/Neck/Head");

        if (head != null)
        {
            this.rotationPoint = head.transform.position
                - this.transform.position;
        }
        this.lastPosition = this.transform.position;
        this.anim = this.gameObject.GetComponent<Animator>();
        this.walkID = Animator.StringToHash("Walk");
        this.turnID = Animator.StringToHash("Turn");
        this.walk = 0;
        this.turn = 0;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Turn(Vector3 prevDir, Vector3 dir)
    {
        float theta = Mathf.Sign(Mathf.Asin(prevDir.z)) * Mathf.Acos(prevDir.x)
            - Mathf.Sign(Mathf.Asin(dir.z)) * Mathf.Acos(dir.x);

        theta = (Mathf.Sign(Mathf.PI - Mathf.Abs(theta)) * theta);
        this.transform.RotateAround(this.transform.position, Vector3.up, theta);
        this.turn = Mathf.Clamp(this.turn - this.turnAnimationSpeed
            * Time.deltaTime * theta / Mathf.PI, -1, 1);
    }

    void SmoothResetTurn()
    {
        this.turn = Mathf.Clamp(this.turn - this.turnAnimationSpeed / 2
            * Time.deltaTime * Mathf.Sign(this.turn), -1, 1);
        if (Mathf.Abs(this.turn) < EPSILON)
            this.turn = 0;
    }

    void SmoothResetWalk()
    {
        this.walk = Mathf.Clamp(this.walk - this.walkAnimationSpeed / 2
            * Time.deltaTime, 0, 1);
        if (Mathf.Abs(this.walk) < EPSILON)
            this.walk = 0;
    }

    void CharacterControls()
    {
        Vector3 forward = Vector3.ProjectOnPlane(
            Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 dir = Input.GetAxis("Vertical") * forward
            + Input.GetAxis("Horizontal") * Camera.main.transform.right;
        Vector3 prevDir = Vector3.ProjectOnPlane(
            this.transform.forward, Vector3.up).normalized;

        if (dir.magnitude > EPSILON)
        {
            if ((dir.normalized - prevDir).magnitude > EPSILON)
            {
                this.Turn(prevDir, dir.normalized);
            }
            else
            {
                this.SmoothResetTurn();
            }
            this.walk = Mathf.Clamp(this.walk + this.walkAnimationSpeed
                * Time.deltaTime * dir.magnitude, 0, 1);
        }
        else
        {
            this.SmoothResetWalk();
            this.SmoothResetTurn();
        }
    }

    void RotateCameraByAxis(Vector3 axis, float theta)
    {
        Vector3 point = this.transform.position + this.rotationPoint;

        Camera.main.transform.RotateAround(
            point,
            axis,
            theta * Time.deltaTime * this.cameraSensitivity
        );
        Camera.main.transform.LookAt(point, Vector3.up);
    }

    bool CameraInZone()
    {
        float dot = Vector3.Dot(Camera.main.transform.forward, Vector3.up);

        return (dot < 4 * EPSILON && dot > EPSILON - 1);
    }

    void CameraControls()
    {
        Vector3 deltaPos = this.transform.position - this.lastPosition;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (deltaPos.magnitude > Time.deltaTime)
        {
            Camera.main.transform.Translate(deltaPos, Space.World);
            this.lastPosition = this.transform.position;
        }
        if (mouseX != 0)
        {
            this.RotateCameraByAxis(Vector3.up, mouseX);
        }
        if (mouseY != 0)
        {
            this.RotateCameraByAxis(Camera.main.transform.right, -mouseY);
            if (!this.CameraInZone())
            {
                this.RotateCameraByAxis(Camera.main.transform.right, mouseY);
            }
        }
    }

    void UpdateAnimation()
    {
        this.anim.SetFloat(this.walkID, this.walk);
        this.anim.SetFloat(this.turnID, this.turn);
    }

    void Update()
    {
        this.CharacterControls();
        this.CameraControls();
        this.UpdateAnimation();
    }
}
