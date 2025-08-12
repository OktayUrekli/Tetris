using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowShape : MonoBehaviour
{
    ShapeManager shadowShape=null;

    bool isShapeSettled=false;

    public Color shadowColor=new Color(1f,1f,1f,.2f);

    public void CreateShadowShape(ShapeManager activeShape,BoardManager board)
    {
        if (!shadowShape) // eðer gölge yoksa
        {
            shadowShape = Instantiate(activeShape, activeShape.transform.position, activeShape.transform.rotation);

            shadowShape.name = "Shadow of " + activeShape.name;

            SpriteRenderer[] srOfTiles = shadowShape.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in srOfTiles)
            {
                sr.color = shadowColor;
            }
        }
        else // gölge varsa aktif þeklin pozisyon ve rotasyon bilgilerine göre güncellenir. // late update ile sürekli güncellenir
        {
            shadowShape.transform.position=activeShape.transform.position;
            shadowShape.transform.rotation=activeShape.transform.rotation;
        }

        isShapeSettled = false;

        while (!isShapeSettled) // gridin sonuna deðmediði sürece
        {
            shadowShape.MoveDown(); // CreateShadowShape fonksiyonu late update te çalýþtýðý için normal shapeten önce aþþaðýya varýyor.
            if (!board.IsShapeInValidPos(shadowShape))
            {
                shadowShape.MoveUp();
                isShapeSettled=true;    
            }
        }
    }

    public void ResetShadow()
    {
        Destroy(shadowShape.gameObject);
    }
}
