using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour {
    public static int score { get; private set; }           // 점수

    float lastEnemyKillTime;                                // 이전 적 처치 후 시간
    int streakCount;                                        // 연속 처치 카운트
    float streakExpiryTime = 1;                                 // 연속 처치 만료 시간

    void Start() {
        score = 0;                                                  // 점수 초기화
        Enemy.OnDeathStatic += OnEnemyKilled;                       // 적 사망 이벤트 구독
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;        // 플레이어 사망 이벤트 구독
    }

    // ■ 적 사망 시 호출될 메소드
    void OnEnemyKilled() {
        if(Time.time < lastEnemyKillTime + streakExpiryTime) {          // 적 처치 후 처치 만료 시간이 지나기 전인 경우
            streakCount++;                                              // 연속 처치 카운트 추가
        }
        else {
            streakCount = 0;                                            // 연속 처치 카운트 초기화
        }

        lastEnemyKillTime = Time.time;                                  // 적 처치 시간 설정

        score += 5 + streakCount;                                       // 점수 추가
    }

    // ■ 플레이어 사망 시 호출될 메소드
    void OnPlayerDeath() { 
        Enemy.OnDeathStatic -= OnEnemyKilled;                       // 적 사망 이벤트 구독 취소
    }
}
