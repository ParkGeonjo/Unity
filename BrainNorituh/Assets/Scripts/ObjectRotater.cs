using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float rotationSpeed = 20.0f; // 회전 속도 조절 변수

    private float speed = 0f;           // 현재 회전 속도 (z축 회전)
    private bool dragging = false;      // 드래그 중인지 여부를 저장
    private Vector2 lastMousePosition;  // 마지막 마우스/터치 위치 저장

    void OnMouseDown()
    {
        dragging = true;
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        // 드래그 중일 때 회전 속도를 계산
        if (Input.GetMouseButton(0) && dragging)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 delta = currentMousePosition - lastMousePosition;

            // 드래그 이동량의 x축만 사용하여 z축 회전 속도 설정
            speed = -delta.x * rotationSpeed;

            // 오브젝트를 월드 z축을 기준으로 회전
            transform.Rotate(Vector3.forward, speed * Time.deltaTime, Space.World);

            // 마지막 마우스 위치 업데이트
            lastMousePosition = currentMousePosition;
        }

        // 마우스 버튼을 떼면 드래그 종료
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            speed = 0f; // 드래그가 끝나면 즉시 회전 속도 초기화
        }
    }
}
