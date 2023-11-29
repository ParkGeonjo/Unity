using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Wave[] waves; // ���̺���� ������ �迭 ����
    public Enemy enemy; // ������ �� ���۷���

    Wave currentWave; // ���� ���̺� ���۷���
    int currentWaveNumber; // ���� ���̺� ��ȣ

    int enemiesRemainingToSpawn; // �����ִ� ���� �� �� ��
    int enemiesRemainingAlive; // ����ִ� ���� ��
    float nextSpawnTime; // ���� ���������� �ð�

    void Start()
    {
        NextWave(); // ���� �� ���̺� ����
    }

    void Update()
    {
        if(enemiesRemainingToSpawn > 0 && Time.time >= nextSpawnTime)
        // �� ���� �����ְ� ���� �ð��� ���� ���� �ð����� ū ���
        {
            enemiesRemainingToSpawn--; // �� ���� �ϳ� ����
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
            // ���� ���� �ð��� ����ð� + ���� ���� ���� ����

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            // ���� �ν��Ͻ�ȭ�� ���� ����, �� �߾ӿ� ȸ���� ���� ��ġ
            spawnedEnemy.OnDeath += OnEnemyDeath;
            // ���� ���� �� �� �޼ҵ带 �߰�
        }
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

    void NextWave() // ���� ���̺� ���� �޼ҵ�
    {
        currentWaveNumber++; // ���̺� ���� ����
        print("Wave : " + currentWaveNumber); // ���� ���̺� ��ȣ ���
        if(currentWaveNumber - 1 < waves.Length) { // �迭 �ε��� ���� ������ ó��
            currentWave = waves[currentWaveNumber - 1];
            /* ���� ���̺� ���۷��� ����
               (���̺� ���ڴ� 1���� ���� �� ���̹Ƿ� -1 �Ͽ� �迭�� �ε����� �°� ����) */

            enemiesRemainingToSpawn = currentWave.enemyCount;
            // �̹� ���̺��� �� �� ����
            enemiesRemainingAlive = enemiesRemainingToSpawn;
            // ����ִ� ���� ���� ���� �� ���� ���� ����
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
