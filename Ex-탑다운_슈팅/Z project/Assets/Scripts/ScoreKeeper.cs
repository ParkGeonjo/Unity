using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour {
    public static int score { get; private set; }           // ����

    float lastEnemyKillTime;                                // ���� �� óġ �� �ð�
    int streakCount;                                        // ���� óġ ī��Ʈ
    float streakExpiryTime = 1;                                 // ���� óġ ���� �ð�

    void Start() {
        score = 0;                                                  // ���� �ʱ�ȭ
        Enemy.OnDeathStatic += OnEnemyKilled;                       // �� ��� �̺�Ʈ ����
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;        // �÷��̾� ��� �̺�Ʈ ����
    }

    // �� �� ��� �� ȣ��� �޼ҵ�
    void OnEnemyKilled() {
        if(Time.time < lastEnemyKillTime + streakExpiryTime) {          // �� óġ �� óġ ���� �ð��� ������ ���� ���
            streakCount++;                                              // ���� óġ ī��Ʈ �߰�
        }
        else {
            streakCount = 0;                                            // ���� óġ ī��Ʈ �ʱ�ȭ
        }

        lastEnemyKillTime = Time.time;                                  // �� óġ �ð� ����

        score += 5 + streakCount;                                       // ���� �߰�
    }

    // �� �÷��̾� ��� �� ȣ��� �޼ҵ�
    void OnPlayerDeath() { 
        Enemy.OnDeathStatic -= OnEnemyKilled;                       // �� ��� �̺�Ʈ ���� ���
    }
}
