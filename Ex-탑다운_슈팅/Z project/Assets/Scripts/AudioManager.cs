using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    float masterVolumePercent = 1;          // ������ ����
    float sfxVolumePercent = 1;             // ȿ���� ����
    float musicVolumePercent = 1;           // ���� ����

    AudioSource[] musicSources;             // ������ ������ ������ҽ� ���۷��� �迭
    int activeMusicSourceIntdex;            // ������� ���� �ε���

    public static AudioManager instance;    // �̱��� ����

    Transform audioListener;                // ����� ������ ��ġ ���۷���
    Transform playerT;                      // �÷��̾� ��ġ ���۷���

    void Awake() {
        instance = this; // �ν��Ͻ� ����

        musicSources = new AudioSource[2];                                              // ũ�� �����Ͽ� �Ҵ�
        for(int i = 0; i < 2; i++) {
            GameObject newMusicSource = new GameObject("Music Source " + (i + 1));      // ������ҽ��� ���� ������Ʈ ����
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();               // ������ҽ� �Ҵ�
            newMusicSource.transform.parent = transform;                                // �θ� ������Ʈ ����
        }

        audioListener = FindObjectOfType<AudioListener>().transform;                    // ����� �����ʰ� �ִ� ������Ʈ�� ��ġ�� ����
        playerT = FindObjectOfType<Player>().transform;                                 // �÷��̾� ������Ʈ�� ��ġ ����
    }

    void Update() { 
        if(playerT != null) {                           // �÷��̾ �ִ� ���
            audioListener.position = playerT.position;  // ����� �����ʰ� �ִ� ������Ʈ ��ġ�� �÷��̾� ��ġ�� ����
        }
    }

    // �� ���� ��� �޼ҵ�
    public void PlayMusic(AudioClip clip, int fadeDuration = 1) {
        activeMusicSourceIntdex = 1 - activeMusicSourceIntdex;                          // ����� ���� �ε��� ����
        musicSources[activeMusicSourceIntdex].clip = clip;                              // ����� ���� Ŭ�� ����
        musicSources[activeMusicSourceIntdex].Play();                                   // ���� ���

        StartCoroutine(AnimateMusicCrossFade(fadeDuration));                            // ���� ũ�ν����̵� �ڷ�ƾ ����
    }

    // �� ���� ��� �޼ҵ�
    public void PlaySound(AudioClip clip, Vector3 pos) {
        if(clip != null) { // ����� Ŭ���� �ִ� ���
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
            // pos ��ġ�� ȿ���� * ������ ���� ũ��� clip ���
        }
    }

    // �� ���� ũ�ν����̵�(�ε巴�� ��ȯ) �ڷ�ƾ
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
