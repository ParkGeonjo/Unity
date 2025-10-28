using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public GameObject OptionCanvas;
    public GameObject CreditPanel;
    public GameObject Game1Panel;
    public GameObject Game2Panel;
    public GameObject Game3Panel;
    public GameObject[] Game1Score;                 // 게임 점수 표시 스프라이트 (기본 별)
    public GameObject[] Game2Score;                 // 게임 점수 표시 스프라이트 (기본 별)
    public GameObject[] Game3Score;                 // 게임 점수 표시 스프라이트 (기본 별)
    public Sprite GameScoreYellow;                 // 게임 점수 표시 스프라이트 (정답 별)

    public Slider[] OptionSliders;
    public TextMeshProUGUI[] OptionValueTexts;

    public AudioClip Click;

    void Start()
    {
        AudioManager.instance.LoadVolume();
        OptionSliders[0].value = AudioManager.instance.masterVolumePercent;         // 마스터 볼륨 슬라이더 값 설정
        OptionSliders[1].value = AudioManager.instance.musicVolumePercent;          // 배경음악 볼륨 슬라이더 값 설정
        OptionSliders[2].value = AudioManager.instance.sfxVolumePercent;            // 효과음 볼륨 슬라이더 값 설정
    }

    public void LoadGame1()
    {
        SceneManager.LoadScene("Game1");
    }

    public void LoadGame2()
    {
        SceneManager.LoadScene("Game2");
    }

    public void LoadGame3()
    {
        SceneManager.LoadScene("Game3");
    }

    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }

    public void OptionToggle()
    {
        if (OptionCanvas.activeSelf == false) OptionCanvas.SetActive(true);
        else OptionCanvas.SetActive(false);

        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);
    }

    public void CreditToggle()
    {
        if (CreditPanel.activeSelf == false) CreditPanel.SetActive(true);
        else CreditPanel.SetActive(false);

        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);
    }

    public void Game1Toggle()
    {
        if (Game1Panel.activeSelf == false) {
            Game1Panel.SetActive(true);
            int game1Score = PlayerPrefs.GetInt("game1 score", 0);
            for(int i = 0; i < game1Score; i++) {
                Game1Score[i].GetComponent<Image>().sprite = GameScoreYellow;
            }
        }
        else Game1Panel.SetActive(false);

        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);
    }

    public void Game2Toggle()
    {
        if (Game2Panel.activeSelf == false) {
            Game2Panel.SetActive(true);
            int game2Score = PlayerPrefs.GetInt("game2 score", 0);
            for(int i = 0; i < game2Score; i++) {
                Game2Score[i].GetComponent<Image>().sprite = GameScoreYellow;
            }
        }
        else Game2Panel.SetActive(false);

        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);
    }

    public void Game3Toggle()
    {
        if (Game3Panel.activeSelf == false) {
            Game3Panel.SetActive(true);
            int game3Score = PlayerPrefs.GetInt("game3 score", 0);
            for(int i = 0; i < game3Score; i++) {
                Game3Score[i].GetComponent<Image>().sprite = GameScoreYellow;
            }
        }
        else Game3Panel.SetActive(false);

        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);
    }

    public void SetOptionValueText(int num)
    {
        int real_num = num - 1;
        OptionValueTexts[real_num].text = ((int)(OptionSliders[real_num].value * 100)).ToString();
    }

    // ■ 마스터 볼륨 설정 메소드
    public void SetMasterVolume()
    {
        AudioManager.instance.SetVolume(OptionSliders[0].value, AudioManager.AudioChannel.Master);   // 마스터 볼륨 설정
    }

    // ■ 음악 볼륨 설정 메소드
    public void SetMusicVolume()
    {
        AudioManager.instance.SetVolume(OptionSliders[1].value, AudioManager.AudioChannel.Music);    // 배경음악 볼륨 설정
    }

    // ■ 효과음 볼륨 설정 메소드
    public void SetSfxVolume()
    {
        AudioManager.instance.SetVolume(OptionSliders[2].value, AudioManager.AudioChannel.Sfx);      // 효과음 볼륨 설정
    }
}
