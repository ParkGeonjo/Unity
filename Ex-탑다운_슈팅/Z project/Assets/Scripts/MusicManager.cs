using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme; // 메인 음악
    public AudioClip menuTheme; // 메뉴 음악

    void Start() {
        AudioManager.instance.PlayMusic(menuTheme, 2);  // 음악 재생 메소드 호출
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {          // 스페이스바 누를 때
        AudioManager.instance.PlayMusic(mainTheme, 3);  // 음악 재생 메소드 호출
        }
    }
}
