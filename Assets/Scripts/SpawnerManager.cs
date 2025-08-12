using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerManager : MonoBehaviour
{
    [SerializeField] ShapeManager[] shapes;

    [SerializeField] Sprite[] shapeImages;
    [SerializeField] Image[] imagesOfNextShapes;

    ShapeManager[] nextTwoShapes=new ShapeManager[2];

    public ShapeManager CreateRandomShape()
    {   
        int shapeIndex=Random.Range(0, shapes.Length);
        ShapeManager shape = Instantiate(shapes[shapeIndex],transform.position,Quaternion.identity) as ShapeManager;

        MakeQuene(0);
        MakeQuene(1);
        //UpdateImagesOfNextShapes();

        if (shape != null)
        {
            return shape;
        }
        else { return null; }
    }

    void MakeQuene(int index)
    {
        int shapeIndex = Random.Range(0, shapes.Length);
        //ShapeManager shape = Instantiate(shapes[shapeIndex], transform.position, Quaternion.identity) as ShapeManager;
        nextTwoShapes[index] = shapes[shapeIndex];
        imagesOfNextShapes[index].sprite= shapes[shapeIndex].shapeSprite;
    }

    public ShapeManager CreateNextShape()
    {
        ShapeManager shape = Instantiate(nextTwoShapes[0], transform.position, Quaternion.identity) as ShapeManager;
        UpdateShapeQuene();

        if (shape != null)
        {
            return shape;
        }
        else { return null; }
    }

    void UpdateShapeQuene()
    {
        nextTwoShapes[0] = nextTwoShapes[1];
        imagesOfNextShapes[0].sprite = nextTwoShapes[0].shapeSprite;
        MakeQuene(1);

        //UpdateImagesOfNextShapes();
    }

    //void UpdateImagesOfNextShapes()
    //{
    //    for (int i = 0; i < nextTwoShapes.Length; i++)
    //    {
    //        string shapeName = nextTwoShapes[i].name;
    //        switch (shapeName)
    //        {
    //            case "I":
    //                imagesOfNextShapes[i].sprite = shapeImages[0];
    //                break;
    //            case "J":
    //                imagesOfNextShapes[i].sprite = shapeImages[1];
    //                break;
    //            case "L":
    //                imagesOfNextShapes[i].sprite = shapeImages[2];
    //                break;
    //            case "O":
    //                imagesOfNextShapes[i].sprite = shapeImages[3];
    //                break;
    //            case "S":
    //                imagesOfNextShapes[i].sprite = shapeImages[4];
    //                break;
    //            case "T":
    //                imagesOfNextShapes[i].sprite = shapeImages[5];
    //                break;
    //            case "Z":
    //                imagesOfNextShapes[i].sprite = shapeImages[6];
    //                break;
    //        }
    //    }
    //}
}
