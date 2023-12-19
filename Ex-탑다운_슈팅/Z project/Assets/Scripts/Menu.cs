using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public GameObject mainMenuHolder;           // ���� �޴� ������Ʈ
    public GameObject optionMenuHolder;         // ���� �޴� ������Ʈ

    public Slider[] volumeSliders;              // ���� �����̴� �迭
    public Toggle[] resolutionToggles;          // �ػ� ��� ��ư �迭
    public int[] screenWidths;                  // �ػ� �迭
    public Toggle fullScreenToggle;             // ��üȭ�� ��� ��ư

    int activeScreenResIndex;                   // Ȱ��ȭ ���� �ػ� �ε���

    void Start() {
        activeScreenResIndex = PlayerPrefs.GetInt("screen res index");              // Ȱ��ȸ �ػ� �ε��� ��������
        bool isFullScreen = PlayerPrefs.GetInt("fullScreen") == 1 ? true : false;   // ��üȭ�� ���� ��������

        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;         // ������ ���� �����̴� �� ����
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;          // ������� ���� �����̴� �� ����
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;            // ȿ���� ���� �����̴� �� ����

        for(int i = 0; i < resolutionToggles.Length; i++) {
            resolutionToggles[i].isOn = i == activeScreenResIndex;                  // ȭ�� �ػ� ��� ��ư ����
        }

        fullScreenToggle.isOn = isFullScreen;                                       // ��üȭ�� ���� 
    }

    // �� ���� ���� �޼ҵ�
    public void Play() {
        SceneManager.LoadScene("Game");         // ���� �� �ε�
    }

    // �� ���� ���� �޼ҵ�
    public void Quit() {
        Application.Quit();                     // ���ø����̼� ����
    }

    // �� ���� �޴� �̵� �޼ҵ�
    public void OptionMenu() {
        mainMenuHolder.SetActive(false);        // ���� �޴� ������Ʈ ��Ȱ��ȭ
        optionMenuHolder.SetActive(true);       // ���� �޴� ������Ʈ Ȱ��ȭ
    }

    // �� ���� �޴� �̵� �޼ҵ�
    public void MainMenu() {
        mainMenuHolder.SetActive(true);         // ���� �޴� ������Ʈ Ȱ��ȭ
        optionMenuHolder.SetActive(false);      // ���� �޴� ������Ʈ ��Ȱ��ȭ
    }

    // �� �ػ� ���� �޼ҵ�
    public void SetScreenResolution(int i) {
        if (resolutionToggles[i].isOn) {        // �ش� ��� ��ư Ȱ��ȭ ��
            activeScreenResIndex = i;           // Ȱ��ȭ �ػ� �ε��� ����
            float aspectRatio = 16 / 9f;        // ���� ���� ����
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);         // ȭ�� ���� ����
            PlayerPrefs.SetInt("screen res index", activeScreenResIndex);                               // Ȱ��ȭ �ػ� �ε��� ����
            PlayerPrefs.Save();                                                                         // ����
        }
    }
    
    // �� ��üȭ�� ���� �޼ҵ�
    public void SetFullScreen(bool isFullScreen) {
        for(int i = 0; i < resolutionToggles.Length; i++) {                         // �ػ� ��� ��ư ����ŭ �ݺ�
            resolutionToggles[i].interactable = !isFullScreen;                      // �ػ� ��� ��ư ��Ȱ��ȭ
        }

        if (isFullScreen) {                                                         // ��üȭ�� ���� üũ ��
            Resolution[] allResolutions = Screen.resolutions;                       // �ػ� �迭 ����(��� �ػ󵵰� �����)
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];   // �ִ� �ػ� ����
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);  // �ػ� ����, �ִ� �ػ󵵿� ��üȭ������
        }
        else {                                                                      // ��üȭ�� ���� üũ �ȵ� ���
            SetScreenResolution(activeScreenResIndex);                              // Ȱ��ȭ �صξ��� �ػ󵵷� �缳�� 
        }

        PlayerPrefs.SetInt("fullScreen", ((isFullScreen) ? 1 : 0));                 // ��üȭ�� ���� ����
        PlayerPrefs.Save();                                                         // ����
    }

    // �� ������ ���� ���� �޼ҵ�
    public void SetMasterVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);   // ������ ���� ����
    }

    // �� ���� ���� ���� �޼ҵ�
    public void SetMusicVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);    // ������� ���� ����
    }

    // �� ȿ���� ���� ���� �޼ҵ�
    public void SetSfxVolume(float value) {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);      // ȿ���� ���� ����
    }
}
