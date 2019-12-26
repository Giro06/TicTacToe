using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public Sprite x;
    public Sprite o;
    public Sprite defaultSprite;
    int xIndex;
    int yIndex;
    Manager manager;
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<Manager>();
    }
    public void SetIndex(int x,int y)
    {
        xIndex = x;
        yIndex = y;
    }
    public void SetShape(int code)
    {
        if (code == 1)
        {
            GetComponent<SpriteRenderer>().sprite = x;
        }
        else if(code==2)
        {
            GetComponent<SpriteRenderer>().sprite = o;
        }
        else
        {

            GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }
        manager.SetBoardPiece(xIndex, yIndex, code);
       
     
    }
     private void OnMouseOver()
    {
         
        if (Input.GetMouseButtonDown(0))
        {
            if (GetComponent<SpriteRenderer>().sprite == defaultSprite)
            {
                SetShape(1);
                manager.ActivateAi();
            }
         
        }
    }
}
