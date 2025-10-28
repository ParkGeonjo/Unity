using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class Game3Manager : MonoBehaviour
{
    [System.Serializable]
    public class Game3ObjectPrefabs                 // 게임 객체 저장
    {
        public string Game3Prefabs;               // 문제 답
        public Sprite Game3PrefabsObject;     // 문제 오브젝트
    }

    public RecordingAudio recordingAudio;           // 음성 녹음 객체 인스턴스
    public Game3ObjectPrefabs[] game3Objects;       // 게임 객체 저장 배열

    // 게임2 오브젝트 정보 파일 경로 지정 
    public string filePath = "Game3ObjectData.txt";

    public TextMeshProUGUI AnsText;                 // 정답 텍스트 인스턴스
    public TextMeshProUGUI HintText;                // 게임 힌트 텍스트 인스턴스
    public TextMeshProUGUI RoundText;               // 게임 라운드 텍스트 인스턴스
    public GameObject GameQuestionPanel;                 // 게임 문제 제시 패널 인스턴스
    public GameObject GameHelpPanel;                // 게임 도움말 패널 인스턴스
    public GameObject GameExitPanel;                // 게임 나가기 패널 인스턴스
    public GameObject GameEndPanel;                 // 게임 종료 패널 인스턴스

    public ParticleSystem AnsParticle;              // 정답 파티클 인스턴스
    public ParticleSystem EndParticle;              // 종료 파티클 인스턴스

    public Button RecordingButton;                  // 녹음 버튼
    public Button HelpButton;                       // 도움말 버튼
    public TextMeshProUGUI RecordingButtonText;     // 녹음 버튼 텍스트
    public TextMeshProUGUI GameEndText;     // 게임 결과 텍스트

    public AudioClip CorrectSound;                  // 정답 사운드
    public AudioClip IncorrectSound;                // 오답 사운드
    public AudioClip ClearSound;                    // 게임 종료 사운드
    public AudioClip ClearSound2;                   // 게임 종료 사운드 2
    public AudioClip Click;                         // 버튼 클릭 사운드
    public AudioClip Voice1;                        // 게임 대사 1
    public AudioClip Voice2;                        // 게임 대사 2
    public AudioClip Voice3;                        // 게임 대사 3

    public GameObject[] Game3Score;                 // 게임 점수 표시 스프라이트 (기본 별)
    public Sprite Game3ScoreYellow;                 // 게임 점수 표시 스프라이트 (정답 별)

    bool isRecord;                                  // 녹음 여부
    int gameNum;                                    // 현재 문제 번호
    int gameScore;                                  // 점수
    double percentageOfAnswer;                      // 음성인식을 통해 받은 텍스트와 정답의 유사도(%)
    GameObject QuestionObject;                      // 현재 문제 오브젝트

    private Coroutine autoStopCoroutine = null;     // 현재 실행 중인 녹음 자동 중지 코루틴 핸들

    void Awake()
    {
        // 녹음 여부 설정
        isRecord = false;

        // 문제 번호 (1, 2, 3)
        gameNum = 0;

        // 점수
        gameScore = 0;

        // 음성인식 실패 이벤트 구독
        recordingAudio.OnRecordFail += RecoirdingFail;

        // 음성인식 완료 이벤트 구독
        recordingAudio.OnRecordEnd += CheckAns;

        // ========== 오브젝트 불러오기 ==========

        // Utility 클래스를 통해 파일을 읽고 데이터를 Dictionary에 저장
        Dictionary<string, string> objectData = Utility.ReadObjectDataFromFile(filePath);

        // 게임 객체 저장 배열 game3Objects 초기화
        game3Objects = new Game3ObjectPrefabs[objectData.Count];

        // Dictionary 에서 정답, 객체를 가져와 game3Objects 에 저장.
        int index = 0;
        foreach (KeyValuePair<string, string> entry in objectData)
        {
            string objectName = entry.Key;
            string prefabName = entry.Value;

            // Resources 폴더에서 GameObject(프리팹)를 불러오기
            Sprite prefab = Resources.Load<Sprite>(prefabName);
            if (prefab != null)
            {
                game3Objects[index] = new Game3ObjectPrefabs
                {
                    Game3Prefabs = objectName,
                    Game3PrefabsObject = prefab
                };
                Debug.Log("오브젝트 " + prefab.name + " 로드 완료.");
                index++;
            }
            else
            {
                Debug.LogWarning($"Prefab '{prefabName}' could not be found in Resources folder.");
            }
        }
        // ========================================


        // ========== 오브젝트 랜덤 선택 ==========

        // 로드된 딕셔너리 항목 수 확인
        Debug.Log($"game3Objects에 저장된 객체 수: {game3Objects.Length}");

        // 시드
        int seed = Random.Range(1, 9999);

        // 딕셔너리에서 무작위로 3개 항목 가져오기
        Game3ObjectPrefabs[] randomObjects = Utility.ShuffleArray(game3Objects, seed);

        if (randomObjects == null || randomObjects.Length < 3)
        {
            Debug.LogError("랜덤으로 선택된 객체가 3개 미만입니다.");
            return;
        }

        // 무작위로 선택된 항목을 배열에 저장
        index = 0;
        foreach (var entry in randomObjects)
        {
            game3Objects[index].Game3Prefabs = entry.Game3Prefabs;
            game3Objects[index].Game3PrefabsObject = entry.Game3PrefabsObject;
            Debug.Log((index + 1)+ "번 오브젝트 : " + game3Objects[index].Game3PrefabsObject.name);
            index++;
        }
        // ========================================

        // 새 게임 시작
        StartGame1();
    }

    // 새 게임 시작
    public void StartGame1()
    {
        if (gameNum < 3)
        {
            // 문제 오브젝트 제시
            GameQuestionPanel.GetComponent<Image>().sprite = game3Objects[gameNum].Game3PrefabsObject;

            // 안내 텍스트 변환
            AnsText.text = "무엇이 필요할까요 ?";

            // 대사 재생 (이것은 무엇일까요)
            StartCoroutine(VoicePlay(Voice1, 1));

            // 문제 진행상황 텍스트
            RoundText.text = (gameNum + 1) + " / 3";
        }
        else
        {
            GameEnd();
        }
    }

    // 음성인식 시작, 종료
    public void Recording()
    {
        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);

        // 녹음 자동 중지 코루틴을 위한 대기 시간
        float recordingTimeout = 10f;

        if (!isRecord)
        {
            isRecord = true;

            // 음악 볼륨 서서히 줄이기
            AudioManager.instance.FadeMusicVolumeMultiplier(0.1f, 1f);

            // 안내 텍스트 변환
            AnsText.text = "녹음중...";

            // 녹음 버튼 텍스트 변환
            RecordingButtonText.text = "녹음 종료";

            // 도움말 버튼 비활성화
            HelpButton.interactable = false;

            // 음성녹음 시작
            recordingAudio.Recording();

            // 이전 코루틴이 실행 중이라면 중지
            if (autoStopCoroutine != null)
            {
                StopCoroutine(autoStopCoroutine);
            }

            // 새로운 자동 중지 코루틴 시작
            autoStopCoroutine = StartCoroutine(StopRecordingAfterDelay(recordingTimeout));
        }
        else
        {
            isRecord = false;

            // 음악 볼륨 원래대로 올리기
            AudioManager.instance.FadeMusicVolumeMultiplier(1f, 1f);

            // 안내 텍스트 변환
            AnsText.text = "정답 확인중...";

            // 녹음 버튼 텍스트 변환
            RecordingButtonText.text = "녹음 시작";

            // 녹음 버튼 비활성화
            RecordingButton.interactable = false;

            // 음성녹음 종료
            recordingAudio.Recording();
        }
    }

    private IEnumerator StopRecordingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 지정 시간 후에 자동으로 녹음 중지
        if (isRecord)
        {
            StopRecordingManually();
        }
    }

    // 녹음을 종료하는 메서드
    private void StopRecordingManually()
    {
        isRecord = false;

        // 음악 볼륨 원래대로 올리기
        AudioManager.instance.FadeMusicVolumeMultiplier(1f, 1f);

        // 안내 텍스트 변환
        AnsText.text = "정답 확인중...";
        RecordingButtonText.text = "녹음 시작";
        RecordingButton.interactable = false;

        // 음성녹음 종료
        recordingAudio.Recording();

        // 자동 중지 코루틴이 완료되었으므로 null로 설정
        autoStopCoroutine = null;
    }

    // 음성인식 실패 시 호출, 답 체크하여 띄우고 문제 넘어가기
    public void RecoirdingFail(object sender, EventArgs eventArgs)
    {
        isRecord = false;

        // 음악 볼륨 원래대로 올리기
        AudioManager.instance.FadeMusicVolumeMultiplier(1f, 1f);

        // 안내 텍스트 변환
        AnsText.text = "무엇이 필요할까요 ?";

        // 오답 효과음 재생
        AudioManager.instance.PlaySound(IncorrectSound, transform.position);

        // 오답 대사 재생 (다시 말해볼까요?)
        StartCoroutine(VoicePlay(Voice3, 1));

        // 녹음 버튼 텍스트 변환
        RecordingButtonText.text = "녹음 시작";

        // 녹음 버튼 활성화
        RecordingButton.interactable = true;

        // 도움말 버튼 활성화
        HelpButton.interactable = true;
    }

    // 음성인식 완료 시 호출, 답 체크하여 띄우고 문제 넘어가기 
    public void CheckAns(object sender, EventArgs eventArgs)
    {
        // 유사도 측정
        percentageOfAnswer = PronunciationSimilarity.CalculatePronunciationSimilarity(recordingAudio.recordText, game3Objects[gameNum].Game3Prefabs);

        Debug.Log("정답 : " + game3Objects[gameNum].Game3Prefabs + " / 유사도 : " + percentageOfAnswer);

        // 50%이상
        if(percentageOfAnswer >= 0.5f)
        {
            if (!AnsParticle.isPlaying)
                AnsParticle.Play();

            // 정답 출력
            AnsText.text = game3Objects[gameNum].Game3Prefabs;

            // 정답 효과음 재생
            AudioManager.instance.PlaySound(CorrectSound, transform.position);

            // 정답 대사 재생 (잘했어요 정답이에요)
            StartCoroutine(VoicePlay(Voice2, 1));

            gameScore++;

            Invoke("NextGame", 5);
        }
        else
        {
            // 정답 출력
            AnsText.text = "틀렸어요! 정답은 \"" + game3Objects[gameNum].Game3Prefabs +"\"";

            // 오답 효과음 재생
            AudioManager.instance.PlaySound(IncorrectSound, transform.position);

            Invoke("NextGame", 5);
        }
    }





    // 다음 게임 넘어가기
    public void NextGame()
    {
        // 텍스트 변환
        RecordingButtonText.text = "녹음 시작";

        // 녹음 버튼 활성화
        RecordingButton.interactable = true;

        // 도움말 버튼 활성화
        HelpButton.interactable = true;

        // 맞춘 문제 오브젝트 비활성화
        Destroy(QuestionObject);

        // 게임 번호 증가, 새 게임 시작
        gameNum++;
        StartGame1();
    }

    // 도움말 열기
    public void HelpPanelOpen()
    {
        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);

        // 도움말 패널 활성화
        GameHelpPanel.SetActive(true);

        // 초성 힌트
        HintText.text = "문제 초성힌트\n\n" + HangulExtractor.ExtractInitials(game3Objects[gameNum].Game3Prefabs);
    }

    // 도움말 닫기
    public void HelpPanelClose()
    {
        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);

        // 도움말 패널 비활성화
        GameHelpPanel.SetActive(false);
    }

    // 게임 나가기 시도
    public void GameExitPanelOpen()
    {
        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);

        // 게임 종료 패널 활성화
        GameExitPanel.SetActive(true);
    }

    // 게임 나가기 취소
    public void GameExitPanelClose()
    {
        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);

        // 게임 종료 패널 활성화
        GameExitPanel.SetActive(false);
    }

    // 게임 종료
    void GameEnd()
    {
        // 게임 종료 패널 활성화
        GameEndPanel.SetActive(true);

        // 파티클 재생
        if(!EndParticle.isPlaying)
            EndParticle.Play();

        // 축하 효과음 재생
        AudioManager.instance.PlaySound(ClearSound, transform.position);
        AudioManager.instance.PlaySound(ClearSound2, transform.position);

        int lastScore = PlayerPrefs.GetInt("game3 score", 0);
        if(gameScore == 3) {
            GameEndText.text = "모든 문제를 맞혔어요!\n축하합니다!";
        }
        else if(lastScore >= gameScore) {
            GameEndText.text = "잘했어요!\n좀 더 노력해볼까요?";
        }
        else if(lastScore < gameScore) {
            GameEndText.text = "좋아요!\n예전보다 더 잘했어요!";
            PlayerPrefs.SetInt("game3 score", gameScore);
            PlayerPrefs.Save();
        }

        for(int i = 0; i < gameScore; i++) {
            Game3Score[i].GetComponent<Image>().sprite = Game3ScoreYellow;
        }
    }

    // 게임 나가기
    public void GameExit()
    {
        // 클릭 효과음 재생
        AudioManager.instance.PlaySound(Click, transform.position);

        if (isRecord)
        {
            recordingAudio.ExitRecording();
        }

        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    // 음성 대사
    private IEnumerator VoicePlay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 지정 시간 후에 자동으로 재생
        if (!isRecord)
        {
            AudioManager.instance.PlaySound(clip, transform.position);
        }
    }
}
