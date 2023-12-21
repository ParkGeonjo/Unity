using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable // �������̽� ���
{
    public float startingHealth; // ���� ü��
    public float health { get; protected set; } // ü��
    /* protected �� ���� ��� ���谡 ���� Ŭ�������� ����� �� ���� �Ѵ�.
       �ν����Ϳ����� ������ ����. */
    protected bool dead; // ĳ���Ͱ� �׾�����

    public event System.Action OnDeath;
    /* System.Action �� ��������Ʈ �޼ҵ��̴�.
       ���⼭ ��������Ʈ�� C++ �� �Լ� �����Ϳ� �����ϰ�
       �޼ҵ��� ��ġ�� ����Ű�� �ҷ��� �� �ִ� Ÿ���̴�.
       void �� �����ϰ� �Է��� ���� �ʴ´�. */

    protected virtual void Start()
    // ��� �޴� Ŭ�������� ���� �޼ҵ带 ������ �� �� �ֵ��� virtual �� ����.
    {
        health = startingHealth; // ���� ü�� ����
    }

    // �� Ÿ�� �޼ҵ�
    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection) {
        TakeDamage(damage); // ������ �޴� �޼ҵ� ȣ��
    }

    // �� ������ �޴� �޼ҵ�
    public virtual void TakeDamage(float damage) {
        health -= damage; // ü�¿��� ��������ŭ ����
        if (health <= 0 && !dead) // ü���� 0 ���ϰ� ���� ���
        {
            Die(); // ���� �޼ҵ� ȣ��
        }
    }

    // �ν����Ϳ��� ��ũ��Ʈ ������Ʈ ��Ŭ�� �� ��ü �ı� ��ư �߰�
    [ContextMenu("Self-Destruct")]
    public virtual void Die() // ���� �޼ҵ�
    {
        dead = true; // ���� �������� ����.
        if(OnDeath != null) // �̺�Ʈ�� �ִ� ���
        {
            OnDeath(); // �޼ҵ带 ȣ��
        }
        GameObject.Destroy(gameObject); // ���� ������Ʈ �ı�
    }
}
