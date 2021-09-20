using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiecesInMenu : MonoBehaviour
{
    private string imageName;

    Texture2D thisTexture;
    RawImage rawImage;

    private void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();

        imageName = "Image" + gameObject.name + ".png";

        BetterStreamingAssets.Initialize();

        thisTexture = new Texture2D(100, 100);

        byte[] bytes = BetterStreamingAssets.ReadAllBytes(imageName);

        thisTexture.LoadImage(bytes);
        thisTexture.name = imageName;
        rawImage.texture = thisTexture;
    }

    
}
