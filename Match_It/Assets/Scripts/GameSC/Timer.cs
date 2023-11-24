using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public AudioClip adgameover;
    public AudioSource audios;

    public GameObject GameCan;
    public GameObject End;
    public Text timerTxt;
    static public float time = 101f;
    static public float selectCountdown;

    void Awake()
    {
        audios.Play();
        selectCountdown = time;
    }

    void Update()
    {
        if (Mathf.Floor(selectCountdown) <= 0)
        {
            End.SetActive(true);
            selectCountdown = time;
            CreateTile.score = 0;
            GameCan.SetActive(false);
        }
        else
        {
            selectCountdown -= Time.deltaTime;
            timerTxt.text = Mathf.Floor(selectCountdown).ToString();
        }
    }

    public void BackGroundMusicOffButton() //¹è°æÀ½¾Ç Å°°í ²ô´Â ¹öÆ°
    {
        if (audios.isPlaying)
            audios.Pause();
        else
            audios.Play();
    }
}
