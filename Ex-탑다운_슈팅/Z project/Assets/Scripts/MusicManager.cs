using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;     // ���� ����
    public AudioClip menuTheme;     // �޴� ����

    string sceneName;               // �� �̸�

    void Start() {
        OnLevelWasLoaded(0);            // �� ���� ��� �޼ҵ� ȣ��
    }

    // �� �� ���� ��� �޼ҵ�
    void OnLevelWasLoaded(int sceneIndex) {
        string newSceneName = SceneManager.GetActiveScene().name;       // ���� Ȱ��ȭ���� ���� �̸��� �����´�.
        if(sceneName != newSceneName) {                                 // �����ص� �� �̸��� ���� Ȱ��ȭ�� ���� �̸��� �ٸ���
            sceneName = newSceneName;                                   // �� �̸��� �ٽ� ����
            Invoke("PlayMusic", .2f);                                   // PlayMusic �޼ҵ带 0.2�� �� ȣ��
        }
    }

    // �� ���� ��� �޼ҵ�
    void PlayMusic() {
        AudioClip clipToPlay = null;                            // ����� Ŭ���� ����

        if(sceneName == "Menu") {                               // ���� ���� �޴� ���� ���
            clipToPlay = menuTheme;                             // ����� Ŭ���� �޴� �������� ����
        }
        else if(sceneName == "Game") {                          // ���� ���� ���� ���� ���
            clipToPlay = mainTheme;                             // ����� Ŭ���� ���� �������� ����
        }

        if(clipToPlay != null) {                                // ����� Ŭ���� �ִ� ���
            AudioManager.instance.PlayMusic(clipToPlay, 2);     // Ŭ��(����)���
            Invoke("PlayMusic", clipToPlay.length);             // Ŭ��(����)�� ����ð��� ������ PlayMusic �޼ҵ带 ȣ��
        }
    }
}
