using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScreenSize : MonoBehaviour
{
    private void Awake()
    {
        Camera cam = GetComponent<Camera>();
        // 카메라 컴포넌트의 Viewport Rect
        Rect rt = cam.rect;

        // 현재 세로 모드 9:16, 반대로 하고 싶으면 16:9를 입력.
        float scale_height = ((float)Screen.width / Screen.height) / ((float)9 / 16);
        
        // (가로 / 세로)
        float scale_width = 1f / scale_height;
        if (scale_height < 1)
        {
            rt.height = scale_height;
            rt.y = (1f - scale_height) / 2f;
        }
        else
        {
            rt.width = scale_width;
            rt.x = (1f - scale_width) / 2f;
        }
        cam.rect = rt;
    }
}
