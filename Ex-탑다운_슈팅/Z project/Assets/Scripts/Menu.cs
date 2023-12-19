using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public GameObject mainMenuHolder;           // 메인 메뉴 오브젝트
    public GameObject optionMenuHolder;         // 설정 메뉴 오브젝트

    public Slider[] volumeSliders;              // 볼륨 슬라이더 배열
    public Toggle[] resolutionToggles;          // 해상도 토글 버튼 배열
    public int[] screenWidths;                  // 해상도 배열
    public Toggle fullScreenToggle;             // 전체화면 토글 버튼

    int activeScreenResIndex;                   // 활성화 중인 해상도 인덱스

    void Start() {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");              // 활성회 해상도 인덱스 가져오기
        bool isFullScreen = PlayerPrefs.GetInt("fullScreen") == 1 ? true : false;   // 전체화면 여부 가져오기

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;         // 마스터 볼륨 슬라이더 값 설정
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;          // 배경음악 볼륨 슬라이더 값 설정
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;            // 효과음 볼륨 슬라이더 값 설정

        for(int i = 0; i < resolutionToggles.Length; i++) {
            resolutionToggles[i].isOn = i == activeScreenResIndex;                  // 화면 해상도 토글 버튼 설정
        }

        fullScreenToggle.isOn = isFullScreen;                                       // 전체화면 설정 
    }

    // ■ 게임 시작 메소드
    public void Play() {
        SceneManager.LoadScene("Game");         // 게임 씬 로드
    }

    // ■ 게임 종료 메소드
    public void Quit() {
        Application.Quit();                     // 애플리케이션 종료
    }

    // ■ 설정 메뉴 이동 메소드
    public void OptionMenu() {
        mainMenuHolder.SetActive(false);        // 메인 메뉴 오브젝트 비활성화
        optionMenuHolder.SetActive(true);       // 설정 메뉴 오브젝트 활성화
    }

    // ■ 메인 메뉴 이동 메소드
    public void MainMenu() {
        mainMenuHolder.SetActive(true);         // 메인 메뉴 오브젝트 활성화
        optionMenuHolder.SetActive(false);      // 설정 메뉴 오브젝트 비활성화
    }

    // ■ 해상도 설정 메소드
    public void SetScreenResolution(int i) {
        if (resolutionToggles[i].isOn) {        // 해당 토글 버튼 활성화 시
            activeScreenResIndex = i;           // 활성화 해상도 인덱스 설정
            float aspectRatio = 16 / 9f;        // 세로 비율 설정
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);         // 화면 비율 설정
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);                               // 활성화 해상도 인덱스 저장
            PlayerPrefs.Save();                                                                         // 저장
        }
    }
    
    // ■ 전체화면 설정 메소드
    public void SetFullScreen(bool isFullScreen) {
        for(int i = 0; i < resolutionToggles.Length; i++) {                         // 해상도 토글 버튼 수만큼 반복
            resolutionToggles[i].interactable = !isFullScreen;                      // 해상도 토글 버튼 비활성화
        }

        if (isFullScreen) {                                                         // 전체화면 설정 체크 시
            Resolution[] allResolutions = Screen.resolutions;                       // 해상도 배열 생성(모든 해상도가 저장됨)
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];   // 최대 해상도 저장
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);  // 해상도 설정, 최대 해상도와 전체화면으로
        }
        else {                                                                      // 전체화면 설정 체크 안된 경우
            SetScreenResolution(activeScreenResIndex);                              // 활성화 해두었던 해상도로 재설정 
        }

        PlayerPrefs.SetInt("fullScreen", ((isFullScreen) ? 1 : 0));                 // 전체화면 여부 저장
        PlayerPrefs.Save();                                                         // 저장
    }

    // ■ 마스터 볼륨 설정 메소드
    public void SetMasterVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);   // 마스터 볼륨 설정
    }

    // ■ 음악 볼륨 설정 메소드
    public void SetMusicVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);    // 배경음악 볼륨 설정
    }

    // ■ 효과음 볼륨 설정 메소드
    public void SetSfxVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);      // 효과음 볼륨 설정
    }
}
