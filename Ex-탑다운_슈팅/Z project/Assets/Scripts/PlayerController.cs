using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // ���� ������Ʈ�� Rigidbody ����

public class PlayerController : MonoBehaviour
{
    Vector3 velocity; // �÷��̾� �̵��ӵ�
    Rigidbody myRigidbody; // ������ٵ�
    void Start() {
        myRigidbody = GetComponent<Rigidbody>();
        // ���� ������Ʈ�� ������ٵ� ������
    }

    public void Move(Vector3 _velocity) {
        // �÷��̾� �̵� �޼ҵ� (��ǻ� �̵��ӵ� ����)
        velocity = _velocity; // �̵��ӵ��� ���޹��� �̵��ӵ� ����
    }

    public void LookAt(Vector3 lookPoint){ // �÷��̾� ���� ��ȯ �޼ҵ�
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        // ���ο� ����(����) ����, ���޹��� ��ġ���� y ���� ���� �÷��̾� y ������ ����.
        transform.LookAt(heightCorrectedPoint);
        // LookAt �޼ҵ忡 ��ġ(����)�����Ͽ� ���� ��ȯ.
    }

    private void FixedUpdate() {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.fixedDeltaTime);
        // MovePosition �� ��ġ + �ӵ� + fixedDeltaTime �����Ͽ� �÷��̾� �̵�
        // FixedUpdate �� ���� ������Ʈ�� ���� �̵�, ������ ���̿��� ������ �̵� ����.
    }
}
