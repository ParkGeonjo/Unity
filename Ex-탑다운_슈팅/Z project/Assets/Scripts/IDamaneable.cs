using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamaneable // �������̽� ����.
{
    void TakeHit(float damage, RaycastHit hit); // �������� �޴� �޼ҵ�
    void TakeDamage(float damage); // �������� �޴� �޼ҵ�(����ȭ)
}
