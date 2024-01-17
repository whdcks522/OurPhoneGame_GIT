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

    [Header("의상")]
    public Text clothesText;
    public GameObject colorParent;
    public GameObject[] colorArr;

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

        JsonManager.SaveData();
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
}
