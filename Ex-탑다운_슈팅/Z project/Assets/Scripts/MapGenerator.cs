using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps; // 맵 배열
    public int mapIndex; // 맵 배열 참조용 인덱스

    public Transform tilePrefab; // 인스턴스화 할 타일 프리팹
    public Transform obstaclePrefab; // 인스턴스화 할 장애물 프리팹
    public Transform navmeshFloor; // 내브메쉬를 위한 바닥 사이즈
    public Transform navemeshMaskPrefab; // 맵 바깥쪽 마스킹 프리팹
    public Vector2 maxMapSize; // 최대 맵 크기

    [Range(0, 1)] // 범위 지정
    public float outlinePercent; // 테두리 두께

    public float tileSize; // 타일 사이즈
    List<Coord> allTileCoords; // 모든 좌표값을 저장할 리스트 생성
    Queue<Coord> shuffledTileCoords; // 셔플된 좌표값을 저장할 큐 생성

    Map currentMap; // 현재 맵

    void Start() {
        GeneratorMap();
    }

    // ■ 맵 생성 메소드
    public void GeneratorMap() {
        currentMap = maps[mapIndex]; // 맵 설정
        System.Random prng = new System.Random(currentMap.seed); // 난수 생성
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);
        // 박스 콜라이더 맵 크기로 설정

        // ■ 좌표(Coord)들을 생성
        allTileCoords = new List<Coord>(); // 새 리스트 생성
        for (int x = 0; x < currentMap.mapSize.x; x++) {
            for (int y = 0; y < currentMap.mapSize.y; y++) { // 지정한 맵 크기만큼 루프
                allTileCoords.Add(new Coord(x, y)); // 리스트에 타일 좌표 추가
            }
        }

        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
        // 새 큐 생성, 셔플된 좌표값 배열을 저장함

        // ■ 맵 홀더(부모 오브젝트) 생성
        string holderName = "Generator Map"; // 타일 오브젝트를 가질 부모 오브젝트 이름
        if (transform.Find(holderName)) // 위 오브젝트에 자식 오브젝트가 있다면
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

        // ■ 타일들을 생성
        for (int x = 0; x < currentMap.mapSize.x; x++) {
            for (int y = 0; y < currentMap.mapSize.y; y++) { // 지정한 맵 크기만큼 루프
                Vector3 tilePosition = CoordToPosition(x, y);
                /* 타일이 생성될 위치 저장. 
                   -지정한 맵 가로 길이/2 를 설정하면 0에서 가로길이의 절반만큼 왼쪽으로 이동한 위치가 된다.
                   이를 활용하여 z 좌표에도 적용해 화면의 왼쪽 위 모서리부터 맵이 생성되도록 한다. */
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                /* 타일을 인스턴스화 하여 생성하고 위치, 각도를 설정
                   위치는 위에서 생성한 값을 전달하고 각도는 Quaternion(사원수) 메소드를 사용,
                   오일러 각을 기준으로 회전시킨다. */
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                /* localScale 은 오브젝트의 상대적 크기, Vector3.one 은 Vector3(1,1,1) 이다.
                   크기를 지정한 테두리 두께만큼 줄여서 지정한다.  */
                newTile.parent = mapHolder;
                // 타일의 부모 오브젝트 설정
            }
        }

        // ■ 장애물들을 생성
        bool[,] obstaclemap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y]; // 장애물 위치 확인 배열
        int currentObstacleCount = 0; // 현재 올바른 장애물 생성 개수

        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent); // 지정한 비율에 따른 장애물 개수
        for (int i = 0; i < obstacleCount; i++) { // 장애물 갯수만큼 루프
            Coord randomCoord = GetRandomCoord(); // 랜덤한 좌표를 받아옴
            obstaclemap[randomCoord.x, randomCoord.y] = true; // 해당 랜덤 위치 활성화
            currentObstacleCount++; // 장애물 개수 증가
            if (randomCoord != currentMap.mapCenter && MaplsFullyAccessible(obstaclemap, currentObstacleCount)) {
            // 랜덤 위치가 맵 중앙이 아니고 막힌 곳을 만들지 않을 때 아래 코드 실행

                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                // 장애물 높이 설정

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                // 좌표 변환
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity);
                // 장애물 인스턴스화 하여 생성
                newObstacle.parent = mapHolder;
                // 장애물의 부모 오브젝트 설정

                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);
                // 장애물 크기를 지정한 테두리 두께만큼 줄여서 타일 사이즈와 맞게 지정하고 높이를 지정한다. 

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                // 인스턴스화 하여 생성한 장애물의 렌더러 레퍼런스 생성
                Material obstacleMatetial = new Material(obstacleRenderer.sharedMaterial);
                // 장애물의 마테리얼 생성, 위 레퍼런스를 통해 장애물의 셰어드 마테리얼 저장
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                // 색상 비율 설정
                obstacleMatetial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroungColour, colourPercent);
                // 장애물 색상 설정
                obstacleRenderer.sharedMaterial = obstacleMatetial;
                // 장애물의 셰어드 마테리얼 설정
            }
            else { // 장애물 생성 조건이 맞지 않는 경우
                obstaclemap[randomCoord.x, randomCoord.y] = false; // 해당 랜덤 위치 비활성화
                currentObstacleCount--; // 장애물 개수 감소
            }
        }

        // ■ 내브메쉬 마스크 생성
        Transform maskLeft = Instantiate(navemeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        // 왼쪽 맵 바깥 마스킹 오브젝트 생성
        maskLeft.parent = mapHolder;
        // 부모 오브젝트 설정
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f * tileSize, 1, currentMap.mapSize.y * tileSize);
        // 왼쪽 맵 바깥 마스킹 오브젝트 크기 설정

        Transform maskRight = Instantiate(navemeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        // 오른쪽 맵 바깥 마스킹 오브젝트 생성
        maskRight.parent = mapHolder;
        // 부모 오브젝트 설정
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f * tileSize, 1, currentMap.mapSize.y * tileSize);
        // 오른쪽 맵 바깥 마스킹 오브젝트 크기 설정

        Transform maskTop = Instantiate(navemeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        // 위쪽 맵 바깥 마스킹 오브젝트 생성
        maskTop.parent = mapHolder;
        // 부모 오브젝트 설정
        maskTop.localScale = new Vector3(maxMapSize.x * tileSize, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f * tileSize);
        // 위쪽 맵 바깥 마스킹 오브젝트 크기 설정

        Transform maskBottom = Instantiate(navemeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        // 아래쪽 맵 바깥 마스킹 오브젝트 생성
        maskBottom.parent = mapHolder;
        // 부모 오브젝트 설정
        maskBottom.localScale = new Vector3(maxMapSize.x * tileSize, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f * tileSize);
        // 아래쪽 맵 바깥 마스킹 오브젝트 크기 설정


        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        // 지정한 최대 맵 사이즈에 맞게 내브메쉬 바닥 크기 조정
    }


    // ■ 맵 확인 메소드 (Flood-fill Algorithm)
    bool MaplsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
        bool[,] mapFlag = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        // 지나온 비어있는 타일을 체크할 배열을 생성
        Queue<Coord> queue = new Queue<Coord>(); // 큐 생성
        queue.Enqueue(currentMap.mapCenter); // 맵 중앙 위치를 큐에 넣음
        mapFlag[currentMap.mapCenter.x, currentMap.mapCenter.y] = true; // 맵 중앙을 비어있는 타일로 체크

        int accessibleTileCount = 1; // 접근 가능한 타일 개수(맵 중앙 포함이므로 기본 1)

        while (queue.Count > 0) { // 큐에 들어있는 값이 있는 경우
            Coord tile = queue.Dequeue(); // 큐에 저장된 맨 앞 타일 위치를 빼서 가져옴

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) { // 주변 타일 루프
                    int neighbourX = tile.x + x; // 주변 타일의 x 좌표
                    int neighbourY = tile.y + y; // 주변 타일의 y 좌표
                    if (x == 0 || y == 0) { // 주변 타일 중 대각선상에 위치하지 않은 경우
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
                            // 체크 중 맵 크기를 벗어나지 않는 경우
                            if (!mapFlag[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY]) {
                                // 체크된 타일이 아니고, 장애물이 아닌 경우

                                mapFlag[neighbourX, neighbourY] = true; // 타일 체크
                                queue.Enqueue(new Coord(neighbourX, neighbourY)); // 해당 타일 위치를 큐에 삽입
                                accessibleTileCount++; // 접근 가능한 타일 수 증가
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        // 현재 접근 가능해야 하는 타일 개수

        return targetAccessibleTileCount == accessibleTileCount;
        // 개수가 같다면(막힌 곳 없이 모든 타일에 접근 가능) true, 아니면 false 반환
    }

    // ■ 좌표 변환 메소드
    Vector3 CoordToPosition(int x, int y) {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
        // 입력받은 x, y 좌표로 Vector3 상의 타일 위치 설정
    }

    // ■ 큐에 저장된 좌표를 가져오는 메소드
    public Coord GetRandomCoord() {
        Coord randomCoord = shuffledTileCoords.Dequeue(); // 큐의 첫 번째 값을 가져온다.
        shuffledTileCoords.Enqueue(randomCoord); // 가져온 값을 큐의 맨 뒤로 넣는다.

        return randomCoord;
    }

    // ■ 타일 좌표 구조체
    [System.Serializable] // 인스펙터에서 보이도록 설정
    public struct Coord {
        public int x, y; // x, y 좌표값

        public Coord(int _x, int _y) { // 생성자로 좌표값 초기화
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2) { // 구조체 비교 연산자 정의
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2) { // 구조체 비교 연산자 정의
            return !(c1 == c2);
        }

        public override bool Equals(object obj) { // 비교 메소드 재정의
            return base.Equals(obj);
        }

        public override int GetHashCode() { // GetHashCode 메소드 재정의
            return base.GetHashCode();
        }
    }

    // ■ 맵 속성들을 저장 할 클래스
    [System.Serializable] // 인스펙터에서 보이도록 설정
    public class Map {
        public Coord mapSize; // 맵 크기
        [Range(0, 1)] // 장애물 비율 범위 설정
        public float obstaclePercent; // 맵의 장애물 비율
        public int seed; // 장애물 랜덤 생성 시드
        public float minObstacleHeight; // 장애물 최소 높이
        public float maxObstacleHeight; // 장애물 최대 높이
        public Color foregroundColour; // 장애물 전면부 색상
        public Color backgroungColour; // 장애물 후면부 색상

        public Coord mapCenter { // 맵 중앙 좌표
            get {
                return new Coord(mapSize.x / 2, mapSize.y / 2); // 중앙 좌표 지정 후 리턴
            }
        }
    }
}
