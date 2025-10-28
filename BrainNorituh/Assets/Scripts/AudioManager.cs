using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music };          // 오디오 채널

    // 값을 참조하는 권한은 public 이지만 값을 설정하는 권한은 private 로
    public float masterVolumePercent { get; private set; }  // 마스터 볼륨
    public float sfxVolumePercent { get; private set; }     // 효과음 볼륨
    public float musicVolumePercent { get; private set; }   // 음악 볼륨

    AudioSource[] musicSources;                             // 음악을 가져올 오디오소스 레퍼런스 배열
    int activeMusicSourceIntdex;                            // 재생중인 음악 인덱스

    public static AudioManager instance;                    // 싱글톤 패턴

    void Awake()
    {
        if (instance != null)                                                           // 인스턴스가 생성되어 있다면
        {
            Destroy(gameObject);                                                        // 현재 게임 오브젝트 파괴    
        }
        else
        {
            instance = this;                                                                // 인스턴스 설정
            DontDestroyOnLoad(gameObject);                                                  // 게임 로드 시 이 게임 오브젝트가 파괴되지 않도록 한다

            musicSources = new AudioSource[2];                                              // 크기 설정하여 할당
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));      // 오디오소스를 가질 오브젝트 생성
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();               // 오디오소스 할당
                newMusicSource.transform.parent = transform;                                // 부모 오브젝트 설정
            }
        }

        LoadVolume();
    }

    public void LoadVolume()
    {
        // PlayerPrefs 를 사용해 저장한 볼륨을 불러와 게임에 적용한다.
        masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
        sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
        musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
    }

    // ■ 사운드 볼륨 조절 메소드
    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
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
        Debug.Log("볼륨저장" + masterVolumePercent + " / " + sfxVolumePercent + " / " + musicVolumePercent);
    }

    // ■ 음악 재생 메소드
    public void PlayMusic(AudioClip clip)
    {
        activeMusicSourceIntdex = 1 - activeMusicSourceIntdex;                          // 재생할 음악 인덱스 설정
        musicSources[activeMusicSourceIntdex].clip = clip;                              // 재생할 음악 클립 설정
        musicSources[activeMusicSourceIntdex].Play();                                   // 음악 재생
    }

    // ■ 음악 정지 메소드
    public void StopMusic(AudioClip clip)
    {
        activeMusicSourceIntdex = 1 - activeMusicSourceIntdex;                          // 재생할 음악 인덱스 설정
        musicSources[activeMusicSourceIntdex].clip = clip;                              // 재생할 음악 클립 설정
        musicSources[activeMusicSourceIntdex].Stop();                                   // 음악 정지
    }

    // ■ 오디오 클립과 위치를 받아 해당 위치에 사운드를 재생하는 메소드
    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        { // 오디오 클립이 있는 경우
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
            // pos 위치에 효과음 * 마스터 볼륨 크기로 clip 재생
        }
    }

    float musicFadeMultiplier = 1f;  // 음악 소리 페이드 시에 곱할 값

    // ■ 음악 소리 페이드 메소드
    public void FadeMusicVolumeMultiplier(float targetMultiplier, float duration)
    {
        StartCoroutine(FadeMusicVolumeMultiplierCoroutine(targetMultiplier, duration));
    }

    // ■ 음악 소리 페이드 코루틴
    private IEnumerator FadeMusicVolumeMultiplierCoroutine(float targetMultiplier, float duration)
    {
        float startMultiplier = musicFadeMultiplier;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            musicFadeMultiplier = Mathf.Lerp(startMultiplier, targetMultiplier, time / duration);
            ApplyMusicVolume();  // 실시간 적용
            yield return null;
        }

        musicFadeMultiplier = targetMultiplier;
        ApplyMusicVolume();
    }

    // ■ 음악 볼륨 적용
    private void ApplyMusicVolume()
    {
        float finalVolume = musicVolumePercent * masterVolumePercent * musicFadeMultiplier;
        musicSources[0].volume = finalVolume;
        musicSources[1].volume = finalVolume;
    }
}
