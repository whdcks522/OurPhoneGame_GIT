using Assets.PixelHeroes.Scripts.CharacterScripts;
using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SingleInfoData;

public class Paper : MonoBehaviour
{
    public Box hostBox;
    CharacterBuilder characterBuilder;
    BattleUIManager battleUIManager;
    JSONManager JsonManager;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        JsonManager = battleUIManager.jsonManager;
        characterBuilder = hostBox.characterControls.CharacterBuilder;

        //귀찮아서 비활성화
        gameObject.SetActive(false);
    }

    #region 의상 전환
    public void changeClothes() 
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
}
