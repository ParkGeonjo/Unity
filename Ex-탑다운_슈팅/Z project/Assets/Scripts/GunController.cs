using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
    public Transform weaponHold; // 플레이어 손(총)위치
    public Gun startingGun; // 시작 총
    Gun equippedGun; // 착용하는 총

    private void Start() {
        if (startingGun != null) { // 시작 총이 설정된 경우
            EquipGun(startingGun); // 시작 총을 착용
        }
    }

    public void EquipGun(Gun gunToEquip) { // 총 착용 메소드
        if (equippedGun != null) { // 착용중인 총이 있다면
            Destroy(equippedGun.gameObject); // 착용중인 총(오브젝트)를 제거(파괴)
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        // 착용중인 총에 전달받은 총 오브젝트를 인스턴스화하여 생성
        equippedGun.transform.parent = weaponHold;
        // 착용중인 총의 위치를 설정, 부모 오브젝트를 설정하여 위치를 부모 오브젝트의 위치로.
    }

    public void OnTriggerHold() // 방아쇠 당김 메소드
    {
        if(equippedGun != null) // 착용중인 총이 있다면
        {
            equippedGun.OnTriggerHold(); // 방아쇠 당김
        }
    }
    public void OnTriggerRelease() // 방아쇠 놓음 메소드
    {
        if (equippedGun != null) // 착용중인 총이 있다면
        {
            equippedGun.OnTriggerRelease(); // 방아쇠 놓음
        }
    }
}
