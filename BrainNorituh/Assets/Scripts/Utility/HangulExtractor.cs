using System;
using System.Text;

public class HangulExtractor
{
    // 한글 초성 배열
    private static readonly char[] initial = new char[] { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };

    public static string ExtractInitials(string input)
    {
        StringBuilder result = new StringBuilder();

        foreach (char c in input)
        {
            // 한글 여부 확인
            if (c >= 0xAC00 && c <= 0xD7A3) // 유니코드 한글 범위
            {
                int unicodeIndex = c - 0xAC00;
                int initialIndex = unicodeIndex / 588; // 초성 인덱스 계산
                result.Append(initial[initialIndex]);
            }
            else
            {
                // 한글이 아닌 문자는 그대로 추가
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
