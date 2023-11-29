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

    public void EquipGun(Gun gunToEquip) { // �� ���� �޼ҵ�
        if (equippedGun != null) { // �������� ���� �ִٸ�
            Destroy(equippedGun.gameObject); // �������� ��(������Ʈ)�� ����(�ı�)
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        // �������� �ѿ� ���޹��� �� ������Ʈ�� �ν��Ͻ�ȭ�Ͽ� ����
        equippedGun.transform.parent = weaponHold;
        // �������� ���� ��ġ�� ����, �θ� ������Ʈ�� �����Ͽ� ��ġ�� �θ� ������Ʈ�� ��ġ��.
    }

    public void Shoot() // �Ѿ� �߻� �޼ҵ�
    {
        if(equippedGun != null) // �������� ���� �ִٸ�
        {
            equippedGun.Shoot(); // ���� Shoot �޼ҵ�� �Ѿ��� �߻�.
        }
    }
}
