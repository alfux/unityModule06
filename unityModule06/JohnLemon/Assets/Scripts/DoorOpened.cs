using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpened : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator,
        AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject obj = animator.gameObject;

        GameObject.Destroy(obj.GetComponent<OpenDoor>());
        GameObject.Destroy(obj.GetComponent<Animator>());
    }
}
