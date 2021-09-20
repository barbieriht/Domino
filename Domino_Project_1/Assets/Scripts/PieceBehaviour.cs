using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.Networking;

public class PieceBehaviour : MonoBehaviour
{
    public int value = 1;
    private int index;
    private int cardNumber;
    public bool upSide;

    private SpriteRenderer spriteRenderer;

    private string pieceName;
    private string imageName;
    // public bool isBorder = true;

    private void Start()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        pieceName = this.transform.parent.name;

        if (value != (int)char.GetNumericValue(pieceName[6]))
            index = (int)char.GetNumericValue(pieceName[6]);
        else
            index = (int)char.GetNumericValue(pieceName[10]);

        imageName = "Image" + this.value.ToString() + index.ToString() + ".png";

        SelectImage();

        cardNumber = this.GetComponentInParent<DraggablePiece>().cardNumber;

        //sprite = Resources.Load<Sprite>(this.value.ToString() + "-" + index.ToString());
        //spriteRenderer.sprite = sprite;
    }

    public void SelectImage()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath);


        foreach (FileInfo file in directoryInfo.GetFiles("*.*"))
        {
            if(file.Name.Contains(imageName))
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

    public int GetValue()
    {
        return this.value;
    }

   /* public bool GetIsBorder()
    {
        return isBorder;
    }*/

    public void SetValue(int x)
    {
        value = x;
    }

    [PunRPC]
    public void TurnActive(bool x)
    {
        this.gameObject.SetActive(x);
    }

    /*public void SetBool(bool b)
    {
        isBorder = b;
    }*/
}
