using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTile : MonoBehaviour
{
    public AudioClip adbtclick;
    public AudioClip adtlmatch;
    public AudioSource audioSource;
    
    public GameObject Pobj;
    public Text Score;
    public Text EndScore;
    Sprite TestSprite;
    Toggle oobj;
    Toggle[,] obj = new Toggle[16, 11];
    int[,] tileValue = new int[16, 11];
    int onoffck = 0, Tmp = 0;
    static public int score = 0;
    bool bk;

    void CheckTile(Toggle[,] obj, int[,] tileValue)
    {
        int i, j, x1 = -1, x2 = -1, y1 = -1, y2 = -1, sum = 0, x11, x12, y11, y12;
        
        for (i = 0; i < 16; i++)
        {
            for (j = 0; j < 11; j++)
            {
                if (!obj[i, j].isOn)
                {
                    onoffck++;
                }
            }
        }

        if ((onoffck-Tmp) == 2)
        {
            bk = false;
            i = 0;

            // 첫번째, 두번째 타일 좌표 저장
            while (i < 16)
            {
                j = 0;
                while (j < 11)
                {
                    if (!obj[i, j].isOn && tileValue[i, j] != 0)
                    {
                        x1 = i;
                        y1 = j;
                        Debug.Log("=========");
                        Debug.Log("1번째 타일 x : " + x1);
                        Debug.Log("1번째 타일 y : " + y1);
                        bk = true;
                        break;
                    }
                    j++;
                }
                if (bk == true)
                    break;
                i++;
            }
            bk = false;
            i = 15;
            while (i >= 0)
            {
                j = 10;
                while(j >= 0)
                {
                    if (!obj[i, j].isOn && tileValue[i, j] != 0)
                    {
                        x2 = i;
                        y2 = j;
                        Debug.Log("2번째 타일 x : " + x2);
                        Debug.Log("2번째 타일 y : " + y2);
                        bk = true;
                        break;
                    }
                    j--;
                }
                if (bk == true)
                    break;
                i--;
            }

            // 좌표 크기 비교
            if (x1 >= x2)
            {
                x11 = x2;
                x12 = x1;
            }
            else
            {
                x11 = x1;
                x12 = x2;
            }
            if (y1 >= y2)
            {
                y11 = y2;
                y12 = y1;
            }
            else
            {
                y11 = y1;
                y12 = y2;
            }

            //
            for (i = x11; i <= x12; i++)
            {
                for (j = y11; j <= y12; j++)
                {
                    Debug.Log("타일 i : " + i);
                    Debug.Log("타일 j : " + j);
                    Debug.Log("타일값 : "+tileValue[i, j]);
                    sum += tileValue[i, j];
                }
            }

            Debug.Log(sum);
            Debug.Log("=========");
            if (sum == 10)
            {
                Debug.Log("10 완성");
                for (i = x11; i <= x12; i++)
                {
                    for (j = y11; j <= y12; j++)
                    {
                        tileValue[i, j] = 0;
                        obj[i, j].isOn = false;
                        obj[i, j].enabled = false;
                        Tmp = 0;
                        for (int k = 0; k < 16; k++)
                        {
                            for (int g = 0; g < 11; g++)
                            {
                                if (!obj[k, g].isOn)
                                {
                                    Tmp++;
                                }
                            }
                        }
                        audioSource.clip = adtlmatch;
                        audioSource.Play();
                        score = (int)(Tmp * 1.24f);
                        Debug.Log(Tmp);
                    }
                }
            }
        }
        onoffck = 0;
    }

    void Start()
    {
        int x = Screen.width / 19;
        int y = Screen.height - Screen.height / 7;
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                tileValue[i, j] = Random.Range(1, 10);
                switch (tileValue[i, j])
                {
                    case 1:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT1"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                    case 2:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT2"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                    case 3:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT3"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                    case 4:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT4"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                    case 5:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT5"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                    case 6:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT6"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                    case 7:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT7"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                    case 8:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT8"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                    case 9:
                        obj[i, j] = Instantiate(Resources.Load<Toggle>("BT9"), new Vector3(x, y, 0), Quaternion.identity, Pobj.transform);
                        obj[i, j].transform.localScale = new Vector3(4.5f, 4.5f, 1);
                        break;
                }
                x += Screen.width / 11;
            }
            x = Screen.width / 19;
            y -= (Screen.width / 11);
        }
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                obj[i, j].onValueChanged.AddListener((value) => { MyListener(value); });//Do this in Start() for example
            }
        }
    }

    public void MyListener(bool value)
    {
        if (value)
        {
            CheckTile(obj, tileValue); audioSource.clip = adbtclick;
            audioSource.Play();
        }
        else
        {
            CheckTile(obj, tileValue); audioSource.clip = adbtclick;
            audioSource.Play();
        }
    }

    public void BTsound()
    {
        audioSource.clip = adbtclick;
        audioSource.Play();
    }

    void Update()
    {
        Score.text = score.ToString();
        EndScore.text = score.ToString();
        int onoffck3 = 0;
        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                if (!obj[i, j].isOn && obj[i, j].enabled != false)
                {
                    onoffck3++;
                }
            }
        }

        if (onoffck3 == 2)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (obj[i, j].isOn)
                        obj[i, j].enabled = false;
                }
            }
        }
        else
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (obj[i, j].isOn)
                        obj[i, j].enabled = true;
                }
            }
        }
    }
}
