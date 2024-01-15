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

        //�����Ƽ� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }

    #region �ǻ� ��ȯ
    public void changeClothes() 
    {
        //�ڽ� â ������
        hostBox.ControlAdavancedBox(false);

        //�� �ٽ� �����
        hostBox.characterControls.CharacterBuilder.Rebuild();
        //JSON �����ϱ�
        JsonManager.customJSON.clothesArr[0] = characterBuilder.Head;//���
        JsonManager.customJSON.clothesArr[1] = characterBuilder.Ears;//��
        JsonManager.customJSON.clothesArr[2] = characterBuilder.Eyes;//��
        JsonManager.customJSON.clothesArr[3] = characterBuilder.Body;//��
        JsonManager.customJSON.clothesArr[4] = characterBuilder.Hair;//�Ӹ�ī��

        JsonManager.customJSON.clothesArr[5] = characterBuilder.Armor;//����
        JsonManager.customJSON.clothesArr[6] = characterBuilder.Helmet;//����

        JsonManager.customJSON.clothesArr[7] = characterBuilder.Weapon;//����
        JsonManager.customJSON.clothesArr[8] = characterBuilder.Shield;//����
        JsonManager.customJSON.clothesArr[9] = characterBuilder.Cape;//����
        JsonManager.customJSON.clothesArr[10] = characterBuilder.Back;//��
        JsonManager.customJSON.clothesArr[11] = characterBuilder.Mask;//����ũ
        JsonManager.customJSON.clothesArr[12] = characterBuilder.Horns;//��

        JsonManager.SaveData();
    }
    #endregion
}
