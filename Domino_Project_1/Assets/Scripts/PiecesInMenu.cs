using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiecesInMenu : MonoBehaviour
{
    /*
     * [SerializeField]
    private int value;
    [SerializeField]
    private int index;
    */

    private string imageName;

    Texture2D thisTexture;
    RawImage rawImage;

    private void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();

        /*
         * index = this.transform.GetSiblingIndex();
        value = this.transform.parent.transform.GetSiblingIndex() - 1;
        
        imageName = "Image" + value.ToString() + index.ToString() + ".png";
        */
        imageName = "Image" + gameObject.name + ".png";

        BetterStreamingAssets.Initialize();

        thisTexture = new Texture2D(100, 100);

        byte[] bytes = BetterStreamingAssets.ReadAllBytes(imageName);

        thisTexture.LoadImage(bytes);
        thisTexture.name = imageName;
        rawImage.texture = thisTexture;
    }

    
}
