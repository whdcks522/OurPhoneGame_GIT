using Assets.PixelHeroes.Scripts.CharacterScripts;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;
using static SingleInfoData;
//using static UnityEditor.PlayerSettings;

public class Paper : MonoBehaviour
{
    public Box hostBox;
    public GameManager gameManager;

    CharacterBuilder characterBuilder;
    BattleUIManager battleUIManager;
    JSONManager JsonManager;

    public enum PaperType { Player, Bullet, Enemy };
    public PaperType paperType;

    [Header("의상 요소")]
    public Text clothesText;
    public GameObject colorParent;
    GameObject[] colorArr;

    [Header("점프 민감도 조작 요소")]
    public Text JumpSenseText;
    public Image leftJumpArea;
    public Image rightJumpArea;

    [Header("투사체 저장소 요소")]
    public Bullet bulletScript;
    public Text bulletTitleText;
    [TextArea]
    public string bulletTitle;
    public BulletShotter bulletShotter;
    public GameObject bulletStart;
    public GameObject bulletEnd;
    public Text bulletDmgText;
    public Text bulletCureText;
    public Text bulletSpdText;
    public Text bulletDescText;
    [TextArea]
    public string bulletDesc;

    [Header("빌런 저장소 요소")]
    public Enemy enemyScript;
    public Text enemyTitleText;
    [TextArea]
    public string enemyTitle;
    public Transform enemyPoint;
    public Text enemyHealthText;
    public Text enemyCureText;
    public Text enemyDescText;
    [TextArea]
    public string enemyDesc;


    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        if (battleUIManager != null)//테스트에서 간편성 증가
            JsonManager = battleUIManager.jsonManager;

