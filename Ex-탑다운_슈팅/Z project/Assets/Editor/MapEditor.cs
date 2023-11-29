using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // ����Ƽ ������ ���

[CustomEditor(typeof(MapGenerator))]
// �����Ͱ� ���(����)�� Ŭ���� ���
public class MapEditor : Editor { // ������ ���
    public override void OnInspectorGUI() // �ν����� �޼ҵ�
    {
        base.OnInspectorGUI(); // �츮�� ����Ƽ���� ����ϴ� �⺻ �ν������� ���������� ���Խ�Ų��.

        MapGenerator map = target as MapGenerator;
        /* ����Ƽ���� ���� CuntomEditor �� ����ص� Ŭ������ target ���� �������ش�.
           �̸� ����ȯ�Ͽ� MapGenerator map ���� ��������. */

        map.GeneratorMap();
        /* GUI �� �׷����� �� ������, �� GUI ���� �츮�� ���� �����ϸ� �ٲܶ�����
           �޼ҵ带 ȣ���Ͽ� Ÿ���� ������Ѵ�. */
    }
}
