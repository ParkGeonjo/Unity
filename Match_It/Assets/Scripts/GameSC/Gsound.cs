using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gsound : MonoBehaviour
{
    public AudioClip btcli;
    AudioSource auds;

    void Awake()
    {
        auds = GetComponent<AudioSource>();
    }

    public void btclic()
    {
        auds.clip = btcli;
        auds.Play();
    }
}
