using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {
    public Image fadePlane;                         // 페이드 이미지(UI 배경) 오브젝트 레퍼런스
    public GameObject gameOverUI;                   // 게임 오버 텍스트, 버튼 오브젝트 레퍼런스

    public RectTransform newWaveBanner;             // 배너 UI 레퍼런스
    public TextMeshProUGUI newWaveTitle;            // 새 웨이브 타이틀 텍스트 레퍼런스
    public TextMeshProUGUI newWaveEnemyCount;       // 새 웨이브 적 카운트 텍스트 레퍼런스
    public TextMeshProUGUI scoreUI;                 // 점수 텍스트 레퍼런스
    public TextMeshProUGUI gameOverScoreUI;         // 최종 점수 텍스트 레퍼런스
    public RectTransform healthBar;                 // 체력 바 레퍼런스

    Spawner spawner;                                // 적 스폰기 레퍼런스
    Player player;                                  // 플레이어 레퍼런스

    void Start() {
        player = FindObjectOfType<Player>();        // 플레이어 오브젝트 할당
        player.OnDeath += OnGameOver;               // 플레이어 사망 이벤트 구독
    }

    void Update() {
        scoreUI.text = ScoreKeeper.score.ToString("D6");                    // 점수 텍스트 설정, "D6" 으로 6자리로 출력
        float healthPercent = 0;
        if(player != null) {
            healthPercent = player.health / player.startingHealth;          // 체력 퍼센트 계산
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);        // 체력 퍼센트에 따라 체력 바 크기 설정
    }

    void Awake() {
        spawner = FindObjectOfType<Spawner>(); // 스포너 레퍼런스에 스포너 오브젝트 찾아서 할당
        spawner.OnNewWave += OnNewWave; // 새 웨이브 시작 이벤트 구독
    }

    // ■ 새 웨이브 시작 메소드
    void OnNewWave(int waveNumber) {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" }; // 숫자 텍스트 문자열
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -"; // 새 웨이브 배너 타이틀 텍스트 설정
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        // 해당 웨이브가 무한 모드인지 판단, 맞다면 Infinite, 아니면 적 수를 출력한다
        newWaveEnemyCount.text = "Enemy : " + enemyCountString; // 새 웨이브 적 카운트 텍스트 설정

        StopCoroutine("AnimateNewWaveBanner"); // 새 웨이브 배너 애니메이션 코루틴 중지
        StartCoroutine("AnimateNewWaveBanner"); // 새 웨이브 배너 애니메이션 코루틴
    }

    // ■ 플레이어 사망(게임 오버) 시 UI 처리 메소드
    void OnGameOver() {
        Cursor.visible = true;                                              // 마우스 커서 보이도록 설정
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, .95f), 1));     // 배경 이미지 페이드 인 효과 코루틴 시작
        gameOverScoreUI.text = scoreUI.text;                                // 최종 점수 텍스트 설정
        scoreUI.gameObject.SetActive(false);                                // 게임 화면 점수 텍스트 오브젝트 비활성화
        healthBar.transform.parent.gameObject.SetActive(false);             // 체력 바 오브젝트 비활성화
        gameOverUI.SetActive(true);                                         // 게임 오버 텍스트, 버튼 오브젝트 활성화
    }

    // ■ 새 웨이브 배너 애니메이션 코루틴
    IEnumerator AnimateNewWaveBanner() {
        float delayTime = 1f; // 배너 대기 시간
        float speed = 2.5f; // 배너 움직이는 시간
        float animatePercent = 0; // 애니메이션 실행 퍼센트 설정
        int dir = 1; // 배너 이동 방향, 양수면 위, 음수면 아래쪽

        float endDelayTime = Time.time + 1 / speed + delayTime; // 배너 나타난 후 대기시간 설정

        while(animatePercent >= 0) { // 애니메이션 실행
            animatePercent += Time.deltaTime * speed * dir; // 애니메이션 실행 퍼센트 설정

            if (animatePercent >= 1) { // 애니메이션 실행 퍼센트가 1 이상인 경우
                animatePercent = 1; // 1로 설정
                if(Time.time > endDelayTime) { // 현재 시간이 배너가 나타나고 대기시간까지 지난 경우
                    dir = -1; // 방향을 음수로 설정, 아래로 이동하도록 함
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-190, 45, animatePercent); // 배너의 위치를 설정

            yield return null; // 다음 프레임으로 스킵 
        }
        
    }

    // ■ 이미지 페이드 효과 코루틴
    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time; // 페이드 효과 속도
        float percent = 0; // 이미지 색상(투명도) 변화 퍼센트

        while(percent < 1) { // 색상(투명도) 변화가 1(100%)미만일 때
            percent += Time.deltaTime * speed; // 색상(투명도) 변화율 계산
            fadePlane.color = Color.Lerp(from, to, percent); // 색상(투명도) 지정
            yield return null; // 다음 프레임으로 건너뜀
        }
    }

    // UI Input 부분

    // ■ 새 게임 시작 메소드
    public void startNewGame() {
        SceneManager.LoadScene("Game"); // 게임 씬, 게임을 다시 로드
    }

    // ■ 메인 메뉴 이동 메소드
    public void ReturnToMainMenu() {
        SceneManager.LoadScene("Menu"); // 메뉴 씬, 메뉴를 로드
    }
}
