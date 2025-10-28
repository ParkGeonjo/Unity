using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public List<GameObject> canecorsoPieces;
    public List<GameObject> deerPieces;
    public List<GameObject> horsePieces; // 말 퍼즐 조각들 추가
    public List<List<GameObject>> allPuzzleSets; // 모든 퍼즐 세트를 담는 리스트
    public List<GameObject> currentPuzzlePieces; // 현재 선택된 퍼즐 조각들
    public Transform[] leftPositions;
    public Transform[] rightPositions;

    private int currentPuzzleIndex = 0; // 현재 풀고 있는 퍼즐의 인덱스

    void Start()
    {
        InitializePuzzleSets();
        StartNextPuzzle();
    }

    void InitializePuzzleSets()
    {
        allPuzzleSets = new List<List<GameObject>>
        {
            canecorsoPieces,
            deerPieces,
            horsePieces
        };
        // 퍼즐 세트 순서를 랜덤하게 섞기
        for (int i = 0; i < allPuzzleSets.Count; i++)
        {
            int randomIndex = Random.Range(i, allPuzzleSets.Count);
            var temp = allPuzzleSets[i];
            allPuzzleSets[i] = allPuzzleSets[randomIndex];
            allPuzzleSets[randomIndex] = temp;
        }
    }

    void StartNextPuzzle()
    {
        if (currentPuzzleIndex < allPuzzleSets.Count)
        {
            currentPuzzlePieces = new List<GameObject>(allPuzzleSets[currentPuzzleIndex]);
            SetPuzzleVisibility();
            ShuffleAndPlacePieces();
            currentPuzzleIndex++;
        }
        else
        {
            Debug.Log("모든 퍼즐을 완료했습니다!");
            // 여기에 모든 퍼즐을 완료했을 때의 로직을 추가할 수 있습니다.
        }
    }

    void SetPuzzleVisibility()
    {
        foreach (var puzzleSet in allPuzzleSets)
        {
            foreach (GameObject piece in puzzleSet)
            {
                piece.SetActive(currentPuzzlePieces.Contains(piece));
            }
        }
    }

    void ShuffleAndPlacePieces()
    {
        List<GameObject> shuffledPieces = new List<GameObject>(currentPuzzlePieces);
        for (int i = 0; i < shuffledPieces.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledPieces.Count);
            var temp = shuffledPieces[i];
            shuffledPieces[i] = shuffledPieces[randomIndex];
            shuffledPieces[randomIndex] = temp;
        }

        for (int i = 0; i < 3 && i < shuffledPieces.Count; i++)
        {
            RectTransform pieceRect = shuffledPieces[i].GetComponent<RectTransform>();
            pieceRect.anchoredPosition = leftPositions[i].GetComponent<RectTransform>().anchoredPosition;
        }

        for (int i = 3; i < 6 && i < shuffledPieces.Count; i++)
        {
            RectTransform pieceRect = shuffledPieces[i].GetComponent<RectTransform>();
            pieceRect.anchoredPosition = rightPositions[i - 3].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    // 퍼즐이 완성되었을 때 호출할 메서드
    public void PuzzleCompleted()
    {
        StartNextPuzzle();
    }
}