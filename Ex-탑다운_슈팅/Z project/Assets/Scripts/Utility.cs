using System.Collections;
using System.Collections.Generic;

public static class Utility {
    public static T[] ShuffleArray<T>(T[] array, int seed) {
        /* 셔플 메소드 생성, 배열과 시드를 전달받아 일정 규칙대로 섞도록 설정
           전달 받을 배열의 정확한 형태를 모르기 때문에 제너릭타입 T 를 사용한다. */
        System.Random prng = new System.Random(seed);
        // 난수 생성기, 주어진 시드를 통해 일정 규칙으로 난수를 생성한다.

        for(int i = 0; i < array.Length - 1; i++) { // 배열 길이만큼 반복
            int randIndex = prng.Next(i, array.Length); // 랜덤 인덱스 번호 생성

            // 위치 변경
            T tempItem = array[randIndex];
            array[randIndex] = array[i];
            array[i] = tempItem;
        }

        return array; // 셔플된 배열 반환
    }
}
