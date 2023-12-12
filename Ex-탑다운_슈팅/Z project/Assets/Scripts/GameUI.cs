using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    public Image fadePlane; // 페이드 이미지(UI 배경) 오브젝트 레퍼런스
    public GameObject gameOverUI; // 게임 오버 텍스트, 버튼 오브젝트 레퍼런스

    public RectTransform newWaveBanner; // 배너 UI 레퍼런스
    public Text newWaveTitle; // 새 웨이브 타이틀 텍스트 레퍼런스
    public Text newWaveEnemyCount; // 새 웨이브 적 카운트 텍스트 레퍼런스

    Spawner spawner; // 적 스폰기 레퍼런스

    void Start() {
        FindObjectOfType<Player>().OnDeath += OnGameOver; // 플레이어 사망 이벤트 구독
    }

    void Awake() {
        spawner = FindObjectOfType<Spawner>(); // 스포너 레퍼런스에 스포너 오브젝트 찾아서 할당
        spawner.OnNewWave += OnNewWave; // 새 웨이브 시작 이벤트 구독
    }

    // ■ 새 웨이브 시작 메소드
    void OnNewWave(int waveNumber) {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" }; // 숫자 텍스트 문자열
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -"; // 새 웨이브 배너 타이틀 텍스트 설정
        newWaveEnemyCount.text = "Enemy : " + spawner.waves[waveNumber - 1].enemyCount; // 새 웨이브 적 카운트 텍스트 설정

        StartCoroutine(AnimateNewWaveBanner()); // 새 웨이브 배너 애니메이션 코루틴
    }

    // ■ 플레이어 사망(게임 오버) 시 UI 처리 메소드
    void OnGameOver() {
        Cursor.visible = true; // 마우스 커서 보이도록 설정
        StartCoroutine(Fade(Color.clear, Color.black, 1)); // 배경 이미지 페이드 인 효과 코루틴 시작
        gameOverUI.SetActive(true); // 게임 오버 텍스트, 버튼 오브젝트 활성화
    }

    // ■ 새 웨이브 배너 애니메이션 코루틴
    IEnumerator AnimateNewWaveBanner() {
        float delayTime = 1f; // 배너 대기 시간
        float speed = 2.5f; // 배너 움직이는 시간
        float animatePercent = 0; // 애니메이션 실행 퍼센트 설정
        int dir = 1; // 이동 방향

        float endDelayTime = Time.time + 1 / speed + delayTime; // 배너 대기시간 설정

        while(animatePercent < 1) { // 애니메이션 실행
            animatePercent += Time.deltaTime * speed * dir; // 애니메이션 실행 퍼센트 설정
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
        Application.LoadLevel("Game"); // 게임 씬, 게임을 다시 로드
    }
}
