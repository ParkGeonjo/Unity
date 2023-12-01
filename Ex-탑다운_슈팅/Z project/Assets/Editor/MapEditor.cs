using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // ����Ƽ ������ ���

[CustomEditor(typeof(MapGenerator))]
// �����Ͱ� ���(����)�� Ŭ���� ���
public class MapEditor : Editor
{ // ������ ���
    public override void OnInspectorGUI() // �ν����� �޼ҵ�
    {
        MapGenerator map = target as MapGenerator;
        /* ����Ƽ���� ���� CuntomEditor �� ����ص� Ŭ������ target ���� �������ش�.
           �̸� ����ȯ�Ͽ� MapGenerator map ���� ��������. */

        if (DrawDefaultInspector())
        { // bool ���� ��ȯ, �ν����Ϳ��� ���� ������ ��쿡�� true �� ��ȯ�Ѵ�.
            map.GeneratorMap();
            /* GUI �� �׷����� �� ������, �� GUI ���� �츮�� ���� �����ϸ� �ٲܶ�����
               �޼ҵ带 ȣ���Ͽ� Ÿ���� ������Ѵ�. */
        }

        if (GUILayout.Button("Generate Map")) { // ��ư�� ������ �ڵ� ���� 
            map.GeneratorMap();
        }
    }
}
