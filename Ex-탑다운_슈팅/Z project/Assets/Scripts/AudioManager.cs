using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public enum AudioChannel {Master, Sfx, Music};          // ����� ä��

    // ���� �����ϴ� ������ public ������ ���� �����ϴ� ������ private ��
    public float masterVolumePercent { get; private set; }  // ������ ����
    public float sfxVolumePercent { get; private set; }     // ȿ���� ����
    public float musicVolumePercent { get; private set; }   // ���� ����

    AudioSource sfx2DSource;                                // 2D ȿ���� ������ҽ� ���۷���
    AudioSource[] musicSources;                             // ������ ������ ������ҽ� ���۷��� �迭
    int activeMusicSourceIntdex;                            // ������� ���� �ε���

    public static AudioManager instance;                    // �̱��� ����

    Transform audioListener;                                // ����� ������ ��ġ ���۷���
    Transform playerT;                                      // �÷��̾� ��ġ ���۷���

    SoundLibrary library;                                   // ���� ���̺귯�� ���۷���

    void Awake() {
        if(instance != null) {                                                          // �ν��Ͻ��� �����Ǿ� �ִٸ�
            Destroy(gameObject);                                                        // ���� ���� ������Ʈ �ı�    
        }
        else { 
            instance = this;                                                                // �ν��Ͻ� ����
            DontDestroyOnLoad(gameObject);                                                  // ���� �ε� �� �� ���� ������Ʈ�� �ı����� �ʵ��� �Ѵ�

            library = GetComponent<SoundLibrary>();                                         // ���� ���̺귯�� �Ҵ�

            musicSources = new AudioSource[2];                                              // ũ�� �����Ͽ� �Ҵ�
            for(int i = 0; i < 2; i++) {
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));      // ������ҽ��� ���� ������Ʈ ����
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();               // ������ҽ� �Ҵ�
                newMusicSource.transform.parent = transform;                                // �θ� ������Ʈ ����
            }

            GameObject newSfxSource = new GameObject("2D sfx Source");                      // 2D ������ҽ��� ���� ������Ʈ ����
            sfx2DSource = newSfxSource.AddComponent<AudioSource>();                         // ������ҽ� �Ҵ�
            newSfxSource.transform.parent = transform;                                      // �θ� ������Ʈ ����

            audioListener = FindObjectOfType<AudioListener>().transform;                    // ����� �����ʰ� �ִ� ������Ʈ�� ��ġ�� ����
            if(FindObjectOfType<Player>() != null) {                                        // �÷��̾� ������Ʈ�� �ִ� ���
                playerT = FindObjectOfType<Player>().transform;                             // �÷��̾� ������Ʈ�� ��ġ ����
            }

            // PlayerPrefs �� ����� ������ ������ �ҷ��� ���ӿ� �����Ѵ�.
            masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
        } 
    }

    void Update() { 
        if(playerT != null) {                           // �÷��̾ �ִ� ���
            audioListener.position = playerT.position;  // ����� �����ʰ� �ִ� ������Ʈ ��ġ�� �÷��̾� ��ġ�� ����
        }
    }

    // �� ���� ���� ���� �޼ҵ�
    public void SetVolume(float volumePercent, AudioChannel channel) { 
        switch(channel) {
            case AudioChannel.Master:                       // ������ ���� ����
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:                          // ȿ���� ���� ����
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:                        // ���� ���� ����
                musicVolumePercent = volumePercent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;          // ������ ù ��° ������ ���� ����
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;          // ������ �� ��° ������ ���� ����

        // PlayerPrefs �� ����� ������ �����ϰ� �������� ������ ������ ���� ����ǵ��� �Ѵ�.
        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }

    // �� ���� ��� �޼ҵ�
    public void PlayMusic(AudioClip clip, int fadeDuration = 1) {
        activeMusicSourceIntdex = 1 - activeMusicSourceIntdex;                          // ����� ���� �ε��� ����
        musicSources[activeMusicSourceIntdex].clip = clip;                              // ����� ���� Ŭ�� ����
        musicSources[activeMusicSourceIntdex].Play();                                   // ���� ���

        StartCoroutine(AnimateMusicCrossFade(fadeDuration));                            // ���� ũ�ν����̵� �ڷ�ƾ ����
    }

    // �� ����� Ŭ���� ��ġ�� �޾� �ش� ��ġ�� ���带 ����ϴ� �޼ҵ�
    public void PlaySound(AudioClip clip, Vector3 pos) {
        if(clip != null) { // ����� Ŭ���� �ִ� ���
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
            // pos ��ġ�� ȿ���� * ������ ���� ũ��� clip ���
        }
    }

    // �� ����� �̸��� ��ġ�� �޾� �ش� ��ġ�� ���带 ����ϴ� �޼ҵ�
    public void PlaySound(string soundName, Vector3 pos) {
        PlaySound(library.GetClipFromName(soundName), pos);                 // ���� ��� �޼ҵ� ȣ�� 
    }

    // �� ����� �̸��� ��ġ�� �޾� 2D ���� ���带 ����ϴ� �޼ҵ�
    public void PlaySound2D(string soundName) {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
        // ȿ������ 2D ����� ���
    }

    // �� ������ ũ�ν����̵�(�ε巴�� ��ȯ) �ڷ�ƾ
    IEnumerator AnimateMusicCrossFade(int duration) {
        float percent = 0; // ���� �ۼ�Ʈ                  

        while(percent < 1) { 
            percent += Time.deltaTime * 1 / duration; // �ۼ�Ʈ ���
            musicSources[activeMusicSourceIntdex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIntdex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            // ���� ���� ����, Lerp �� ���� Ȱ��ȭ�ϴ� ������ ���� Ŀ����, ��Ȱ���� ������ ���� �۾����� ����.
            yield return null; // ���� ���������� ��ŵ 
        }
    }
}
