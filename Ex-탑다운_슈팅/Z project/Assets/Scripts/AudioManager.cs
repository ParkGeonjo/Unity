using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    float masterVolumePercent = 1;          // 마스터 볼륨
    float sfxVolumePercent = 1;             // 효과음 볼륨
    float musicVolumePercent = 1;           // 음악 볼륨

    AudioSource[] musicSources;             // 음악을 가져올 오디오소스 레퍼런스 배열
    int activeMusicSourceIntdex;            // 재생중인 음악 인덱스

    public static AudioManager instance;    // 싱글톤 패턴

    Transform audioListener;                // 오디오 리스너 위치 레퍼런스
    Transform playerT;                      // 플레이어 위치 레퍼런스

    void Awake() {
        instance = this; // 인스턴스 설정

        musicSources = new AudioSource[2];                                              // 크기 설정하여 할당
        for(int i = 0; i < 2; i++) {
            GameObject newMusicSource = new GameObject("Music Source " + (i + 1));      // 오디오소스를 가질 오브젝트 생성
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();               // 오디오소스 할당
            newMusicSource.transform.parent = transform;                                // 부모 오브젝트 설정
        }

        audioListener = FindObjectOfType<AudioListener>().transform;                    // 오디오 리스너가 있는 오브젝트의 위치를 저장
        playerT = FindObjectOfType<Player>().transform;                                 // 플레이어 오브젝트의 위치 저장
    }

    void Update() { 
        if(playerT != null) {                           // 플레이어가 있는 경우
            audioListener.position = playerT.position;  // 오디오 리스너가 있는 오브젝트 위치를 플레이어 위치로 설정
        }
    }

    // ■ 음악 재생 메소드
    public void PlayMusic(AudioClip clip, int fadeDuration = 1) {
        activeMusicSourceIntdex = 1 - activeMusicSourceIntdex;                          // 재생할 음악 인덱스 설정
        musicSources[activeMusicSourceIntdex].clip = clip;                              // 재생할 음악 클립 설정
        musicSources[activeMusicSourceIntdex].Play();                                   // 음악 재생

        StartCoroutine(AnimateMusicCrossFade(fadeDuration));                            // 음악 크로스페이드 코루틴 실행
    }

    // ■ 사운드 재생 메소드
    public void PlaySound(AudioClip clip, Vector3 pos) {
        if(clip != null) { // 오디오 클립이 있는 경우
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
            // pos 위치에 효과음 * 마스터 볼륨 크기로 clip 재생
        }
    }

    // ■ 음악 크로스페이드(부드럽게 변환) 코루틴
    IEnumerator AnimateMusicCrossFade(int duration) {
        float percent = 0; // 음악 퍼센트                  

        while(percent < 1) { 
            percent += Time.deltaTime * 1 / duration; // 퍼센트 계산
            musicSources[activeMusicSourceIntdex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIntdex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            // 음악 볼륨 설정, Lerp 를 통해 활성화하는 음악은 점점 커지게, 비활성와 음악은 점점 작아지게 설정.
            yield return null; // 다음 프레임으로 스킵 
        }
    }
}
