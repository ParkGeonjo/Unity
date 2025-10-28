using UnityEngine;
using System.Collections;

public class ImagePopEffect : MonoBehaviour
{
    public float popDuration = 0.7f; // 전체 애니메이션 시간

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (rectTransform != null)
            StartCoroutine(PlayPopIn());
    }

    private IEnumerator PlayPopIn()
    {
        rectTransform.localScale = Vector3.zero;

        float halfDuration = popDuration / 2f;
        float t = 0f;

        // 0% → 120%
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float progress = t / halfDuration;
            float scale = Mathf.Lerp(0f, 1.3f, progress);
            rectTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        t = 0f;

        // 120% → 100%
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            float progress = t / halfDuration;
            float scale = Mathf.Lerp(1.3f, 1f, progress);
            rectTransform.localScale = Vector3.one * scale;
            yield return null;
        }

        rectTransform.localScale = Vector3.one; // 정확히 100%로 고정
    }
}
