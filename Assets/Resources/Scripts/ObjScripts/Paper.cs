using Assets.PixelHeroes.Scripts.CharacterScripts;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SingleInfoData;

public class Paper : MonoBehaviour
{
    public Box hostBox;
    CharacterBuilder characterBuilder;
    BattleUIManager battleUIManager;
    JSONManager JsonManager;

    [Header("의상 요소")]
    public Text clothesText;
    public GameObject colorParent;
    GameObject[] colorArr;

    [Header("점프 민감도 조작 요소")]
    public Text JumpSenseText;
    public Image leftJumpArea;
    public Image rightJumpArea;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        JsonManager = battleUIManager.jsonManager;
        characterBuilder = hostBox.characterControls.CharacterBuilder;

        //활성화돼있다면, 비활성화
        if(gameObject.activeSelf)
            gameObject.SetActive(false);

        //색깔 리스트 저장
        colorArr = new GameObject[colorParent.transform.childCount];
        for (int i = 0; i < colorParent.transform.childCount; i++)
        {
            colorArr[i] = colorParent.transform.GetChild(i).gameObject;
        }

        //점프 민감도 이미지 갱신을 위함
        changeJumpSense(0f);
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

    //색깔 전용
    public void playColorSfx()=> battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Ink);


    //float startValue = 0.01f;//
    void changeJumpSense(float _value)//점프 민감도 조작, 이미지 갱신
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
    //-1과 +1을
    //0과 0.5 사이로 변환

    
    private void Update()
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

    float curTime = 0f;//1초 대기를 위함

    int jumpSenseIndex = 0;//-1이면 감소, +1이면 증가
    public void clickJumpSense(int _input)//버튼 눌러서 값 조절
    {
        //금속 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

        //바로 증가하지 않도록 설정
        curTime = 0f;

        //누른 순간 증가
        if(_input == -1)
            changeJumpSense(-0.01f);
        else if(_input == 1)
            changeJumpSense(0.01f);

        jumpSenseIndex = _input;
    }
}
