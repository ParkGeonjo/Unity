using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode {Auto, Burst, Single}; // �� �ݹ� ��� (�ڵ�, ����, �ܹ�)
    public FireMode fireMode; // �ݹ� ��� ����

    public Transform[] projectileSpawn; // �Ѿ� �߻� ��ġ �迭
    public Projectile projectile; // �Ѿ�
    public float msBetweenShots = 100; // �Ѿ� �߻� �ð� ����
    public float muzzleVelocity = 35; // �Ѿ� �߻� �ӵ�

    public int burstCount; // ���� ��� �Ѿ� �߻� ����
    public int shotsRemainingInBurst; // ���� ��� ���� �Ѿ�

    public Vector2 kickMinMax; // �ݵ� �ּ�, �ִ밪

    public Transform shell; // ź�� ���۷���
    public Transform shellEjection; // ź�� ���� ��ġ ���۷���

    MuzzleFlash muzzleFlash; // �ѱ� ȭ�� ���۷���

    float nextShotTime; // ���� �Ѿ� �߻� �ð�

    bool triggerReleasedSinceLastShoot; // ���� �߻� �� ��Ƽ�(���콺 ��Ŭ��)�� ���Ҵ���

    Vector3 recoilSmoothDampVelocity; // �ݵ� ȸ�� �ӵ�
    float recoilRotSmoothDampVelocity; // �ݵ� ȸ�� �ӵ�
    float recoilAngle; // �ݵ� ����
    

    void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>(); // �ѱ� ȭ�� ���۷��� �Ҵ�
        shotsRemainingInBurst = burstCount; // ���� ��� ���� �Ѿ� ���� ����
    }

    void LateUpdate() {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, .1f);
        // SmoothDamp �� ���� �ε巯�� ���������� ���� ��ġ���� ��ǥ��ġ�� zero ���� ������ �ӵ��� .1f �� �ð����� �����δ�.
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, .1f);
        // ���� ���� ���� ���� �ε巴�� ���������� �����ϵ��� ��
        transform.localEulerAngles = transform.localEulerAngles + Vector3.right * recoilAngle;
    }

    void Shoot() // �Ѿ� �߻� �޼ҵ�
    {
        if(Time.time > nextShotTime) // �Ѿ� �߻� �ð��� �������� Ȯ��
        {
            // �� ���� ��� ������ ���� �߻�
            if (fireMode == FireMode.Burst) { // ���� �߻� ��尡 ������ ���
                if(shotsRemainingInBurst == 0) { // �߻� �� ���� �Ѿ��� ���� ���
                    return; // �Ʒ� �ڵ�(�Ѿ� �߻�) ���� �ǳʶ�
                }
                shotsRemainingInBurst--; // ���� �Ѿ� ���� ����
            }
            else if (fireMode == FireMode.Single) { // ���� �߻� ��尡 �ܹ��� ���
                if (!triggerReleasedSinceLastShoot) { // �̹� �Ѿ��� �߻� �ϰ� ��Ƽ踦 ����(���콺 ��Ŭ�� ��) �ִ� ���
                    return; // �Ʒ� �ڵ�(�Ѿ� �߻�) ���� �ǳʶ� 
                }
            }

            // �� �Ѿ� ���� �� �߻�
            for (int i = 0; i < projectileSpawn.Length; i++) { // �Ѿ� �߻� ������ŭ �ݺ�
                nextShotTime = Time.time + msBetweenShots / 1000; // ���� �Ѿ� �߻� ���� �ð� ����
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                //  �Ѿ� �������� ���� ������Ʈ�� �ν��Ͻ�ȭ �Ͽ� ����.
                newProjectile.SetSpeed(muzzleVelocity); // �Ѿ� �߻� �ӵ� ����
            }

            // �� ���� ����Ʈ(ź��, �ѱ� ȭ��)
            Instantiate(shell, shellEjection.position, shellEjection.rotation); // ź�� �ν��Ͻ�ȭ �Ͽ� ����
            muzzleFlash.Activate(); // �ѱ� ȭ�� ����

            // �� ���� �ݵ�
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y); // ���� ���ʹ������� �̵�
            recoilAngle += 5; // ���� �������� ȸ��
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30); // ���� �ݵ� ���� ���� ����
        }
    }


    // �� ���� ���� �޼ҵ�
    public void Aim(Vector3 aimPoint) {
        transform.LookAt(aimPoint); // ���޹��� ��ġ�� ������ ���� ����
    }

    // �� ��Ƽ�(���콺 ��Ŭ��)�� ������ ���� ��
    public void OnTriggerHold() {
        Shoot(); // �Ѿ� �߻�
        triggerReleasedSinceLastShoot = false; // ��Ƽ� ���� ���·�
    }

    // �� ��Ƽ�(���콺 ��Ŭ��)�� ������ ��
    public void OnTriggerRelease() {
        triggerReleasedSinceLastShoot = true; // ��Ƽ� ���� ���·�

        shotsRemainingInBurst = burstCount; // ���� ��� ���� �Ѿ� ���� ����
    }
}
