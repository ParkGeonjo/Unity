using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOnOff : MonoBehaviour
{
    public GameObject Canv;

    public void explinoff()
    {
        if (Canv.activeSelf)
            Canv.SetActive(false);
        else
            Canv.SetActive(true);
    }
}
