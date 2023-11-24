using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Msound : MonoBehaviour
{
    public AudioClip btcl;
    AudioSource ads;

    void Awake()
    {
        ads = GetComponent<AudioSource>();
    }

    public void btc()
    {
        ads.clip = btcl;
        ads.Play();
    }
}

