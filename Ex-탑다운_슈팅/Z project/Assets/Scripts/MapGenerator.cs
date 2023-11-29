using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab; // 인스턴스화 할 타일 프리팹
    public Transform obstaclePrefab; // 인스턴스화 할 장애물 프리팹

    public Vector2 mapSize; // 맵 크기

    [Range(0, 1)] // 범위 지정
    public float outlinePercent; // 테두리 두께

    List<Coord> allTileCoords; // 모든 좌표값을 저장할 리스트 생성
    Queue<Coord> shuffledTileCoords; // 셔플된 좌표값을 저장할 큐 생성

    public int seed = 10; // 좌표값 랜덤 설정 시드

    void Start() {
        GeneratorMap();
    }

    public void GeneratorMap() {
        allTileCoords = new List<Coord>(); // 새 리스트 생성
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) { // 지정한 맵 크기만큼 루프
                allTileCoords.Add(new Coord(x, y)); // 리스트에 타일 좌표 추가
            }
        }

        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        // 새 큐 생성, 셔플된 좌표값 배열을 저장함

        string holderName = "Generator Map"; // 타일 오브젝트를 가질 부모 오브젝트 이름
        if(transform.Find(holderName)) // 위 오브젝트에 자식 오브젝트가 있다면
        // * 강의에서는 FindChild 를 사용하는데, 버전이 바뀌면서 사용하지 않고 Find 로 변경되었다.
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
            /* 오브젝트의 자식 게임 오브젝트를 제거한다.
               에디터에서 호출 할 것이기 때문에 DestroyImmediate 를 사용한다. */
        }

        Transform mapHolder = new GameObject(holderName).transform;
        // 타일을 자식 오브젝트로 가질 새 오브젝트를 생성
        mapHolder.parent = transform;
        // 현재 오브젝트를 mapHolder 오브젝트의 부모 오브젝트로 설정.


        for(int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++) // 지정한 맵 크기만큼 루프
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                /* 타일이 생성될 위치 저장. 
                   -지정한 맵 가로 길이/2 를 설정하면 0에서 가로길이의 절반만큼 왼쪽으로 이동한 위치가 된다.
                   이를 활용하여 z 좌표에도 적용해 화면의 왼쪽 위 모서리부터 맵이 생성되도록 한다. */
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                /* 타일을 인스턴스화 하여 생성하고 위치, 각도를 설정
                   위치는 위에서 생성한 값을 전달하고 각도는 Quaternion(사원수) 메소드를 사용,
                   오일러 각을 기준으로 회전시킨다. */
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                /* localScale 은 오브젝트의 상대적 크기, Vector3.one 은 Vector3(1,1,1) 이다.
                   크기를 지정한 테두리 두께만큼 줄여서 지정한다.  */
                newTile.parent = mapHolder;
                // 타일의 부모 오브젝트 설정
            }
        }

        int obstacleCount = 10; // 장애물 갯수
        for(int i = 0; i < obstacleCount; i++) { // 장애물 갯수만큼 루프
            Coord randomCoord = GetRandomCoord(); // 랜덤한 좌표를 받아옴
            Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y); // 좌표 변환
            Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
            // 장애물 인스턴스화 하여 생성
            newObstacle.parent = mapHolder;
            // 장애물의 부모 오브젝트 설정
        }

    }

    Vector3 CoordToPosition(int x, int y) { // 좌표 변환 메소드
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
        // 입력받은 x, y 좌표로 Vector3 상의 타일 위치 설정
    }

    public Coord GetRandomCoord() { // 큐에 저장된 좌표를 가져오는 메소드
        Coord randomCoord = shuffledTileCoords.Dequeue(); // 큐의 첫 번째 값을 가져온다.
        shuffledTileCoords.Enqueue(randomCoord); // 가져온 값을 큐의 맨 뒤로 넣는다.

        return randomCoord;
    }

    public struct Coord { // 타일 좌표 구조체
        public int x, y; // x, y 좌표값

        public Coord(int _x, int _y) { // 생성자로 좌표값 초기화
            x = _x;
            y = _y;
        }
    }
}
