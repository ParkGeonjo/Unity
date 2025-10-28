using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        /* 셔플 메소드 생성, 배열과 시드를 전달받아 일정 규칙대로 섞도록 설정
           전달 받을 배열의 정확한 형태를 모르기 때문에 제너릭타입 T 를 사용한다. */
        System.Random prng = new System.Random(seed);
        // 난수 생성기, 주어진 시드를 통해 일정 규칙으로 난수를 생성한다.

        for (int i = 0; i < array.Length - 1; i++)
        { // 배열 길이만큼 반복
            int randIndex = prng.Next(i, array.Length); // 랜덤 인덱스 번호 생성

            // 위치 변경
            T tempItem = array[randIndex];
            array[randIndex] = array[i];
            array[i] = tempItem;
        }

        return array; // 셔플된 배열 반환
    }

    // StreamingAssets에서 파일 읽고 Dictionary로 변환
    public static Dictionary<string, string> ReadObjectDataFromFile(string fileName)
    {
        Dictionary<string, string> objectData = new Dictionary<string, string>();

        // StreamingAssets 경로 설정
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string fileContent = string.Empty;

        // 경로 확인 로그 추가
        Debug.Log($"Attempting to load file from path: {filePath}");

        if (Application.platform == RuntimePlatform.Android)
        {
            // Android에서는 UnityWebRequest 사용
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            var operation = request.SendWebRequest();

            while (!operation.isDone) { } // 비동기 요청 대기

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load file from StreamingAssets on Android: {request.error}");
                return objectData;
            }

            fileContent = request.downloadHandler.text;
        }
        else
        {
            // 에디터 및 Windows/Mac/Linux에서는 File.ReadAllText 사용
            if (File.Exists(filePath))
            {
                fileContent = File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError($"File not found in StreamingAssets: {filePath}");
                return objectData;
            }
        }

        // 파일 내용을 Dictionary로 변환
        return ParseTextToDictionary(fileContent);
    }

    // 텍스트 내용을 Dictionary로 변환
    private static Dictionary<string, string> ParseTextToDictionary(string text)
    {
        Dictionary<string, string> objectData = new Dictionary<string, string>();

        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            // 콤마(,)로 구분하여 Key와 Value로 저장
            string[] parts = line.Split(',');
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                objectData[key] = value;
            }
        }

        return objectData;
    }
}
