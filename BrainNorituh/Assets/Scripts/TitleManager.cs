using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleManager : MonoBehaviour
{
    public GameObject OptionPanel;
    public GameObject CreditPanel;

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

    // ■ 게임 종료 메소드
    public void Quit()
    {
        Application.Quit();                     // 애플리케이션 종료
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
    public void SetSfxVolume() {
        AudioManager.instance.SetVolume(OptionSliders[2].value, AudioManager.AudioChannel.Sfx);      // 효과음 볼륨 설정
    }

    public void LoadLobby()
    {
        SceneManager.LoadScene("Lobby");

        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);
    }

    public void OptionToggle()
    {
        if (OptionPanel.activeSelf == false) OptionPanel.SetActive(true);
        else OptionPanel.SetActive(false);

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

    public void SetOptionValueText(int num)
    {
        int real_num = num - 1;
        OptionValueTexts[real_num].text = ((int)(OptionSliders[real_num].value * 100)).ToString();
    }
}
