using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject globalPostProcess = null;

    private Canvas canvas = null;
    private Animator anim = null;
    private int hashCode = 0;

    private static GameManager instance = null;

    void Awake()
    {
        if (GameManager.instance != null)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            if (this.globalPostProcess != null)
            {
                this.globalPostProcess.SetActive(true);
            }
            this.canvas = this.GetComponent<Canvas>();
            this.anim = this.GetComponent<Animator>();
            this.hashCode = Animator.StringToHash("GameOver");
            GameManager.instance = this;
            SceneManager.sceneLoaded += this.RestartGame;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
    }

    void RestartGame(Scene scene, LoadSceneMode load)
    {
        if (this.canvas != null)
        {
            this.canvas.worldCamera = Camera.main;
            Time.timeScale = 1;
        }
    }

    public static void GameOver(int state)
    {
        GameManager.instance.anim.SetInteger(
            GameManager.instance.hashCode,
            state
        );
    }
}
