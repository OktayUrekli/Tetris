using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    SpawnerManager spawnerManager;
    BoardManager boardManager;

    ShapeManager activeShape;

    [Header("�ekil a�a�� inme h�z�")]
    [Range(0.025f, 1f)]
    [SerializeField] float moveDownDuration;
    float moveDownCounter=0; // ba�lang�� de�eri 0 olmal�

    [Header("�ekil sa�/sol kayd�rma h�z�")]
    [Range(0.025f, 1f)]
    [SerializeField] float rightLeftKeyDuration; // tu�a bas�l� tutma durumunda hareketi yava�latmak i�in cool down s�resi
    float rightLeftKeyCounter=0;

    [Header("�ekil d�nd�rme h�z�")]
    [Range(0.025f, 1f)]
    [SerializeField] float rotateKeyDuration; 
    float rotateKeyCounter=0;

    [Header("�ekil a�a�� indirme h�z�")]
    [Range(0.025f, 1f)]
    [SerializeField] float downKeyDuration; 
    float downKeyCounter = 0;

    public bool isGameOver=false;


    bool clockwise=true;
    [SerializeField]  IconController rotateIcon;

    ShadowShape shadowShape;

    private void Awake()
    {
        spawnerManager = FindObjectOfType<SpawnerManager>();
        boardManager = FindObjectOfType<BoardManager>();
        shadowShape = FindObjectOfType<ShadowShape>();
    }


    private void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        if (spawnerManager)
        {
            if (activeShape==null)
            {
                activeShape=spawnerManager.CreateRandomShape();
                activeShape.transform.position=MakePositionRound(activeShape.transform.position);
            }
        } 
    }

    private void Update()
    {
        if (!spawnerManager || !boardManager || !activeShape || isGameOver ) { return; } // objelerden birisi bo� ise i�lem ger�ekle�meyecek

        DetectingInputs();

    }

    private void LateUpdate()
    {
        if (!spawnerManager || !boardManager || !activeShape || isGameOver) { return; }

        if (shadowShape)
        {
            shadowShape.CreateShadowShape(activeShape, boardManager);
        }
    }

    private void DetectingInputs()
    {
        if ((Input.GetKey("right") && Time.time > rightLeftKeyCounter) || Input.GetKeyDown("right")) // �ekli sa�a kayd�rma
        {
            activeShape.MoveRight();
            rightLeftKeyCounter = Time.time + rightLeftKeyDuration; // sonraki hareket zaman� ayarlan�yor

            if (!boardManager.IsShapeInValidPos(activeShape)) // yeni konumun uygunlu�u kontrol ediliyor. 
            {
                PlaySfx(1); // eror sfx index =1
                activeShape.MoveLeft(); // yeni konum uygun de�ilse eski konumuna geri al�n�yor.
            }
            else { PlaySfx(3); } // move sfx indexi = 3
        }

        else if ((Input.GetKey("left") && Time.time > rightLeftKeyCounter) || Input.GetKeyDown("left")) // �ekli sola kayd�rma
        {
            activeShape.MoveLeft();
            rightLeftKeyCounter = Time.time + rightLeftKeyDuration;

            if (!boardManager.IsShapeInValidPos(activeShape))
            {
                PlaySfx(1);
                activeShape.MoveRight();
            }
            else { PlaySfx(3); }
        }

        else if ((Input.GetKey("up") && Time.time > rotateKeyCounter) || Input.GetKeyDown("up")) // �ekil rotasyonu
        {
            activeShape.RotateLeft();
            rotateKeyCounter = Time.time + rotateKeyDuration;

            if (!boardManager.IsShapeInValidPos(activeShape))
            {
                
                PlaySfx(1);
                activeShape.RotateRight();
            }
            else {
                PlaySfx(3);
                clockwise = !clockwise;
                if (rotateIcon)
                {
                    rotateIcon.UpdateIconState(clockwise);
                }
            }
        }

        else if (Input.GetKey("down") && Time.time> downKeyCounter || Time.time > moveDownCounter)
        { //          h�zl�ca a�a��ya indirme                    ve ya   kendi kendine s�rekli a�a�� inme

            activeShape.MoveDown();
            moveDownCounter = Time.time+ moveDownDuration;
            downKeyCounter = Time.time + downKeyDuration;

            if (!boardManager.IsShapeInValidPos(activeShape)) 
            {
                if (boardManager.DidShapeOverflowTheGrid(activeShape)) // e�er �ekil gridden ta�t� ise oyun biter
                {
                    activeShape.MoveUp();
                    isGameOver = true;
                    FindAnyObjectByType<UiManager>().GameOverPanelOnOff(isGameOver);
                    PlaySfx(6); // game over sfx index=6
                }
                else
                {
                    ShapeSettleDown(); 
                }
            }
        }
    }

    // �ekil gidebilece�i son noktaya geldikten sonra �al��acak fonksiyon
    private void ShapeSettleDown()
    {
        PlaySfx(5); // yerle�me sfx index=5

        activeShape.PlaySettleEffects(); // yerle�me efektleri oynat�l�yor

        // de�erler yeni �ekili y�nlendirmek i�in bir nevi s�f�rlan�yor.
        rightLeftKeyCounter=Time.time;
        downKeyCounter=Time.time;
        rotateKeyCounter=Time.time;

        activeShape.MoveUp(); // bu sat�r ile �ekil hatal� pozisyondan ��kacak
        boardManager.TakeTheActiveShapeToGrid(activeShape); // �eklin par�alar�n�n bulundu�u grid kareleri me�gul duruma �ekiliyor. bir nevi kaydediliyor

        if (shadowShape)
            shadowShape.ResetShadow();

        StartCoroutine(boardManager.CleanAllRows()); // �ekil yerle�tikten sonra grid karelerinin doluluk durumuna g�re silme i�lemi kntrol� yap�lacak

        // di�er �ekilin gidecek bir yeri kalmad��� i�in yeni bir �ekil olu�turuluyor.
        activeShape = spawnerManager.CreateNextShape();
    }


    // olu�turulan random �ekilin pozisyon de�erlerini tam say� yapar. B�ylelikle gridlere tam oturur
    Vector2 MakePositionRound(Vector2 shapePos) 
    {
        return new Vector2(Mathf.Round(shapePos.x),Mathf.Round( shapePos.y));
    }

    public void RotateDirectionButton()
    {
        clockwise = !clockwise;
        activeShape.RotateClockwise(clockwise);
        if (!boardManager.IsShapeInValidPos(activeShape))
        {
            activeShape.RotateClockwise(!clockwise);
            PlaySfx(1);
        }
        else
        {
            if (rotateIcon)
            {
                rotateIcon.UpdateIconState(clockwise);
            }
            PlaySfx(3);
        }
    }

    void PlaySfx(int sfxIndex)
    {
        SoundManager.instance.PlaySfx(sfxIndex);
    }

    
    
}