        if (paperType == PaperType.Player)//플레이어 세팅 패널
        {
            //활성화돼있다면, 비활성화
            //if (gameObject.activeSelf)
            {
                //gameObject.SetActive(false);
                //Debug.Log("비활성화");
            }

            characterBuilder = hostBox.characterControls.CharacterBuilder;

            colorArr = new GameObject[colorParent.transform.childCount];
            for (int i = 0; i < colorParent.transform.childCount; i++)
            {
                colorArr[i] = colorParent.transform.GetChild(i).gameObject;
            }

            //점프 민감도 이미지 갱신을 위함
            changeJumpSense(0f);
        }
        else if (paperType == PaperType.Bullet)//투사체 패널
        {
            descPanelControl();
        }
        else if (paperType == PaperType.Enemy)//빌런 패널
        {
            //바로 하면 위치 오류
            Invoke("descPanelControl", 0.1f);
        }

    }

    #region 의상 전환
    public void saveData() 
    {
        //박스 창 나가기
        hostBox.ControlAdavancedBox(false);

        //몸 다시 만들기
        hostBox.characterControls.CharacterBuilder.Rebuild();

        //JSON 저장하기
        JsonManager.customJSON.clothesArr[0] = characterBuilder.Head;//헤드
        JsonManager.customJSON.clothesArr[1] = characterBuilder.Ears;//귀
        JsonManager.customJSON.clothesArr[2] = characterBuilder.Eyes;//눈
        JsonManager.customJSON.clothesArr[3] = characterBuilder.Body;//몸
        JsonManager.customJSON.clothesArr[4] = characterBuilder.Hair;//머리카락

        JsonManager.customJSON.clothesArr[5] = characterBuilder.Armor;//갑옷
        JsonManager.customJSON.clothesArr[6] = characterBuilder.Helmet;//모자

        JsonManager.customJSON.clothesArr[7] = characterBuilder.Weapon;//무기
        JsonManager.customJSON.clothesArr[8] = characterBuilder.Shield;//방패
        JsonManager.customJSON.clothesArr[9] = characterBuilder.Cape;//망토
        JsonManager.customJSON.clothesArr[10] = characterBuilder.Back;//등
        JsonManager.customJSON.clothesArr[11] = characterBuilder.Mask;//마스크
        JsonManager.customJSON.clothesArr[12] = characterBuilder.Horns;//뿔

        //데이터 저장
        JsonManager.SaveData();

        //점프 민감도 이미지 설정
        battleUIManager.jumpSenseControl();
        //플레이어의 점프 민감도 수치 설정
        hostBox.characterControls.jumpSense = JsonManager.customJSON.jumpSense;
    }
    #endregion

    #region 색 전환
    public void changeColor(int index)
    {
        //효과음 출력
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);

        //전부 비활성화
        foreach (GameObject go in colorArr) 
            go.SetActive(false);
        
        //선택한 색만 활성화
        colorArr[index].SetActive(true);

        string str = "";
        switch (index) 
        {
            case 0:
            default:
                str = "HEAD";
                break;
            case 1:
                str = "EARS";
                break;
            case 2:
                str = "BODY";
                break;
            case 3:
                str = "EYES";
                break;
            case 4:
                str = "MASK";
                break;
            case 5:
                str = "HAIR";
                break;
            case 6:
                str = "ARMOR";
                break;
            case 7:
                str = "HELMET";
                break;
        }
        clothesText.text = "의상_"+str;
    }
    #endregion

    //옷 색깔 바꿈 전용
    public void playColorSfx()=> battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Ink);

    #region 점프 민감도 조작, 이미지 갱신
    void changeJumpSense(float _value)
    {
        JsonManager.customJSON.jumpSense += _value;

        if (JsonManager.customJSON.jumpSense > 0.85f) JsonManager.customJSON.jumpSense = 0.85f;
        else if (JsonManager.customJSON.jumpSense < -0.85f) JsonManager.customJSON.jumpSense = -0.85f;

        //텍스트 갱신하기
        JumpSenseText.text = (JsonManager.customJSON.jumpSense).ToString("F2");

        //이미지 갱신하기
        leftJumpArea.fillAmount = - 0.25f * JsonManager.customJSON.jumpSense + 0.25f;//1일때 0
        rightJumpArea.fillAmount = -0.25f * JsonManager.customJSON.jumpSense + 0.25f;//-1일때 0.5
    }
    #endregion

    private void Update()
    {
        if (paperType == PaperType.Player)
        {
            if (curTime >= 1f)
            {
                curTime = 1f;
                if (jumpSenseIndex != 0)
                {
                    //값 갱신하기
                    if (jumpSenseIndex == -1)
                        changeJumpSense(-0.005f);
                    else if (jumpSenseIndex == +1)
                        changeJumpSense(0.005f);
                }
            }
            else if (curTime < 1f)
            {
                curTime += Time.deltaTime;
            }
        }
        else if (paperType == PaperType.Bullet)
        {
            curTime += Time.deltaTime;
            if (curTime >= 1.5f)//1초마다 발사
            {
                //시간 초기화
                curTime = 0f;
                //직선 총알을 약간 꺽어서 생성
                bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, bulletScript.bulletEffectType, bulletStart, bulletEnd, 0);
            }
        }
    }

    float curTime = 0f;//플레이어 세팅에서, 1초 대기를 위함
    int jumpSenseIndex = 0;//-1 이면 점프 민감도 감소, +1 이면 점프 민감도 증가

    #region 점프 민감도 조작 버튼
    public void clickJumpSense(int _input)//버튼 눌러서 값 조절
    {
        //바로 증가하지 않도록 설정
        curTime = 0f;

        //누른 순간 증가
        if (_input != 0) 
        {
            //금속 효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

            if (_input == -1)
                changeJumpSense(-0.01f);
            else if (_input == 1) 
                changeJumpSense(0.01f);
        }
        
        jumpSenseIndex = _input;
    }
    #endregion

    #region 패널 조작(시작했을 때, 보여줌)
    void descPanelControl() 
    {
        if (paperType == PaperType.Bullet)//투사체 패널
        {
            //제목
            bulletTitleText.text = bulletTitle;

            //투사체 설정
            bulletShotter.gameManager = gameManager;

            //피해량
            bulletDmgText.text = "피해량: " + bulletScript.bulletDamage.ToString();

            //회복량
            bulletCureText.text = "회복량: " + bulletScript.bulletHeal.ToString();

            //속도
            bulletSpdText.text = "속도: " + bulletScript.bulletSpeed.ToString();

            //설명
            bulletDescText.text = bulletDesc;
        }
        else if (paperType == PaperType.Enemy)//빌런 패널
        {
            //제목
            enemyTitleText.text = enemyTitle;

            //체력
            enemyHealthText.text = "체력: " + enemyScript.maxHealth.ToString();

            //회복량
            enemyCureText.text = "회복량: " + enemyScript.playerHeal.ToString();

            //설명
            enemyDescText.text = enemyDesc;

            //적 생성
            string type = "";
            switch (enemyScript.enemyType)
            {
                case Enemy.EnemyType.Goblin:
                    type = "Enemy_Goblin";
                    break;
                case Enemy.EnemyType.Orc:
                    type = "Enemy_Orc";
                    break;
                case Enemy.EnemyType.Lizard:
                    type = "Enemy_Lizard";
                    break;
            }

            GameObject enemyGameObject = gameManager.CreateObj(type, GameManager.PoolTypes.EnemyType);
            Enemy enemyComponent = enemyGameObject.GetComponent<Enemy>();
            enemyComponent.gameManager = gameManager;
            enemyComponent.player = gameManager.player;

            enemyComponent.isPrison = true;//공격 방지

            //적 위치 조정
            enemyGameObject.transform.position = enemyPoint.position;

            //적 활성화
            enemyComponent.activateRPC();
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)//카메라 이동
    {
        if (paperType == PaperType.Bullet) 
        {
            if (collision.transform.CompareTag("Player"))
            {
                gameManager.cameraControl(bulletStart.transform);
            }
        }
        else if (paperType == PaperType.Enemy)
        {
            if (collision.transform.CompareTag("Player"))
            {
                gameManager.cameraControl(enemyPoint);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)//카메라 복귀
    {
        if (paperType == PaperType.Bullet || paperType == PaperType.Enemy)
        {
            if (collision.transform.CompareTag("Player"))
            {
                gameManager.cameraControl(gameManager.player.transform);
            }
        }
    }
}
