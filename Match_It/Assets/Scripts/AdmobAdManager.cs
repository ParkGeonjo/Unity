using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdmobAdManager : MonoBehaviour
{
    private BannerView bannerView;
    private AdRequest request;

    public void Start()
    {
        #if UNITY_ANDROID
            string appId = "ca-app-pub-8225164502788348~4601733954";
        #elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
        #else
            string appId = "unexpected_platform";
        #endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((initStatue) => {
            RequestBanner();
        });

        bannerView.OnAdLoaded += BannerView_OnAdLoaded;

    }

    private void RequestBanner()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-8225164502788348/3576426898";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
            string adUnitId = "unexpected_platform";
        #endif

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        request = new AdRequest.Builder()
        .AddTestDevice("56B6317C622B075D8556AF7F76E82B32")
        .Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    private void BannerView_OnAdLoaded(object sender, EventArgs e)
    {
        print("OnAdLoaded");
        foreach (var item in request.TestDevices)
        {
            Debug.Log("TestDevices List" + item);
        }

    }
}
