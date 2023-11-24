using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayGameScene : MonoBehaviour
{
    public void LoadPlayGame()
    {
        SceneManager.LoadScene("PlayGame", LoadSceneMode.Single);
    }
}
