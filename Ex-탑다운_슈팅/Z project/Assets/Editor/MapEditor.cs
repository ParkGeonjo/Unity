using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // 유니티 에디터 사용

[CustomEditor(typeof(MapGenerator))]
// 에디터가 사용(설정)할 클래스 명시
public class MapEditor : Editor
{ // 에디터 상속
    public override void OnInspectorGUI() // 인스펙터 메소드
    {
        MapGenerator map = target as MapGenerator;
        /* 유니티에서 위에 CuntomEditor 로 명시해둔 클래스를 target 으로 지정해준다.
           이를 형변환하여 MapGenerator map 으로 가져오자. */

        if (DrawDefaultInspector())
        { // bool 값을 반환, 인스펙터에서 값을 변경한 경우에만 true 를 반환한다.
            map.GeneratorMap();
            /* GUI 가 그려지는 매 프레임, 즉 GUI 에서 우리가 값을 설정하며 바꿀때마다
               메소드를 호출하여 타일을 재생성한다. */
        }

        if (GUILayout.Button("Generate Map")) { // 버튼을 누르면 코드 실행 
            map.GeneratorMap();
        }
    }
}
