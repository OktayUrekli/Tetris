using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    [SerializeField] Transform tilePrefab;

    public int height=22;
    public int width=10;

    Transform[,] grid;

    int completedRowCount;

    ScoreManager scoreManager;

    [SerializeField] ParticleManager[] particleManager=new ParticleManager[4];
    // int[] copmlatedRowsPositions=new int[4];


    private void Awake()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        grid = new Transform[width,height];
    }

    private void Start()
    {
        CreateEmptySquares();

    }


    // Grid kareleri olu�turuluyor
    void CreateEmptySquares()
    {
        if (tilePrefab != null)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Transform tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity,this.transform);
                    tile.name = "x" + x.ToString() + "," + "y" + y.ToString();
                }
            }
        }
        else
        {
            Debug.Log("Tile prefab eklenmedi");
        }
    }

    // yerle�en �eklin her par�as�n� gridde denk geldi�i kareye e�itleyerek o kareleri m�sait durumdan ��kar�yoruz
    public void TakeTheActiveShapeToGrid(ShapeManager shape)
    {   
        if (shape==null)
        {
            return;
        }

        foreach(Transform child in shape.transform)
        {
            Vector2 pos=MakePositionRound(child.position);
            grid[(int)pos.x, (int)pos.y] = child;  // kareyi �eklin bir bar�as� ile dolduruyoruz
        }
    }

    // �eklin board i�inde uygun konumda olup olmad���n� kontrol eder
    public bool IsShapeInValidPos(ShapeManager shape) 
    {
        foreach(Transform childTile in shape.transform) // �eklin her par�as�n�n board �zerindeki uygunluk durumunu kontrol ediyor
        {
            Vector2 pos =MakePositionRound(childTile.position); // t�m par�alar�n pozisyonlar� int de�er olacak �ekilde yuvarlan�yor

            if (!IsTilesOfShapesInBoard((int)pos.x,(int)pos.y)) 
            {
                return false; // e�er �eklin herhangi bir par�as� board d���na ��kt�ysa false d�ner
            }

            if (pos.y<height)
            {
                if (IsSquareEmpty((int)pos.x, (int)pos.y, shape))
                {
                    return false; // e�er kare doluysa �ekil bu konuma yerle�emez
                }
            }

        }
        return true;
    }

    #region Sat�rlar�n Doluluk durumunu ve G�ncellenmesini Kontrol Eden Fonksiyonlar

    // g�nderilen sat�r�n t�m karelerinin doluluk durumunu kontrol eder
    bool IsRowFull(int row)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x,row]==null)
            {
                return false; // e�er ilgili sat�rda herhangi bir kare bo� ise sat�r tamamlanmam��t�r. false d�ner 
            }
        }
        return true; // t�m sat�r par�alarla dolmu� ise t�m par�alar silinebilir ve true d�ner
    }

    // dolmu� sat�rlar� siler ve silinen par�alar�n karelerini null yapar
    void CleanTheFulledRow(int row)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, row] != null)
            {
                Destroy(grid[x, row].gameObject); // par�alar teker teker siliniyor.
            }
            grid[x, row] = null; // kareler null yap�l�yor
        }
    }


    // sat�r silme sonras� bir sat�r a��a�� kayd�r�l�r. 
    void MoveDownOneRow(int row) // g�nderilen row de�eri silinen sat�r�n 1 �st�ndeki sat�r de�eridir
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, row] != null)
            {
                grid[x,row-1]=grid[x,row];
                grid[x, row] = null;
                grid[x, row-1].position +=Vector3.down;
            }
        }
    }

    // silme sonras� silinen sat�r�n �st�ndeki t�m sat�rlar� 1 sat�r a��a�� kayd�r�r
    void MoveDownAllRows(int startingRow)
    {
        for (int i = startingRow; i < height; ++i)
        {
            MoveDownOneRow(i);
        }
    }


    // �ekiller yerle�tikten sonra t�m grid doluluk durumu kontrol ediliyor
    public IEnumerator CleanAllRows()
    {
        completedRowCount = 0;

        for (int y = 0; y < height; ++y)
        {
            if (IsRowFull(y)) // e�er sat�r dolu ise 
            {

                PlayComplatedRowsVfxs(completedRowCount, y); // tamamlanan sat�r�n efekti oynat�l�r
                completedRowCount++; // �al��cak efekt indexi olarak kullan�laca�� i�in efekt �al��t�ktan sonra y�kseltiliyor 
                yield return new WaitForSeconds(0.15f); // vfx ler s�rayla oynamas� i�in

            }
        }

        for (int y = 0; y < height; y++)
        {
            if (IsRowFull(y)) // e�er sat�r dolu ise 
            {              
                CleanTheFulledRow(y); // dolu sat�r temizlenir
                MoveDownAllRows(y + 1); // t�m sat�rlar bir sat�r a��a�� kayd�r�l�r
                y--; // sat�r g�ncellemesinden dolay� tekrar ayn� sat�r kontrol edilmeli yoksa 1 sat�r atlan�l�r 
                
            }
        }

        if (completedRowCount > 1)
        {
            SoundManager.instance.PlayVocalSound();
            scoreManager.ScoreCounter(completedRowCount);
        }
        else if (completedRowCount == 1)
        {
            SoundManager.instance.PlaySfx(4);
            scoreManager.ScoreCounter(completedRowCount);
        }

        yield return new WaitForSeconds(0.1f );

    }

    #endregion

    // �ekil yerle�tikten sonra gridden ta�ma olup olmad���n� kontrol eder varsa true d�ner ve oyun biter
    public bool DidShapeOverflowTheGrid(ShapeManager shape)
    {
        foreach (Transform child in shape.transform)
        {
            if (child.position.y>=height-1)
            {
                return true;
            }
        }
        return false;
    }

    // g�nderilen grid konumundaki doluluk durumunu kontrol eder 
    bool IsSquareEmpty(int x, int y, ShapeManager shape) 
    {
        // gridde o konum doluysa ve dolu olan konum ba�ka �eklin bir par�as� ile doluysa true d�ner
        return (grid[x, y] != null && grid[x, y].parent != shape.transform); 
    }

    // �eklin g�nderilen par�as�n�n pozisyonunun grid s�n�rlar� i�inde olup olmad���n� kontrol eder 
    bool IsTilesOfShapesInBoard(int x, int y) 
    {
        return (x >= 0 && x < width && y >= 0);  // x grid s�n�rlar� 0-9 aras� y s�n�rlar� 0-21 aras�
    }

    Vector2 MakePositionRound(Vector2 shapePos) // de�erleri int de�erlere yuvarlar
    {
        return new Vector2(Mathf.Round(shapePos.x), Mathf.Round(shapePos.y));
    }

    void PlayComplatedRowsVfxs(int compRowCount,int y)
    {
        //if (particleManager)
        //{
        //    particleManager.transform.position = new Vector3(0, y, 0); // vfx leri tamamlanan sat�ra getiriyor
        //    particleManager.PlayEffects();
        //}

        particleManager[completedRowCount].transform.position = new Vector3(0, y, 0); // vfx leri tamamlanan sat�ra getiriyor
        particleManager[completedRowCount].PlayEffects();
    }
}
