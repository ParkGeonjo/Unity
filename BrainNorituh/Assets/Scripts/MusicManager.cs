using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip MainTheme;         // 타이틀, 로비 음악
    public AudioClip GameTheme;         // 게임 음악

    string sceneName;                   // 씬 이름

    void Start() {
        OnLevelWasLoaded(0);            // 씬 음악 재생 메소드 호출
    }

    // ■ 씬 음악 재생 메소드
    void OnLevelWasLoaded(int sceneIndex) {
        string newSceneName = SceneManager.GetActiveScene().name;       // 현재 활성화충인 씬의 이름을 가져온다.
        if(sceneName != newSceneName) {                                 // 저장해둔 씬 이름이 현재 활성화된 씬의 이름과 다르면
            if ((sceneName == "Title" || sceneName == "Lobby") && (newSceneName == "Title" || newSceneName == "Lobby")) return;   
            sceneName = newSceneName;                                   // 씬 이름을 다시 저장
            PlayMusic();                                                // PlayMusic 메소드를 호출
        }
    }

    // ■ 음악 재생 메소드
    void PlayMusic()
    {
        AudioClip clipToStop = null;
        AudioClip clipToPlay = null;                                                        // 재생할 클립을 생성

        if (sceneName == "Title" || sceneName == "Lobby")                                   // 현재 씬이 메인 또는 로비 씬인 경우
        {
            clipToPlay = MainTheme;                                                         // 재생할 클립을 메인 음악으로 설정
            clipToStop = GameTheme;
        }
        else if (sceneName == "Game1" || sceneName == "Game2" || sceneName == "Game3")      // 현재 씬이 게임 씬인 경우
        {
            clipToPlay = GameTheme;                                                         // 재생할 클립을 음악으로 설정
            clipToStop = MainTheme;
        }

        if (clipToPlay != null)                                                             // 재생할 클립이 있는 경우
        {
            AudioManager.instance.StopMusic(clipToStop);                                    // 기존 클립(음악)정지
            AudioManager.instance.PlayMusic(clipToPlay);                                    // 클립(음악)재생
            Invoke("PlayMusic", clipToPlay.length);                                         // 클립(음악)의 재생시간이 지나고 PlayMusic 메소드를 호출
        }
    }
}
