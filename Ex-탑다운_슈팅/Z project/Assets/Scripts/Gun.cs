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
    public int projectilesPerMag; // źâ �Ѿ� ��
    public float reloadTime = .3f; // ������ �ӵ�

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .5f); // ���� �ݵ� �ּ�, �ִ밪
    public Vector2 recoilAngleMinMax = new Vector2(3, 5); // ���� �ݵ� �ּ�, �ִ밪
    public float recoilMoveSettleTime = .1f; // ���� �ݵ� ȸ�� �ð�
    public float recoilRotationSettleTime = .1f; // ���� �ݵ� ȸ�� �ð�

    [Header("Effect")]
    public Transform shell; // ź�� ���۷���
    public Transform shellEjection; // ź�� ���� ��ġ ���۷���
    MuzzleFlash muzzleFlash; // �ѱ� ȭ�� ���۷���
    float nextShotTime; // ���� �Ѿ� �߻� �ð�

    bool triggerReleasedSinceLastShoot; // ���� �߻� �� ��Ƽ�(���콺 ��Ŭ��)�� ���Ҵ���
    int shotsRemainingInBurst; // ���� ��� ���� �Ѿ�
    int projectilesRemainingInMag; // źâ�� �����ִ� �Ѿ�
    bool isReloading; // ���������� ����

    Vector3 recoilSmoothDampVelocity; // �ݵ� ȸ�� �ӵ�
    float recoilRotSmoothDampVelocity; // �ݵ� ȸ�� �ӵ�
    float recoilAngle; // ���� �ݵ� ����
    

    void Start() {
        muzzleFlash = GetComponent<MuzzleFlash>(); // �ѱ� ȭ�� ���۷��� �Ҵ�
        shotsRemainingInBurst = burstCount; // ���� ��� ���� �Ѿ� ���� ����
        projectilesRemainingInMag = projectilesPerMag; // �⺻ źâ �Ѿ� ���� ����
    }

    void LateUpdate() {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        // SmoothDamp �� ���� �ε巯�� ���������� ���� ��ġ���� �������� zero ���� ������ �ӵ��� ������ �ð����� �����δ�.
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettleTime);
        // ���� ���� ���� ���� �ε巴�� ���������� �����ϵ��� ��
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
        // ���� �ݵ� ���� ����

        if(!isReloading && projectilesRemainingInMag == 0) { // ���������� �ƴϰ� źâ�� �Ѿ��� ���� ���
            Reload(); // ������ �޼ҵ� ȣ��
        }
    }

    void Shoot() // �Ѿ� �߻� �޼ҵ�
    {
        if(!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0) {
        // ���������� �ʰ�, �Ѿ� �߻� �ð��� ������, źâ�� �Ѿ��� �ִ��� Ȯ��
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
                if(projectilesRemainingInMag == 0) { // źâ�� �Ѿ��� ���� ���
                    break; // ���� Ż��(�Ѿ� ���� ���)
                }
                projectilesRemainingInMag--; // źâ���� �Ѿ� ����
                nextShotTime = Time.time + msBetweenShots / 1000; // ���� �Ѿ� �߻� ���� �ð� ����
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                //  �Ѿ� �������� ���� ������Ʈ�� �ν��Ͻ�ȭ �Ͽ� ����.
                newProjectile.SetSpeed(muzzleVelocity); // �Ѿ� �߻� �ӵ� ����
            }

            // �� ���� ����Ʈ(ź��, �ѱ� ȭ��)
            Instantiate(shell, shellEjection.position, shellEjection.rotation); // ź�� �ν��Ͻ�ȭ �Ͽ� ����
            muzzleFlash.Activate(); // �ѱ� ȭ�� ����

            // �� ���� �ݵ�
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y); // ���� �ݵ��� ����
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y); // ���� �ݵ��� ����
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30); // ���� �ݵ� ���� �ּ�, �ִ� ���� ����
        }
    }

    // �� ������ �޼ҵ�
    public void Reload() {
        if(!isReloading && projectilesRemainingInMag != projectilesPerMag) { // ���������� �ƴϰ� źâ�� �Ѿ��� �� ������ ���� ���
            StartCoroutine(AnimateReload()); // ������ �ִϸ��̼� �ڷ�ƾ ����
        }
    }

    // �� ������ �ִϸ��̼� �ڷ�ƾ
    IEnumerator AnimateReload() {
        isReloading = true; // ������������ ����
        yield return new WaitForSeconds(.2f); // ���� �ڵ� ������� 0.2 �� ���

        float reloadSpeed = 1 / reloadTime; // ������ �ӵ� ����
        float percent = 0; // �ִϸ��̼� ���� ����
        Vector3 initialRot = transform.localEulerAngles; // ���� ����
        float maxReloadAngle = 30; // �ִ� ���� ���� ����

        while (percent < 1) {
            percent += Time.deltaTime * reloadSpeed; // �ִϸ��̼� ���� ���� ����
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4; // ������ ������ ���� ������ ����
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation); // ������ ���� ����
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle; // ���� ����

            yield return null; // ���� ������ ��ŵ
        }

        isReloading = false; // ���������� �ƴ����� ����
        projectilesRemainingInMag = projectilesPerMag; // źâ �Ѿ� �� ����
    }

    // �� ���� ���� �޼ҵ�
    public void Aim(Vector3 aimPoint) {
        if(!isReloading) { // ���������� �ƴ� ��
            transform.LookAt(aimPoint); // ���޹��� ��ġ�� ������ ���� ����
        }
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
