using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    public Vector2 correctPosition;
    public Game1Manager puzzleManager;
    private bool inRightPosition;
    private bool selected;

    public static int correctPiecesCount = 0;
    public static int totalPieces = 6;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        puzzleManager = FindObjectOfType<Game1Manager>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        correctPosition = rectTransform.anchoredPosition;
    }

    private void Start()
    {
    }

   public void OnBeginDrag(PointerEventData eventData)
    {
        if (!inRightPosition)
        {
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;
            selected = true;
        }
        else
        {
            // 올바른 위치에 있으면 드래그 시작을 막습니다.
            eventData.pointerDrag = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!inRightPosition)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!inRightPosition)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            selected = false;
            CheckPosition();
        }
    }

    private void CheckPosition()
    {
        Debug.Log("CheckPosition 호출됨");
        if (Vector2.Distance(rectTransform.anchoredPosition, correctPosition) < 80f)
        {
            if (!inRightPosition)
            {
                Debug.Log("올바른 위치에 도달");
                inRightPosition = true;
                rectTransform.anchoredPosition = correctPosition;
                correctPiecesCount++;
                Debug.Log(correctPiecesCount);
                
                // 올바른 위치에 도달하면 드래그를 막습니다.
                canvasGroup.blocksRaycasts = true;

                // 클릭 효과음 재생
                puzzleManager.PuzzleSetSound();

                CheckPuzzleCompletion();
            }
        }
        else
        {
            if (inRightPosition)
            {
                inRightPosition = false;
                correctPiecesCount--;
                Debug.Log(correctPiecesCount);
            }
        }
    }

    private void CheckPuzzleCompletion()
    {
        if (correctPiecesCount == totalPieces)
        {
            puzzleManager.PuzzleCompleted();
        }
    }
}