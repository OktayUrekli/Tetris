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


    // Grid kareleri oluþturuluyor
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

    // yerleþen þeklin her parçasýný gridde denk geldiði kareye eþitleyerek o kareleri müsait durumdan çýkarýyoruz
    public void TakeTheActiveShapeToGrid(ShapeManager shape)
    {   
        if (shape==null)
        {
            return;
        }

        foreach(Transform child in shape.transform)
        {
            Vector2 pos=MakePositionRound(child.position);
            grid[(int)pos.x, (int)pos.y] = child;  // kareyi þeklin bir barçasý ile dolduruyoruz
        }
    }

    // þeklin board içinde uygun konumda olup olmadýðýný kontrol eder
    public bool IsShapeInValidPos(ShapeManager shape) 
    {
        foreach(Transform childTile in shape.transform) // þeklin her parçasýnýn board üzerindeki uygunluk durumunu kontrol ediyor
        {
            Vector2 pos =MakePositionRound(childTile.position); // tüm parçalarýn pozisyonlarý int deðer olacak þekilde yuvarlanýyor

            if (!IsTilesOfShapesInBoard((int)pos.x,(int)pos.y)) 
            {
                return false; // eðer þeklin herhangi bir parçasý board dýþýna çýktýysa false döner
            }

            if (pos.y<height)
            {
                if (IsSquareEmpty((int)pos.x, (int)pos.y, shape))
                {
                    return false; // eðer kare doluysa þekil bu konuma yerleþemez
                }
            }

        }
        return true;
    }

    #region Satýrlarýn Doluluk durumunu ve Güncellenmesini Kontrol Eden Fonksiyonlar

    // gönderilen satýrýn tüm karelerinin doluluk durumunu kontrol eder
    bool IsRowFull(int row)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x,row]==null)
            {
                return false; // eðer ilgili satýrda herhangi bir kare boþ ise satýr tamamlanmamýþtýr. false döner 
            }
        }
        return true; // tüm satýr parçalarla dolmuþ ise tüm parçalar silinebilir ve true döner
    }

    // dolmuþ satýrlarý siler ve silinen parçalarýn karelerini null yapar
    void CleanTheFulledRow(int row)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, row] != null)
            {
                Destroy(grid[x, row].gameObject); // parçalar teker teker siliniyor.
            }
            grid[x, row] = null; // kareler null yapýlýyor
        }
    }


    // satýr silme sonrasý bir satýr aþþaðý kaydýrýlýr. 
    void MoveDownOneRow(int row) // gönderilen row deðeri silinen satýrýn 1 üstündeki satýr deðeridir
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

    // silme sonrasý silinen satýrýn üstündeki tüm satýrlarý 1 satýr aþþaðý kaydýrýr
    void MoveDownAllRows(int startingRow)
    {
        for (int i = startingRow; i < height; ++i)
        {
            MoveDownOneRow(i);
        }
    }


    // þekiller yerleþtikten sonra tüm grid doluluk durumu kontrol ediliyor
    public IEnumerator CleanAllRows()
    {
        completedRowCount = 0;

        for (int y = 0; y < height; ++y)
        {
            if (IsRowFull(y)) // eðer satýr dolu ise 
            {

                PlayComplatedRowsVfxs(completedRowCount, y); // tamamlanan satýrýn efekti oynatýlýr
                completedRowCount++; // çalýþcak efekt indexi olarak kullanýlacaðý için efekt çalýþtýktan sonra yükseltiliyor 
                yield return new WaitForSeconds(0.15f); // vfx ler sýrayla oynamasý için

            }
        }

        for (int y = 0; y < height; y++)
        {
            if (IsRowFull(y)) // eðer satýr dolu ise 
            {              
                CleanTheFulledRow(y); // dolu satýr temizlenir
                MoveDownAllRows(y + 1); // tüm satýrlar bir satýr aþþaðý kaydýrýlýr
                y--; // satýr güncellemesinden dolayý tekrar ayný satýr kontrol edilmeli yoksa 1 satýr atlanýlýr 
                
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

    // þekil yerleþtikten sonra gridden taþma olup olmadýðýný kontrol eder varsa true döner ve oyun biter
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

    // gönderilen grid konumundaki doluluk durumunu kontrol eder 
    bool IsSquareEmpty(int x, int y, ShapeManager shape) 
    {
        // gridde o konum doluysa ve dolu olan konum baþka þeklin bir parçasý ile doluysa true döner
        return (grid[x, y] != null && grid[x, y].parent != shape.transform); 
    }

    // þeklin gönderilen parçasýnýn pozisyonunun grid sýnýrlarý içinde olup olmadýðýný kontrol eder 
    bool IsTilesOfShapesInBoard(int x, int y) 
    {
        return (x >= 0 && x < width && y >= 0);  // x grid sýnýrlarý 0-9 arasý y sýnýrlarý 0-21 arasý
    }

    Vector2 MakePositionRound(Vector2 shapePos) // deðerleri int deðerlere yuvarlar
    {
        return new Vector2(Mathf.Round(shapePos.x), Mathf.Round(shapePos.y));
    }

    void PlayComplatedRowsVfxs(int compRowCount,int y)
    {
        //if (particleManager)
        //{
        //    particleManager.transform.position = new Vector3(0, y, 0); // vfx leri tamamlanan satýra getiriyor
        //    particleManager.PlayEffects();
        //}

        particleManager[completedRowCount].transform.position = new Vector3(0, y, 0); // vfx leri tamamlanan satýra getiriyor
        particleManager[completedRowCount].PlayEffects();
    }
}
