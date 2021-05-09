using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class PiecesInMenu : MonoBehaviour
{
    [SerializeField]
    private int value;
    [SerializeField]
    private int index;

    private SpriteRenderer spriteRenderer;

    private string imageName;

    private void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        
    }

    private void OnEnable()
    {
        index = this.transform.GetSiblingIndex();
        value = this.transform.parent.transform.GetSiblingIndex() - 1;

        imageName = "Image" + value.ToString() + index.ToString() + ".png";

        //Debug.Log(imageName);

        SelectImage();
    }

    public void SelectImage()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);

        foreach (FileInfo file in directoryInfo.GetFiles("*.*"))
        {
            if (file.Name.Contains(imageName))
                StartCoroutine("LoadPieceUI", file);
        }
    }

    IEnumerator LoadPieceUI(FileInfo pieceFile)
    {
        string wwwPieceFilePath = "file://" + pieceFile.FullName.ToString();
        WWW www = new WWW(wwwPieceFilePath);
        yield return www;
        spriteRenderer.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
    }
}
