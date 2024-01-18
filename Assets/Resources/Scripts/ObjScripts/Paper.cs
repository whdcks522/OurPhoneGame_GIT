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

    [Header("�ǻ� ���")]
    public Text clothesText;
    public GameObject colorParent;
    GameObject[] colorArr;

    [Header("���� �ΰ��� ���� ���")]
    public Text JumpSenseText;
    public Image leftJumpArea;
    public Image rightJumpArea;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        JsonManager = battleUIManager.jsonManager;
        characterBuilder = hostBox.characterControls.CharacterBuilder;

        //Ȱ��ȭ���ִٸ�, ��Ȱ��ȭ
        if(gameObject.activeSelf)
            gameObject.SetActive(false);

        //���� ����Ʈ ����
        colorArr = new GameObject[colorParent.transform.childCount];
        for (int i = 0; i < colorParent.transform.childCount; i++)
        {
            colorArr[i] = colorParent.transform.GetChild(i).gameObject;
        }

        //���� �ΰ��� �̹��� ������ ����
        changeJumpSense(0f);
    }

    #region �ǻ� ��ȯ
    public void saveData() 
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

        //������ ����
        JsonManager.SaveData();

        //���� �ΰ��� �̹��� ����
        battleUIManager.jumpSenseControl();
        //�÷��̾��� ���� �ΰ��� ��ġ ����
        hostBox.characterControls.jumpSense = JsonManager.customJSON.jumpSense;
    }
    #endregion

    #region �� ��ȯ
    public void changeColor(int index)
    {
        //ȿ���� ���
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);

        //���� ��Ȱ��ȭ
        foreach (GameObject go in colorArr) 
            go.SetActive(false);
        
        //������ ���� Ȱ��ȭ
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
        clothesText.text = "�ǻ�_"+str;
    }
    #endregion

    //���� ����
    public void playColorSfx()=> battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Ink);


    //float startValue = 0.01f;//
    void changeJumpSense(float _value)//���� �ΰ��� ����, �̹��� ����
    {
        JsonManager.customJSON.jumpSense += _value;

        if (JsonManager.customJSON.jumpSense > 0.85f) JsonManager.customJSON.jumpSense = 0.85f;
        else if (JsonManager.customJSON.jumpSense < -0.85f) JsonManager.customJSON.jumpSense = -0.85f;

        //�ؽ�Ʈ �����ϱ�
        JumpSenseText.text = (JsonManager.customJSON.jumpSense).ToString("F2");

        //�̹��� �����ϱ�
        leftJumpArea.fillAmount = - 0.25f * JsonManager.customJSON.jumpSense + 0.25f;//1�϶� 0
        rightJumpArea.fillAmount = -0.25f * JsonManager.customJSON.jumpSense + 0.25f;//-1�϶� 0.5
    }
    //-1�� +1��
    //0�� 0.5 ���̷� ��ȯ

    
    private void Update()
    {
        if (curTime >= 1f)
        {
            curTime = 1f;
            if (jumpSenseIndex != 0)
            {
                //�� �����ϱ�
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

    float curTime = 0f;//1�� ��⸦ ����

    int jumpSenseIndex = 0;//-1�̸� ����, +1�̸� ����
    public void clickJumpSense(int _input)//��ư ������ �� ����
    {
        //�ݼ� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

        //�ٷ� �������� �ʵ��� ����
        curTime = 0f;

        //���� ���� ����
        if(_input == -1)
            changeJumpSense(-0.01f);
        else if(_input == 1)
            changeJumpSense(0.01f);

        jumpSenseIndex = _input;
    }
}
