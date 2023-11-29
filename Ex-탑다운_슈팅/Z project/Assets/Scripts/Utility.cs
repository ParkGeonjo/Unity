using System.Collections;
using System.Collections.Generic;

public static class Utility {
    public static T[] ShuffleArray<T>(T[] array, int seed) {
        /* ���� �޼ҵ� ����, �迭�� �õ带 ���޹޾� ���� ��Ģ��� ������ ����
           ���� ���� �迭�� ��Ȯ�� ���¸� �𸣱� ������ ���ʸ�Ÿ�� T �� ����Ѵ�. */
        System.Random prng = new System.Random(seed);
        // ���� ������, �־��� �õ带 ���� ���� ��Ģ���� ������ �����Ѵ�.

        for(int i = 0; i < array.Length - 1; i++) { // �迭 ���̸�ŭ �ݺ�
            int randIndex = prng.Next(i, array.Length); // ���� �ε��� ��ȣ ����

            // ��ġ ����
            T tempItem = array[randIndex];
            array[randIndex] = array[i];
            array[i] = tempItem;
        }

        return array; // ���õ� �迭 ��ȯ
    }
}
