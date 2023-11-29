using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))] // GunController ��ũ��Ʈ�� ����
[RequireComponent(typeof(PlayerController))] // PlayerController ��ũ��Ʈ�� ����
public class Player : LivingEntity
{
    public float moveSpeed = 5.0f; // �÷��̾� �̵��ӵ�
    Camera viewCamera; // ī�޶�
    PlayerController controller; // �÷��̾� ��Ʈ�ѷ�
    GunController gunController; // �� ��Ʈ�ѷ�

    protected override void Start() { // override �� �θ� Ŭ������ �޼ҵ带 ������.
        base.Start(); // base �� ���� �θ� Ŭ������ ���� �޼ҵ带 ȣ��.
        controller = GetComponent<PlayerController>();
        // ���� ������Ʈ�� �÷��̾� ��Ʈ�ѷ��� ������
        gunController = GetComponent<GunController>();
        // ���� ������Ʈ�� �� ��Ʈ�ѷ��� ������
        viewCamera = Camera.main;
        // ī�޶�
    }

    void Update() {
        // �̵� �Է�
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxis("Vertical"));
        // GetAxisRaw �� ������ X
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        // normalized �� �������ͷ� ��ȯ, �̵� �ӵ��� ���
        controller.Move(moveVelocity);
        // �̵� �ӵ��� �̵� �޼ҵ�� ����

        // ���� �Է�
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        // ī�޶� -> ���콺 Ŀ�� ��ġ�� ���� �߻�.
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        // ����� �������� ����.
        float rayDistance;
        // ������ �� ���̿� ����� ������������ �Ÿ�

        if (groundPlane.Raycast(ray, out rayDistance)) {
            // ���̰� ���� �����ϴ� ��� �������������� �Ÿ��� rayDistance �� ����.
            Vector3 point = ray.GetPoint(rayDistance);
            // GetPoint �� �������������� �Ÿ��� ���������� ��ġ�� ����.
            // ī�޶���� ������������ ������ ǥ�� : Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            // �÷��̾� ��Ʈ�ѷ��� ���� ��ȯ �޼ҵ忡 ���� ����(����) ����.
        }

        // ���� ���� �Է�
        if (Input.GetMouseButton(0)) // ���콺 ���� ��ư Ŭ�� Ȯ��
        {
            gunController.Shoot(); // �Ѿ� �߻� �޼ҵ� ȣ��
        }
    }
}
