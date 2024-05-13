using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfStage : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        GameManager.GameOver(1);
        Time.timeScale = 0;
    }
}
