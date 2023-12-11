using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
    public Transform weaponHold; // �÷��̾� ��(��)��ġ
    public Gun startingGun; // ���� ��
    Gun equippedGun; // �����ϴ� ��

    private void Start() {
        if (startingGun != null) { // ���� ���� ������ ���
            EquipGun(startingGun); // ���� ���� ����
        }
    }

    // �� �� ���� �޼ҵ�
    public void EquipGun(Gun gunToEquip) {
        if (equippedGun != null) { // �������� ���� �ִٸ�
            Destroy(equippedGun.gameObject); // �������� ��(������Ʈ)�� ����(�ı�)
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        // �������� �ѿ� ���޹��� �� ������Ʈ�� �ν��Ͻ�ȭ�Ͽ� ����
        equippedGun.transform.parent = weaponHold;
        // �������� ���� ��ġ�� ����, �θ� ������Ʈ�� �����Ͽ� ��ġ�� �θ� ������Ʈ�� ��ġ��.
    }

    // �� ��Ƽ� ��� �޼ҵ�
    public void OnTriggerHold()
    {
        if(equippedGun != null) // �������� ���� �ִٸ�
        {
            equippedGun.OnTriggerHold(); // ��Ƽ� ���
        }
    }

    // �� ��Ƽ� ���� �޼ҵ�
    public void OnTriggerRelease()
    {
        if (equippedGun != null) // �������� ���� �ִٸ�
        {
            equippedGun.OnTriggerRelease(); // ��Ƽ� ����
        }
    }

    // �� ���� ���� ��ȯ �޼ҵ�
    public float GunHeight {
        get {
            return weaponHold.position.y; // ������ y��ǥ(����)��ȯ
        }
    }

    // �� ���� ���� �޼ҵ�
    public void Aim(Vector3 aimPoint) {
        if (equippedGun != null) { // �������� ���� �ִٸ�
            equippedGun.Aim(aimPoint); // ������ ���� ���� ���� �޼ҵ� ȣ��
        }
    }
}
