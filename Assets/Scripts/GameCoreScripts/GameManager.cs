using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    SpawnerManager spawnerManager;
    BoardManager boardManager;

    ShapeManager activeShape;

    [Header("Þekil aþaðý inme hýzý")]
    [Range(0.025f, 1f)]
    [SerializeField] float moveDownDuration;
    float moveDownCounter=0; // baþlangýç deðeri 0 olmalý

    [Header("Þekil sað/sol kaydýrma hýzý")]
    [Range(0.025f, 1f)]
    [SerializeField] float rightLeftKeyDuration; // tuþa basýlý tutma durumunda hareketi yavaþlatmak için cool down süresi
    float rightLeftKeyCounter=0;

    [Header("Þekil döndürme hýzý")]
    [Range(0.025f, 1f)]
    [SerializeField] float rotateKeyDuration; 
    float rotateKeyCounter=0;

    [Header("Þekil aþaðý indirme hýzý")]
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
        if (!spawnerManager || !boardManager || !activeShape || isGameOver ) { return; } // objelerden birisi boþ ise iþlem gerçekleþmeyecek

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
        if ((Input.GetKey("right") && Time.time > rightLeftKeyCounter) || Input.GetKeyDown("right")) // þekli saða kaydýrma
        {
            activeShape.MoveRight();
            rightLeftKeyCounter = Time.time + rightLeftKeyDuration; // sonraki hareket zamaný ayarlanýyor

            if (!boardManager.IsShapeInValidPos(activeShape)) // yeni konumun uygunluðu kontrol ediliyor. 
            {
                PlaySfx(1); // eror sfx index =1
                activeShape.MoveLeft(); // yeni konum uygun deðilse eski konumuna geri alýnýyor.
            }
            else { PlaySfx(3); } // move sfx indexi = 3
        }

        else if ((Input.GetKey("left") && Time.time > rightLeftKeyCounter) || Input.GetKeyDown("left")) // þekli sola kaydýrma
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

        else if ((Input.GetKey("up") && Time.time > rotateKeyCounter) || Input.GetKeyDown("up")) // þekil rotasyonu
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
        { //          hýzlýca aþaðýya indirme                    ve ya   kendi kendine sürekli aþaðý inme

            activeShape.MoveDown();
            moveDownCounter = Time.time+ moveDownDuration;
            downKeyCounter = Time.time + downKeyDuration;

            if (!boardManager.IsShapeInValidPos(activeShape)) 
            {
                if (boardManager.DidShapeOverflowTheGrid(activeShape)) // eðer þekil gridden taþtý ise oyun biter
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

    // þekil gidebileceði son noktaya geldikten sonra çalýþacak fonksiyon
    private void ShapeSettleDown()
    {
        PlaySfx(5); // yerleþme sfx index=5

        activeShape.PlaySettleEffects(); // yerleþme efektleri oynatýlýyor

        // deðerler yeni þekili yönlendirmek için bir nevi sýfýrlanýyor.
        rightLeftKeyCounter=Time.time;
        downKeyCounter=Time.time;
        rotateKeyCounter=Time.time;

        activeShape.MoveUp(); // bu satýr ile þekil hatalý pozisyondan çýkacak
        boardManager.TakeTheActiveShapeToGrid(activeShape); // þeklin parçalarýnýn bulunduðu grid kareleri meþgul duruma çekiliyor. bir nevi kaydediliyor

        if (shadowShape)
            shadowShape.ResetShadow();

        StartCoroutine(boardManager.CleanAllRows()); // þekil yerleþtikten sonra grid karelerinin doluluk durumuna göre silme iþlemi kntrolü yapýlacak

        // diðer þekilin gidecek bir yeri kalmadýðý için yeni bir þekil oluþturuluyor.
        activeShape = spawnerManager.CreateNextShape();
    }


    // oluþturulan random þekilin pozisyon deðerlerini tam sayý yapar. Böylelikle gridlere tam oturur
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
