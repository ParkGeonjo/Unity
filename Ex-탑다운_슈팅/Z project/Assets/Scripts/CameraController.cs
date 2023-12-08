using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player; // �÷��̾� ������Ʈ
    public Vector3 offset; // �÷��̾�� ī�޶� ����

    private void Start() {
        offset = this.transform.position - Player.transform.position; // ���� ����
    }

    private void Update() {
        if (Player != null) { // �÷��̾� ������Ʈ�� �ִ� ���
            this.transform.position = Player.transform.position + offset;
            // ���� ������Ʈ(ī�޶�) ��ġ�� �÷��̾� ��ġ + ���� ���� ����
        }
    }
}
