using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle; // �Ѿ� �߻� ��ġ
    public Projectile projectile; // �Ѿ�
    public float msBetweenShots = 100; // �Ѿ� �߻� �ð� ����
    public float muzzleVelocity = 35; // �Ѿ� �߻� �ӵ�

    float nextShotTime; // ���� �Ѿ� �߻� �ð�

    public void Shoot() // �Ѿ� �߻� �޼ҵ�
    {
        if(Time.time > nextShotTime) // �Ѿ� �߻� �ð��� �������� Ȯ��
        {
            nextShotTime = Time.time + msBetweenShots / 1000; // ���� �Ѿ� �߻� ���� �ð� ����
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            //  �Ѿ� �������� ���� ������Ʈ�� �ν��Ͻ�ȭ �Ͽ� ����.
            newProjectile.SetSpeed(muzzleVelocity); // �Ѿ� �߻� �ӵ� ����
        }
    }
}
