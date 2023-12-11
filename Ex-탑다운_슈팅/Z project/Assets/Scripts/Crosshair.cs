using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public LayerMask targetMask; // Ÿ��(��)����ũ ���۷���

    public SpriteRenderer dot; // �� ��������Ʈ ������ ���۷���
    public Color dotHighlightColour; // �� ���̶���Ʈ ����
    Color originalDotColour; // �� ���� ����

    void Start() {
        Cursor.visible = false; // Ŀ�� �Ⱥ��̵��� ����
        originalDotColour = dot.color; // �� ���� ���� ����
    }

    void Update() {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime); // ȸ��
    }

    // �� ������ ����ĳ��Ʈ �޼ҵ�
    public void DetectTarget(Ray ray) {
        if (Physics.Raycast(ray, 100, targetMask)) { // Ÿ�� ����ũ�� ���̰� ���� ���
            dot.color = dotHighlightColour; // �� ���� ���̶���Ʈ �������� ����
        }
        else {
            dot.color = originalDotColour; // �� ���� ���� �������� ����
        }
    }
}
