using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public enum AudioChannel {Master, Sfx, Music};          // 오디오 채널

    // 값을 참조하는 권한은 public 이지만 값을 설정하는 권한은 private 로
    public float masterVolumePercent { get; private set; }  // 마스터 볼륨
    public float sfxVolumePercent { get; private set; }     // 효과음 볼륨
    public float musicVolumePercent { get; private set; }   // 음악 볼륨

    AudioSource sfx2DSource;                                // 2D 효과음 오디오소스 레퍼런스
    AudioSource[] musicSources;                             // 음악을 가져올 오디오소스 레퍼런스 배열
    int activeMusicSourceIntdex;                            // 재생중인 음악 인덱스

    public static AudioManager instance;                    // 싱글톤 패턴

    Transform audioListener;                                // 오디오 리스너 위치 레퍼런스
    Transform playerT;                                      // 플레이어 위치 레퍼런스

    SoundLibrary library;                                   // 사운드 라이브러리 레퍼런스

    void Awake() {
        if(instance != null) {                                                          // 인스턴스가 생성되어 있다면
            Destroy(gameObject);                                                        // 현재 게임 오브젝트 파괴    
        }
        else { 
            instance = this;                                                                // 인스턴스 설정
            DontDestroyOnLoad(gameObject);                                                  // 게임 로드 시 이 게임 오브젝트가 파괴되지 않도록 한다

            library = GetComponent<SoundLibrary>();                                         // 사운드 라이브러리 할당

            musicSources = new AudioSource[2];                                              // 크기 설정하여 할당
            for(int i = 0; i < 2; i++) {
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));      // 오디오소스를 가질 오브젝트 생성
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();               // 오디오소스 할당
                newMusicSource.transform.parent = transform;                                // 부모 오브젝트 설정
            }

            GameObject newSfxSource = new GameObject("2D sfx Source");                      // 2D 오디오소스를 가질 오브젝트 생성
            sfx2DSource = newSfxSource.AddComponent<AudioSource>();                         // 오디오소스 할당
            newSfxSource.transform.parent = transform;                                      // 부모 오브젝트 설정

            audioListener = FindObjectOfType<AudioListener>().transform;                    // 오디오 리스너가 있는 오브젝트의 위치를 저장
            if(FindObjectOfType<Player>() != null) {                                        // 플레이어 오브젝트가 있는 경우
                playerT = FindObjectOfType<Player>().transform;                             // 플레이어 오브젝트의 위치 저장
            }

            // PlayerPrefs 를 사용해 저장한 볼륨을 불러와 게임에 적용한다.
            masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
        } 
    }

    void Update() { 
        if(playerT != null) {                           // 플레이어가 있는 경우
            audioListener.position = playerT.position;  // 오디오 리스너가 있는 오브젝트 위치를 플레이어 위치로 설정
        }
    }

    // ■ 사운드 볼륨 조절 메소드
    public void SetVolume(float volumePercent, AudioChannel channel) { 
        switch(channel) {
            case AudioChannel.Master:                       // 마스터 볼륨 설정
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:                          // 효과음 볼륨 설정
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:                        // 음악 볼륨 설정
                musicVolumePercent = volumePercent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;          // 설정한 첫 번째 음악의 볼륨 설정
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;          // 설정한 두 번째 음악의 볼륨 설정

        // PlayerPrefs 를 사용해 볼륨을 저장하고 다음번에 게임을 실행할 때도 적용되도록 한다.
        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }

    // ■ 음악 재생 메소드
    public void PlayMusic(AudioClip clip, int fadeDuration = 1) {
        activeMusicSourceIntdex = 1 - activeMusicSourceIntdex;                          // 재생할 음악 인덱스 설정
        musicSources[activeMusicSourceIntdex].clip = clip;                              // 재생할 음악 클립 설정
        musicSources[activeMusicSourceIntdex].Play();                                   // 음악 재생

        StartCoroutine(AnimateMusicCrossFade(fadeDuration));                            // 음악 크로스페이드 코루틴 실행
    }

    // ■ 오디오 클립과 위치를 받아 해당 위치에 사운드를 재생하는 메소드
    public void PlaySound(AudioClip clip, Vector3 pos) {
        if(clip != null) { // 오디오 클립이 있는 경우
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
            // pos 위치에 효과음 * 마스터 볼륨 크기로 clip 재생
        }
    }

    // ■ 오디오 이름과 위치를 받아 해당 위치레 사운드를 재생하는 메소드
    public void PlaySound(string soundName, Vector3 pos) {
        PlaySound(library.GetClipFromName(soundName), pos);                 // 사운드 재생 메소드 호출 
    }

    // ■ 오디오 이름과 위치를 받아 2D 기준 사운드를 재생하는 메소드
    public void PlaySound2D(string soundName) {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
        // 효과음을 2D 사운드로 출력
    }

    // ■ 음악을 크로스페이드(부드럽게 변환) 코루틴
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
