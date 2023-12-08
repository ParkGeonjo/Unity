using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRigidbody; // ������ٵ� ���۷���
    public float forceMin; // �������� �� �ּҰ�
    public float forceMax; // �������� �� �ִ밪

    float lifeTime = 1; // ź�� ���� �ð�
    float fadeTime = 1; // ź�� ������� ��� �ð�

    void Start() {
        float force = Random.Range(forceMin, forceMax); // �������� �� ���������� �������� ����
        myRigidbody.AddForce(transform.right * force); // ������ٵ� ���� ���������� �� ����
        myRigidbody.AddTorque(Random.insideUnitSphere * force); // ���� ��ü ���ο� ���� ���� ���� ���� ȸ���� �ش�.

        StartCoroutine(Fade()); // ���̵� �ƿ� ȿ�� �ڷ�ƾ ����
    }

    // �� ź�� ���̵� �ƿ� �ڷ�ƾ
    IEnumerator Fade() {
        yield return new WaitForSeconds(lifeTime); // lifeTime �Ŀ� ����.

        float percent = 0; // ���̵� �ƿ� ȿ�� �ۼ�Ʈ
        float fadeSpeed = 1 / fadeTime; // ���̵� �ƿ� ȿ�� �ð�
        Material mat = GetComponent<Renderer>().material; // ���͸��� ���۷���
        Color initialColour = mat.color; // ���� ���� ����

        while (percent < 1) { // ���̵� �ƿ� ȿ���� ������ �ʾҴٸ�
            percent += Time.deltaTime * fadeSpeed; // ���̵� �ƿ� ȿ�� �ۼ�Ʈ ���

            mat.color = Color.Lerp(initialColour, Color.clear, percent); // ź�� ���� ����
            yield return null; // ���� ������ ���� �ݺ�
        }

        Destroy(gameObject); // �� ������Ʈ �ı�
    }
}
