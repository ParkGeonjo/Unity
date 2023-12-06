using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves; // ���̺���� ������ �迭 ����
    public Enemy enemy; // ������ �� ���۷���

    LivingEntity playerEntity; // �÷��̾� ���۷���
    Transform playerT; // �÷��̾� ��ġ

    Wave currentWave; // ���� ���̺� ���۷���
    int currentWaveNumber; // ���� ���̺� ��ȣ

    int enemiesRemainingToSpawn; // �����ִ� ���� �� �� ��
    int enemiesRemainingAlive; // ����ִ� ���� ��
    float nextSpawnTime; // ���� ���������� �ð�

    MapGenerator map; // �� ������ ���۷���

    float timeBetweenCampingChecks = 2; // ķ�� ���� üũ ������
    float nextCampingCheckTime; // ���� ķ�� üũ �ð�
    float campThresholdDistance = 1.5f; // ķ�� ������ ���� �̵��ؾ� �ϴ� �Ÿ�
    Vector3 campPositionOld; // ���� �÷��̾� ��ġ
    bool isCamping; // ķ��������

    bool isDisable; // �÷��̾� ���� ����

    public event System.Action<int> OnNewWave; // ���̺� ��ȣ�� �Ű������� ���� ���̺� �̺�Ʈ �߰�

    void Start() {
        playerEntity = FindObjectOfType<Player>(); // �÷��̾� ������Ʈ �Ҵ�
        playerT = playerEntity.transform; // �÷��̾� ��ġ

        nextCampingCheckTime = Time.time + timeBetweenCampingChecks;
        // ���� ķ�� üũ �ð� ����
        campPositionOld = playerT.position;
        // �÷��̾� ��ġ ����

        map = FindObjectOfType<MapGenerator>(); // �� ������
        NextWave(); // ���� �� ���̺� ����

        playerEntity.OnDeath += OnPlayerDeath; // �÷��̾ ���� �� �� �޼ҵ带 �߰�
    }

    void Update()
    {
        if (!isDisable) { // ��Ȱ��ȭ(�÷��̾� ����)�� �ƴ� ���
            if (Time.time > nextCampingCheckTime)
            { // ķ�� üũ �ð��� ���� ���
                nextCampingCheckTime = Time.time + timeBetweenCampingChecks;
                // ���� ķ�� üũ �ð� ����

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                // ķ�������� Ȯ��
                campPositionOld = playerT.position;
                // ��ġ ����
            }

            if (enemiesRemainingToSpawn > 0 && Time.time >= nextSpawnTime)
            // �� ���� �����ְ� ���� �ð��� ���� ���� �ð����� ū ���
            {
                enemiesRemainingToSpawn--; // �� ���� �ϳ� ����
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                // ���� ���� �ð��� ����ð� + ���� ���� ���� ����

                StartCoroutine(SpawnEnemy()); // �� ���� �ڷ�ƾ ���� 
            }
        }
    }

    // �� �� ���� �ڷ�ƾ
    IEnumerator SpawnEnemy() {
        float spawnDelay = 1;
        // �� ���� ���ð�
        float tileFlashSpeed = 4;
        // �ʴ� ��� Ÿ���� �������� ����
        
        Transform spawnTile = map.GetRandomOpenTile();
        // ���� ���� Ÿ�� ��������

        if (isCamping) { // �÷��̾ ķ������ ���
            spawnTile = map.GetTileFromPosition(playerT.position);
            // Ÿ�� ��ġ�� �÷��̾� ��ġ�� ����
            isCamping = false;
            // ķ�� ����
        }

        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        // ������ Ÿ���� ���׸��� ��������
        Color initialColour = tileMat.color;
        // ���� Ÿ�� ���� ����
        Color flashColour = Color.red;
        // ������ ���� ����

        float spawnTimer = 0;
        // �� ��ȯ �ð� Ÿ�̸�

        while(spawnTimer < spawnDelay) { // �� ���� ���ð��� �ȵ� ���
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            // �������� ���� ������ �����Ͽ� �����̵���, PingPong �Լ��� ������ Ƚ����ŭ �����̵��� �ӵ��� ����.

            spawnTimer += Time.deltaTime; // Ÿ�̸ӿ� �ð� �߰�
            yield return null; // �� ������ ���
        }

        tileMat.color = initialColour; // Ÿ�� ���� �ʱ�ȭ 

        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        // ���� �ν��Ͻ�ȭ�� ���� ����, ���� Ÿ�� ��ġ�� ȸ���� ���� ��ġ
        spawnedEnemy.OnDeath += OnEnemyDeath;
        // ���� ���� �� �� �޼ҵ带 �߰�
    }

    // �� �÷��̾ ���� �� ó���ϴ� �޼ҵ�
    void OnPlayerDeath() {
        isDisable = true;
        // ���� ���� ����(��Ȱ��ȭ)���� ����.
    }

    void OnEnemyDeath()
    /* ���� ���� �� ó���ϴ� �޼ҵ�
       ���� ������ LivingEntity ���� OnDeath �� ȣ���ϰ�,
       OnDeath �� �� �޼ҵ带 ȣ���ؼ� ���� ���� �� �˷��ش�. */ 
    {
        enemiesRemainingAlive--; // ���� ���� 1 ����

        if(enemiesRemainingAlive == 0) // ���� ���� 0�� �Ǹ�
        {
            NextWave(); // ���� ���̺� ����
        }
    }

    // �� �÷��̾� ��ġ ���� �޼ҵ� 
    void ResetPlayerPosition() {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 1.5f;
        // �÷��̾� ��ġ�� ��� Ÿ�� ��ġ�� ����
    }


    // �� ���� ���̺� ���� �޼ҵ�
    void NextWave() 
    {
        currentWaveNumber++; // ���̺� ���� ���� 

        if(currentWaveNumber - 1 < waves.Length) { // �迭 �ε��� ���� ������ ó��
            currentWave = waves[currentWaveNumber - 1];
            /* ���� ���̺� ���۷��� ����
               (���̺� ���ڴ� 1���� ���� �� ���̹Ƿ� -1 �Ͽ� �迭�� �ε����� �°� ����) */

            enemiesRemainingToSpawn = currentWave.enemyCount;
            // �̹� ���̺��� �� �� ����
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            // ����ִ� ���� ���� ���� �� ���� ���� ����

            if (OnNewWave != null) { // �̺�Ʈ �����ڰ� �ִ� ���
                OnNewWave(currentWaveNumber); // ���̺� ��ȣ ����
            }
            ResetPlayerPosition(); // �÷��̾� ��ġ �ʱ�ȭ
        }
    }


    [System.Serializable]
    /* ��ũ��Ʈ ����ȭ, ����ȭ�� ���� ��ü, ���� ���� ���� �� ����
       ���� ����(private ��)�� ���������� �ν����Ϳ��� ���� ���� �����ϰ� �� */
    public class Wave
    // ���� �ֱ�, ���� �ֱ� �� ���̺� ������ ���� �� Ŭ���� ����
    {
        public int enemyCount; // ���� ��
        public float timeBetweenSpawns; // ���� ���� �ֱ�
    }
}
