using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : StateMachineBehaviour
{
    override public void OnStateExit(
        Animator animator, AnimatorStateInfo stateInfo, int layerIndex
    ) {
        GameManager.GameOver(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}