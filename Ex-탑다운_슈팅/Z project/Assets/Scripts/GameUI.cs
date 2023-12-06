using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    public Image fadePlane; // ���̵� �̹���(UI ���) ������Ʈ ���۷���
    public GameObject gameOverUI; // ���� ���� �ؽ�Ʈ, ��ư ������Ʈ ���۷���

    void Start() {
        FindObjectOfType<Player>().OnDeath += OnGameOver; // �÷��̾� ��� �̺�Ʈ ����
    }

    // �� �÷��̾� ���(���� ����) �� UI ó�� �޼ҵ�
    void OnGameOver() {
        StartCoroutine(Fade(Color.clear, Color.black, 1)); // ��� �̹��� ���̵� �� ȿ�� �ڷ�ƾ ����
        gameOverUI.SetActive(true); // ���� ���� �ؽ�Ʈ, ��ư ������Ʈ Ȱ��ȭ
    }

    // �� �̹��� ���̵� ȿ�� �ڷ�ƾ
    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time; // ���̵� ȿ�� �ӵ�
        float percent = 0; // �̹��� ����(����) ��ȭ �ۼ�Ʈ

        while(percent < 1) { // ����(����) ��ȭ�� 1(100%)�̸��� ��
            percent += Time.deltaTime * speed; // ����(����) ��ȭ�� ���
            fadePlane.color = Color.Lerp(from, to, percent); // ����(����) ����
            yield return null; // ���� ���������� �ǳʶ�
        }
    }

    // UI Input �κ�

    // �� �� ���� ���� �޼ҵ�
    public void startNewGame() {
        Application.LoadLevel("Game"); // ���� ��, ������ �ٽ� �ε�
    }
}
