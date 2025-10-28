using System;
using System.Collections.Generic;
using UnityEngine;

public static class PronunciationSimilarity
{
    // 두 문자열 간 발음 유사도를 계산하여 반환하는 메서드
    public static double CalculatePronunciationSimilarity(string text, string ans)
    {
        // 자모 단위로 분해
        var jamos1 = DecomposeToJamo(text);
        var jamos2 = DecomposeToJamo(ans);

        // 레벤슈타인 거리 계산 (가중치 적용)
        double distance = WeightedLevenshteinDistance(jamos1, jamos2);
        double maxLength = Math.Max(jamos1.Count, jamos2.Count);

        // 유사도를 계산하여 반환
        return maxLength == 0 ? 1.0 : 1.0 - distance / maxLength;
    }

    // 한글 문자열을 자모(초성, 중성, 종성) 단위로 분해하는 메서드
    private static List<(char, CharType)> DecomposeToJamo(string text)
    {
        var jamoList = new List<(char, CharType)>();

        foreach (char c in text)
        {
            if (c >= 0xAC00 && c <= 0xD7A3) // 한글 음절인지 확인
            {
                int unicode = c - 0xAC00;
                int initial = unicode / (21 * 28); // 초성
                int medial = (unicode % (21 * 28)) / 28; // 중성
                int final = unicode % 28; // 종성

                // 초성, 중성, 종성 추가 (각각의 타입과 함께)
                jamoList.Add(((char)(0x1100 + initial), CharType.Initial)); // 초성
                jamoList.Add(((char)(0x1161 + medial), CharType.Medial));  // 중성

                // 종성이 존재할 경우 추가
                if (final != 0)
                {
                    jamoList.Add(((char)(0x11A7 + final), CharType.Final)); // 종성
                }
            }
            else
            {
                // 한글 음절이 아닌 경우 그대로 추가, 타입을 None으로
                jamoList.Add((c, CharType.None));
            }
        }

        return jamoList;
    }

    // 가중치를 고려한 레벤슈타인 거리 계산 메서드
    private static double WeightedLevenshteinDistance(List<(char, CharType)> jamos1, List<(char, CharType)> jamos2)
    {
        double[,] matrix = new double[jamos1.Count + 1, jamos2.Count + 1];

        // 초기화: 첫 번째 행과 열 설정
        for (int i = 0; i <= jamos1.Count; i++)
            matrix[i, 0] = i;
        for (int j = 0; j <= jamos2.Count; j++)
            matrix[0, j] = j;

        // 가중치 설정
        double initialWeight = 2.0;  // 초성 가중치 (높음)
        double medialWeight = 1.5;   // 중성 가중치 (중간)
        double finalWeight = 1.0;    // 종성 가중치 (낮음)

        // 레벤슈타인 거리 계산 (가중치 적용)
        for (int i = 1; i <= jamos1.Count; i++)
        {
            for (int j = 1; j <= jamos2.Count; j++)
            {
                double cost = GetWeight(jamos1[i - 1], jamos2[j - 1], initialWeight, medialWeight, finalWeight);
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost
                );
            }
        }

        return matrix[jamos1.Count, jamos2.Count];
    }

    // 자모 간의 가중치를 적용해 편집 비용을 계산하는 메서드
    private static double GetWeight((char, CharType) jamo1, (char, CharType) jamo2, double initialWeight, double medialWeight, double finalWeight)
    {
        // 두 자모의 타입이 다르면 기본 비용 1 적용
        if (jamo1.Item2 != jamo2.Item2) return 1.0;

        // 동일한 타입에 따라 가중치를 부여하여 비교
        switch (jamo1.Item2)
        {
            case CharType.Initial:
                return jamo1.Item1 == jamo2.Item1 ? 0.0 : initialWeight;
            case CharType.Medial:
                return jamo1.Item1 == jamo2.Item1 ? 0.0 : medialWeight;
            case CharType.Final:
                return jamo1.Item1 == jamo2.Item1 ? 0.0 : finalWeight;
            default:
                return jamo1.Item1 == jamo2.Item1 ? 0.0 : 1.0;
        }
    }

    // 자모의 종류를 나타내는 Enum
    private enum CharType { Initial, Medial, Final, None }
}
