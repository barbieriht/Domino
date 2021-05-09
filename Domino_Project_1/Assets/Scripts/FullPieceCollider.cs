using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullPieceCollider : MonoBehaviour
{
    public Collider2D PieceCollider;
    public bool collidingWithOther = false;

    private void Start()
    {
        PieceCollider = this.GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other)
        {
            default:
                collidingWithOther = true;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        switch (other)
        {
            default:
                collidingWithOther = false;
                break;
        }
            
    }

}
