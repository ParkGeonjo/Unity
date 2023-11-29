using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamaneable // �������̽� ���
{
    public float startingHealth; // ���� ü��
    protected float health; // ü��
    /* protected �� ���� ��� ���谡 ���� Ŭ�������� ����� �� ���� �Ѵ�.
       �ν����Ϳ����� ������ ����. */
    protected bool dead; // ĳ���Ͱ� �׾�����

    public event System.Action OnDeath;
    /* System.Action �� ��������Ʈ �޼ҵ��̴�.
       ���⼭ ��������Ʈ�� C++ �� �Լ� �����Ϳ� �����ϰ�
       �޼ҵ��� ��ġ�� ����Ű�� �ҷ��� �� �ִ� Ÿ���̴�.
       void �� �����ϰ� �Է��� ���� �ʴ´�. */

    protected virtual void Start()
    // ��� �޴� Ŭ������ ���� �޼ҵ带 ������ �� �� �ֵ��� virtual �� ����.
    {
        health = startingHealth; // ���� ü�� ����
    }

    public void TakeHit(float damage, RaycastHit hit) // ������ �޴� �޼ҵ� ����
    {
        TakeDamage(damage); // ������ �޴� �޼ҵ� ȣ��
    }

    public void TakeDamage(float damage) // ������ �޴� �޼ҵ� ����
    {
        health -= damage; // ü�¿��� ��������ŭ ����
        if (health <= 0 && !dead) // ü���� 0 ���ϰ� ���� ���
        {
            Die(); // ���� �޼ҵ� ȣ��
        }
    }

    protected void Die() // ���� �޼ҵ�
    {
        dead = true; // ���� �������� ����.
        if(OnDeath != null) // �̺�Ʈ�� �ִ� ���
        {
            OnDeath(); // �޼ҵ带 ȣ��
        }
        GameObject.Destroy(gameObject); // ���� ������Ʈ �ı�
    }
}
