using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;         // ���� �׷� ��ü �迭

    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();
    // Dictionary �� ���� �� ���� �迭(�׷�)�� Ű�� �ְ� �ش� Ű������ ���� �׷��� ã�� �� �ִ�.

    void Awake() { 
        foreach(SoundGroup soundGroup in soundGroups) {
            groupDictionary.Add(soundGroup.groupID, soundGroup.group);      // ��ųʸ��� �׷� ���� �߰�
        }
    }

    // �� �̸����� ���� �迭�� �޾� ���� ���带 ��ȯ�ϴ� �޼ҵ�
    public AudioClip GetClipFromName(string name) {
        if(groupDictionary.ContainsKey(name)) {                             // name �� Key�� �ϴ� Value �� �ִٸ�
            AudioClip[] sounds = groupDictionary[name];                     // name �� Key�� �ϴ� Value �� sounds �����Ŭ�� �迭�� ����
            return sounds[Random.Range(0, sounds.Length)];                  // sounds �� ���� �� �������� �ϳ��� ��ȯ
        }
        return null;
    }

    // �� ���� �׷�(�迭) Ŭ����, ����ȭ
    [System.Serializable]
    public class SoundGroup {
        public string groupID;              // ���� �迭 �ε���
        public AudioClip[] group;           // ���� �迭
    }
}
