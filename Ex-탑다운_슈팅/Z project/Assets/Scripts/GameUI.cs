using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
    public Image fadePlane;                         // ���̵� �̹���(UI ���) ������Ʈ ���۷���
    public GameObject gameOverUI;                   // ���� ���� �ؽ�Ʈ, ��ư ������Ʈ ���۷���

    public RectTransform newWaveBanner;             // ��� UI ���۷���
    public TextMeshProUGUI newWaveTitle;            // �� ���̺� Ÿ��Ʋ �ؽ�Ʈ ���۷���
    public TextMeshProUGUI newWaveEnemyCount;       // �� ���̺� �� ī��Ʈ �ؽ�Ʈ ���۷���
    public TextMeshProUGUI scoreUI;                 // ���� �ؽ�Ʈ ���۷���
    public TextMeshProUGUI gameOverScoreUI;         // ���� ���� �ؽ�Ʈ ���۷���
    public RectTransform healthBar;                 // ü�� �� ���۷���

    Spawner spawner;                                // �� ������ ���۷���
    Player player;                                  // �÷��̾� ���۷���

    void Start() {
        player = FindObjectOfType<Player>();        // �÷��̾� ������Ʈ �Ҵ�
        player.OnDeath += OnGameOver;               // �÷��̾� ��� �̺�Ʈ ����
    }

    void Update() {
        scoreUI.text = ScoreKeeper.score.ToString("D6");                    // ���� �ؽ�Ʈ ����, "D6" ���� 6�ڸ��� ���
        float healthPercent = 0;
        if(player != null) {
            healthPercent = player.health / player.startingHealth;          // ü�� �ۼ�Ʈ ���
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);        // ü�� �ۼ�Ʈ�� ���� ü�� �� ũ�� ����
    }

    void Awake() {
        spawner = FindObjectOfType<Spawner>(); // ������ ���۷����� ������ ������Ʈ ã�Ƽ� �Ҵ�
        spawner.OnNewWave += OnNewWave; // �� ���̺� ���� �̺�Ʈ ����
    }

    // �� �� ���̺� ���� �޼ҵ�
    void OnNewWave(int waveNumber) {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" }; // ���� �ؽ�Ʈ ���ڿ�
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -"; // �� ���̺� ��� Ÿ��Ʋ �ؽ�Ʈ ����
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        // �ش� ���̺갡 ���� ������� �Ǵ�, �´ٸ� Infinite, �ƴϸ� �� ���� ����Ѵ�
        newWaveEnemyCount.text = "Enemy : " + enemyCountString; // �� ���̺� �� ī��Ʈ �ؽ�Ʈ ����

        StopCoroutine("AnimateNewWaveBanner"); // �� ���̺� ��� �ִϸ��̼� �ڷ�ƾ ����
        StartCoroutine("AnimateNewWaveBanner"); // �� ���̺� ��� �ִϸ��̼� �ڷ�ƾ
    }

    // �� �÷��̾� ���(���� ����) �� UI ó�� �޼ҵ�
    void OnGameOver() {
        Cursor.visible = true;                                              // ���콺 Ŀ�� ���̵��� ����
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, .95f), 1));     // ��� �̹��� ���̵� �� ȿ�� �ڷ�ƾ ����
        gameOverScoreUI.text = scoreUI.text;                                // ���� ���� �ؽ�Ʈ ����
        scoreUI.gameObject.SetActive(false);                                // ���� ȭ�� ���� �ؽ�Ʈ ������Ʈ ��Ȱ��ȭ
        healthBar.transform.parent.gameObject.SetActive(false);             // ü�� �� ������Ʈ ��Ȱ��ȭ
        gameOverUI.SetActive(true);                                         // ���� ���� �ؽ�Ʈ, ��ư ������Ʈ Ȱ��ȭ
    }

    // �� �� ���̺� ��� �ִϸ��̼� �ڷ�ƾ
    IEnumerator AnimateNewWaveBanner() {
        float delayTime = 1f; // ��� ��� �ð�
        float speed = 2.5f; // ��� �����̴� �ð�
        float animatePercent = 0; // �ִϸ��̼� ���� �ۼ�Ʈ ����
        int dir = 1; // ��� �̵� ����, ����� ��, ������ �Ʒ���

        float endDelayTime = Time.time + 1 / speed + delayTime; // ��� ��Ÿ�� �� ���ð� ����

        while(animatePercent >= 0) { // �ִϸ��̼� ����
            animatePercent += Time.deltaTime * speed * dir; // �ִϸ��̼� ���� �ۼ�Ʈ ����

            if (animatePercent >= 1) { // �ִϸ��̼� ���� �ۼ�Ʈ�� 1 �̻��� ���
                animatePercent = 1; // 1�� ����
                if(Time.time > endDelayTime) { // ���� �ð��� ��ʰ� ��Ÿ���� ���ð����� ���� ���
                    dir = -1; // ������ ������ ����, �Ʒ��� �̵��ϵ��� ��
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-190, 45, animatePercent); // ����� ��ġ�� ����

            yield return null; // ���� ���������� ��ŵ 
        }
        
    }

    // �� �̹��� ���̵� ȿ�� �ڷ�ƾ
    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time; // ���̵� ȿ�� �ӵ�
        float percent = 0; // �̹��� ����(����) ��ȭ �ۼ�Ʈ

        while(percent < 1) { // ����(����) ��ȭ�� 1(100%)�̸��� ��
            percent += Time.deltaTime * speed; // ����(����) ��ȭ�� ���
            fadePlane.color = Color.Lerp(from, to, percent); // ����(����) ����
            yield return null; // ���� ���������� �ǳʶ�
        }
    }

    // UI Input �κ�

    // �� �� ���� ���� �޼ҵ�
    public void startNewGame() {
        SceneManager.LoadScene("Game"); // ���� ��, ������ �ٽ� �ε�
    }

    // �� ���� �޴� �̵� �޼ҵ�
    public void ReturnToMainMenu() {
        SceneManager.LoadScene("Menu"); // �޴� ��, �޴��� �ε�
    }
}
