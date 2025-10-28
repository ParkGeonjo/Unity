using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;         // 사운드 그룹 객체 배열

    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();
    // Dictionary 를 통해 각 사운드 배열(그룹)에 키를 주고 해당 키값으로 사운드 그룹을 찾을 수 있다.

    void Awake() { 
        foreach(SoundGroup soundGroup in soundGroups) {
            groupDictionary.Add(soundGroup.groupID, soundGroup.group);      // 딕셔너리에 그룹 사운드 추가
        }
    }

    // ■ 이름으로 사운드 배열을 받아 랜덤 사운드를 반환하는 메소드
    public AudioClip GetClipFromName(string name) {
        if(groupDictionary.ContainsKey(name)) {                             // name 을 Key로 하는 Value 가 있다면
            AudioClip[] sounds = groupDictionary[name];                     // name 을 Key로 하는 Value 를 sounds 오디오클립 배열에 저장
            return sounds[Random.Range(0, sounds.Length)];                  // sounds 의 사운드 중 랜덤으로 하나를 반환
        }
        return null;
    }

    // ■ 사운드 그룹(배열) 클래스, 직렬화
    [System.Serializable]
    public class SoundGroup {
        public string groupID;              // 사운드 배열 인덱스
        public AudioClip[] group;           // 사운드 배열
    }
}
