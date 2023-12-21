using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip mainTheme;     // 메인 음악
    public AudioClip menuTheme;     // 메뉴 음악

    string sceneName;               // 씬 이름

    void Start() {
        OnLevelWasLoaded(0);            // 씬 음악 재생 메소드 호출
    }

    // ■ 씬 음악 재생 메소드
    void OnLevelWasLoaded(int sceneIndex) {
        string newSceneName = SceneManager.GetActiveScene().name;       // 현재 활성화충인 씬의 이름을 가져온다.
        if(sceneName != newSceneName) {                                 // 저장해둔 씬 이름이 현재 활성화된 씬의 이름과 다르면
            sceneName = newSceneName;                                   // 씬 이름을 다시 저장
            Invoke("PlayMusic", .2f);                                   // PlayMusic 메소드를 0.2초 후 호출
        }
    }

    // ■ 음악 재생 메소드
    void PlayMusic() {
        AudioClip clipToPlay = null;                            // 재생할 클립을 생성

        if(sceneName == "Menu") {                               // 현재 씬이 메뉴 씬인 경우
            clipToPlay = menuTheme;                             // 재생할 클립을 메뉴 음악으로 설정
        }
        else if(sceneName == "Game") {                          // 현재 씬이 게임 씬인 경우
            clipToPlay = mainTheme;                             // 재생할 클립을 메인 음악으로 설정
        }

        if(clipToPlay != null) {                                // 재생할 클립이 있는 경우
            AudioManager.instance.PlayMusic(clipToPlay, 2);     // 클립(음악)재생
            Invoke("PlayMusic", clipToPlay.length);             // 클립(음악)의 재생시간이 지나고 PlayMusic 메소드를 호출
        }
    }
}
