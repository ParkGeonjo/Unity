using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder; // ȭ�� ȿ�� ������Ʈ ���۷���
    public Sprite[] flashSprites; // ȭ�� ȿ�� ��������Ʈ �迭
    public SpriteRenderer[] spriteRenderer; // ȭ�� ȿ�� ��������Ʈ ������ �迭

    public float flashTime; // ȭ�� ȿ�� ���� �ð�

    // �� ȭ�� ȿ�� Ȱ��ȭ �޼ҵ�
    public void Activate() {
        flashHolder.SetActive(true); // ȭ�� ȿ�� ������Ʈ Ȱ��ȭ

        int flashSpriteIndex = Random.Range(0, flashSprites.Length); // ��������Ʈ �ε��� ���� ����
        for(int i = 0; i < spriteRenderer.Length; i++) {
            spriteRenderer[i].sprite = flashSprites[flashSpriteIndex]; // ���� ��������Ʈ ����
        }

        Invoke("Deactivate", flashTime); // ������ �ð� �� Deactivate �޼ҵ� ȣ��
    }

    // �� ȭ�� ȿ�� ��Ȱ��ȭ �޼ҵ� 
    void Deactivate() {
        flashHolder.SetActive(false); // ȭ�� ȿ�� ������Ʈ ��Ȱ��ȭ
    }


}
