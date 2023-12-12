using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
    public Transform weaponHold; // 플레이어 손(총)위치
    public Gun[] allGuns; // 총기 배열
    Gun equippedGun; // 착용하는 총

    private void Start() {

    }

    // ■ 총 착용 메소드
    public void EquipGun(Gun gunToEquip) {
        if (equippedGun != null) { // 착용중인 총이 있다면
            Destroy(equippedGun.gameObject); // 착용중인 총(오브젝트)를 제거(파괴)
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        // 착용중인 총에 전달받은 총 오브젝트를 인스턴스화하여 생성
        equippedGun.transform.parent = weaponHold;
        // 착용중인 총의 위치를 설정, 부모 오브젝트를 설정하여 위치를 부모 오브젝트의 위치로.
    }

    // ■ 총 착용 메소드
    public void EquipGun(int weaponIndex) {
        EquipGun(allGuns[weaponIndex]); // 총 장착
    }

        // ■ 방아쇠 당김 메소드
        public void OnTriggerHold()
    {
        if(equippedGun != null) // 착용중인 총이 있다면
        {
            equippedGun.OnTriggerHold(); // 방아쇠 당김
        }
    }

    // ■ 방아쇠 놓음 메소드
    public void OnTriggerRelease()
    {
        if (equippedGun != null) // 착용중인 총이 있다면
        {
            equippedGun.OnTriggerRelease(); // 방아쇠 놓음
        }
    }

    // ■ 무기 높이 반환 메소드
    public float GunHeight {
        get {
            return weaponHold.position.y; // 무기의 y좌표(높이)반환
        }
    }

    // ■ 에임 보정 메소드
    public void Aim(Vector3 aimPoint) {
        if (equippedGun != null) { // 착용중인 총이 있다면
            equippedGun.Aim(aimPoint); // 착용한 총의 에임 보정 메소드 호출
        }
    }

    // ■ 재장전 메소드
    public void Reload() {
        if (equippedGun != null) { // 착용중인 총이 있다면
            equippedGun.Reload(); // 착용한 총의 재장전 메소드 호출
        }
    }
}
