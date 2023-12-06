using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves; // 웨이브들을 저장할 배열 생성
    public Enemy enemy; // 스폰할 적 레퍼런스

    LivingEntity playerEntity; // 플레이어 레퍼런스
    Transform playerT; // 플레이어 위치

    Wave currentWave; // 현재 웨이브 레퍼런스
    int currentWaveNumber; // 현재 웨이브 번호

    int enemiesRemainingToSpawn; // 남아있는 스폰 할 적 수
    int enemiesRemainingAlive; // 살아있는 적의 수
    float nextSpawnTime; // 다음 스폰까지의 시간

    MapGenerator map; // 맵 생성기 레퍼런스

    float timeBetweenCampingChecks = 2; // 캠핑 방지 체크 딜레이
    float nextCampingCheckTime; // 다음 캠핑 체크 시간
    float campThresholdDistance = 1.5f; // 캠핑 방지를 위해 이동해야 하는 거리
    Vector3 campPositionOld; // 이전 플레이어 위치
    bool isCamping; // 캠핑중인지

    bool isDisable; // 플레이어 생존 여부

    public event System.Action<int> OnNewWave; // 웨이브 번호를 매개변수로 갖는 웨이브 이벤트 추가

    void Start() {
        playerEntity = FindObjectOfType<Player>(); // 플레이어 오브젝트 할당
        playerT = playerEntity.transform; // 플레이어 위치

        nextCampingCheckTime = Time.time + timeBetweenCampingChecks;
        // 다음 캠핑 체크 시간 설정
        campPositionOld = playerT.position;
        // 플레이어 위치 설정

        map = FindObjectOfType<MapGenerator>(); // 맵 생성기
        NextWave(); // 시작 시 웨이브 실행

        playerEntity.OnDeath += OnPlayerDeath; // 플레이어가 죽을 때 위 메소드를 추가
    }

    void Update()
    {
        if (!isDisable) { // 비활성화(플레이어 죽음)가 아닌 경우
            if (Time.time > nextCampingCheckTime)
            { // 캠핑 체크 시간이 지난 경우
                nextCampingCheckTime = Time.time + timeBetweenCampingChecks;
                // 다음 캠핑 체크 시간 설정

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                // 캠핑중인지 확인
                campPositionOld = playerT.position;
                // 위치 갱신
            }

            if (enemiesRemainingToSpawn > 0 && Time.time >= nextSpawnTime)
            // 적 수가 남아있고 현재 시간이 다음 스폰 시간보다 큰 경우
            {
                enemiesRemainingToSpawn--; // 적 수를 하나 줄임
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                // 다음 스폰 시간을 현재시간 + 스폰 간격 으로 저장

                StartCoroutine(SpawnEnemy()); // 적 스폰 코루틴 시작 
            }
        }
    }

    // ■ 적 스폰 코루틴
    IEnumerator SpawnEnemy() {
        float spawnDelay = 1;
        // 적 스폰 대기시간
        float tileFlashSpeed = 4;
        // 초당 몇번 타일이 깜빡일지 설정
        
        Transform spawnTile = map.GetRandomOpenTile();
        // 랜덤 오픈 타일 가져오기

        if (isCamping) { // 플레이어가 캠핑중인 경우
            spawnTile = map.GetTileFromPosition(playerT.position);
            // 타일 위치를 플레이어 위치로 설정
            isCamping = false;
            // 캠핑 해제
        }

        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        // 가져온 타일의 마테리얼 가져오기
        Color initialColour = tileMat.color;
        // 기존 타일 색상 저장
        Color flashColour = Color.red;
        // 변경할 색상 저장

        float spawnTimer = 0;
        // 적 소환 시간 타이머

        while(spawnTimer < spawnDelay) { // 적 스폰 대기시간이 안된 경우
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            // 보간값을 통해 색상을 설정하여 깜빡이도록, PingPong 함수로 설정한 횟수만큼 깜빡이도록 속도를 설정.

            spawnTimer += Time.deltaTime; // 타이머에 시간 추가
            yield return null; // 한 프레임 대기
        }

        tileMat.color = initialColour; // 타일 색상 초기화 

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        // 적을 인스턴스화를 통해 생성, 랜덤 타일 위치에 회전값 없이 배치
        spawnedEnemy.OnDeath += OnEnemyDeath;
        // 적이 죽을 때 위 메소드를 추가
    }

    // ■ 플레이어가 죽을 때 처리하는 메소드
    void OnPlayerDeath() {
        isDisable = true;
        // 생존 여부 죽음(비활성화)으로 설정.
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

    // ■ 플레이어 위치 리셋 메소드 
    void ResetPlayerPosition() {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 1.5f;
        // 플레이어 위치를 가운데 타일 위치로 설정
    }


    // ■ 다음 웨이브 실행 메소드
    void NextWave() 
    {
        currentWaveNumber++; // 웨이브 숫자 증가 

        if(currentWaveNumber - 1 < waves.Length) { // 배열 인덱스 예외 없도록 처리
            currentWave = waves[currentWaveNumber - 1];
            /* 현재 웨이브 레퍼런스 참조
               (웨이브 숫자는 1부터 시작 할 것이므로 -1 하여 배열의 인덱스에 맞게 참조) */

            enemiesRemainingToSpawn = currentWave.enemyCount;
            // 이번 웨이브의 적 수 저장
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            // 살아있는 적의 수를 스폰 할 적의 수로 저장

            if (OnNewWave != null) { // 이벤트 구독자가 있는 경우
                OnNewWave(currentWaveNumber); // 웨이브 번호 전달
            }
            ResetPlayerPosition(); // 플레이어 위치 초기화
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
