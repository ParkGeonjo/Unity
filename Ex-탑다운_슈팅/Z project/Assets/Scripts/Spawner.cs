using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves; // 웨이브들을 저장할 배열 생성
    public Enemy enemy; // 스폰할 적 레퍼런스

    Wave currentWave; // 현재 웨이브 레퍼런스
    int currentWaveNumber; // 현재 웨이브 번호

    int enemiesRemainingToSpawn; // 남아있는 스폰 할 적 수
    int enemiesRemainingAlive; // 살아있는 적의 수
    float nextSpawnTime; // 다음 스폰까지의 시간

    void Start()
    {
        NextWave(); // 시작 시 웨이브 실행
    }

    void Update()
    {
        if(enemiesRemainingToSpawn > 0 && Time.time >= nextSpawnTime)
        // 적 수가 남아있고 현재 시간이 다음 스폰 시간보다 큰 경우
        {
            enemiesRemainingToSpawn--; // 적 수를 하나 줄임
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
            // 다음 스폰 시간을 현재시간 + 스폰 간격 으로 저장

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            // 적을 인스턴스화를 통해 생성, 맵 중앙에 회전값 없이 배치
            spawnedEnemy.OnDeath += OnEnemyDeath;
            // 적이 죽을 때 위 메소드를 추가
        }
    }

    void OnEnemyDeath()
    /* 적이 죽을 때 처리하는 메소드
       적이 죽으면 LivingEntity 에서 OnDeath 를 호출하고,
       OnDeath 는 이 메소드를 호출해서 적이 죽을 때 알려준다. */ 
    {
        enemiesRemainingAlive--; // 적의 수를 1 줄임

        if(enemiesRemainingAlive == 0) // 적의 수가 0이 되면
        {
            NextWave(); // 다음 웨이브 실행
        }
    }

    void NextWave() // 다음 웨이브 실행 메소드
    {
        currentWaveNumber++; // 웨이브 숫자 증가
        print("Wave : " + currentWaveNumber); // 현재 웨이브 번호 출력
        if(currentWaveNumber - 1 < waves.Length) { // 배열 인덱스 예외 없도록 처리
            currentWave = waves[currentWaveNumber - 1];
            /* 현재 웨이브 레퍼런스 참조
               (웨이브 숫자는 1부터 시작 할 것이므로 -1 하여 배열의 인덱스에 맞게 참조) */

            enemiesRemainingToSpawn = currentWave.enemyCount;
            // 이번 웨이브의 적 수 저장
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            // 살아있는 적의 수를 스폰 할 적의 수로 저장
        }
    }


    [System.Serializable]
    /* 스크립트 직렬화, 직렬화를 통해 객체, 변수 등을 선언 할 때의
       접근 제한(private 등)은 유지되지만 인스펙터에서 값을 변경 가능하게 함 */
    public class Wave
    // 적의 주기, 스폰 주기 등 웨이브 정보를 저장 할 클래스 생성
    {
        public int enemyCount; // 적의 수
        public float timeBetweenSpawns; // 적의 스폰 주기
    }
}
