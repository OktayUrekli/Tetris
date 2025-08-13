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
        if (!shadowShape) // e�er g�lge yoksa
        {
            shadowShape = Instantiate(activeShape, activeShape.transform.position, activeShape.transform.rotation);

            shadowShape.name = "Shadow of " + activeShape.name;

            SpriteRenderer[] srOfTiles = shadowShape.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in srOfTiles)
            {
                sr.color = shadowColor;
            }
        }
        else // g�lge varsa aktif �eklin pozisyon ve rotasyon bilgilerine g�re g�ncellenir. // late update ile s�rekli g�ncellenir
        {
            shadowShape.transform.position=activeShape.transform.position;
            shadowShape.transform.rotation=activeShape.transform.rotation;
        }

        isShapeSettled = false;

        while (!isShapeSettled) // gridin sonuna de�medi�i s�rece
        {
            shadowShape.MoveDown(); // CreateShadowShape fonksiyonu late update te �al��t��� i�in normal shapeten �nce a��a��ya var�yor.
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
