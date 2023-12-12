using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))] // GunController ��ũ��Ʈ�� ����
[RequireComponent(typeof(PlayerController))] // PlayerController ��ũ��Ʈ�� ����
public class Player : LivingEntity
{
    public float moveSpeed = 5.0f; // �÷��̾� �̵��ӵ�

    public Crosshair crosshairs; // ������ ���۷���

    Camera viewCamera; // ī�޶�
    PlayerController controller; // �÷��̾� ��Ʈ�ѷ�
    GunController gunController; // �� ��Ʈ�ѷ�

    protected override void Start() { // override �� �θ� Ŭ������ �޼ҵ带 ������.
        base.Start(); // base �� ���� �θ� Ŭ������ ���� �޼ҵ带 ȣ��.
    }

    void Awake() {
        controller = GetComponent<PlayerController>();
        // ���� ������Ʈ�� �÷��̾� ��Ʈ�ѷ��� ������
        gunController = GetComponent<GunController>();
        // ���� ������Ʈ�� �� ��Ʈ�ѷ��� ������
        viewCamera = Camera.main;
        // ī�޶�
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        // Spawner �� �� ���̺� ���� �̺�Ʈ ����
    }

    // �� �� ���̺� ���� �� �޼ҵ�
    void OnNewWave(int waveNumber) {
        health = startingHealth; // ü�� �ʱ�ȭ
        gunController.EquipGun(waveNumber - 1); // ������ �� ����
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
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
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
            crosshairs.transform.position = point;
            // ������ ��ġ ����
            crosshairs.DetectTarget(ray);
            // ������ ����ĳ��Ʈ

            if((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1) {
            // ������(Ŀ��)�� �÷��̾� ���� �Ÿ��� 1 �̻��� ��
                gunController.Aim(point);
                // �� ���� ���� ��ġ ����
            }
        }

        // ���� ���� �Է�
        if (Input.GetMouseButton(0)) // ���콺 ���� ��ư Ŭ�� ��
        {
            gunController.OnTriggerHold(); // ��Ƽ� ��� �޼ҵ� ȣ��
        }

        if (Input.GetMouseButtonUp(0)) // ���콺 ���� ��ư Ŭ�� �� ���� �� ��
        {
            gunController.OnTriggerRelease(); // ��Ƽ� ���� �޼ҵ� ȣ��
        }

        if (Input.GetKeyDown(KeyCode.R)) // Ű���� R ��ư ���� ���
        {
            gunController.Reload(); // ������ �޼ҵ� ȣ��
        }
    }
}
