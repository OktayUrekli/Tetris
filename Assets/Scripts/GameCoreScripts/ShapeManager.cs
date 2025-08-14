using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
    [SerializeField] bool canRotate;

    public Sprite shapeSprite;

    GameObject[] settleEffects;
    
    private void Start()
    {
        settleEffects = GameObject.FindGameObjectsWithTag("settleEffect")  ;
    }


    public void PlaySettleEffects()
    {
        int counter = 0;

        foreach (Transform childPos in gameObject.transform)
        {
            if (settleEffects[counter])
            {
                settleEffects[counter].transform.position = new Vector3(childPos.position.x,childPos.position.y+1,0); // +1 nedenini çözemedim
                ParticleManager pm=settleEffects[counter].GetComponent<ParticleManager>();

                if (pm)
                {
                    pm.PlayEffects();
                    
                }   
            }
            counter++;
        }
    }

    public void MoveRight()
    {
        transform.Translate(Vector3.right,Space.World);
    }
    public void MoveLeft()
    {
        transform.Translate(Vector3.left, Space.World);
    }
    public void MoveUp()
    {
        transform.Translate(Vector3.up, Space.World);
    }
    public void MoveDown()
    {
        transform.Translate(Vector3.down, Space.World);
    }

    public void RotateLeft()
    {
        if (canRotate)
        {
            transform.Rotate(0, 0, 90);
        }
    }

    public void RotateRight()
    {
        if (canRotate) 
        { 
            transform.Rotate(0, 0, -90); 
        }
    }

    public void RotateClockwise(bool clockwise)
    {
        if (clockwise)
        {
            RotateRight();
        }
        else
        {
            RotateLeft();
        }
    }
}
