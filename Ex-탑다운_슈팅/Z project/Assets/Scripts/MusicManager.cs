using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme; // ���� ����
    public AudioClip menuTheme; // �޴� ����

    void Start() {
        AudioManager.instance.PlayMusic(menuTheme, 2);  // ���� ��� �޼ҵ� ȣ��
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {          // �����̽��� ���� ��
        AudioManager.instance.PlayMusic(mainTheme, 3);  // ���� ��� �޼ҵ� ȣ��
        }
    }
}
