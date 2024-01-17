using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoreanTyper;
using Photon.Pun.Demo.PunBasics;

public class Box : MonoBehaviour
{
    [Header("ä�� ������")]
    public GameObject boxChat;
    [Header("���� �ڽ� �̹���")]
    public Sprite openBoxImage;
    [Header("���� �ڽ� �̹���")]
    public Sprite closeBoxImage;

    [Header("�ڽ� ���")]
    [TextArea]
    public string boxDesc;

    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;
    [Header("�÷��̾��� �ر��� �ɷ�")]
    public CharacterControls.PlayerStateType boxAbility;

    SpriteRenderer spriteRenderer;
    BattleUIManager battleUIManager;
    public GameManager gameManager;

    [Header("��ĥ ����(���� ��� ����)")]
    public GameObject customPaper;
    Rigidbody2D rigid;
    public Transform cameraTarget;
    

    public enum BoxType { Train, Paper }
    public BoxType boxType;

    //���: Hair2#FFFFFF/0:0:0
    //��: Human#FFFFFF/0:0:0
    //��: Human#FFFFFF/0:0:0
    //�Ӹ�ī��: Hair2#FFFFFF/0:0:0
    //�Ƹ�: PirateCostume#FFFFFF/0:0:0


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        battleUIManager = BattleUIManager.Instance;

        if (boxType == BoxType.Paper)//������
            rigid.bodyType = RigidbodyType2D.Static;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (boxType == BoxType.Train)//�Ʒÿ� ����
            {
                #region �Ʒÿ� ���� �۵�

                //Ÿ����
                characterControls.loopTypingRPC(CharacterControls.TypingType.None, boxDesc, false);

                if (boxChat.activeSelf)
                {
                    //���� ���·� ��ȭ
                    changeForm(true);
                    //���� ����
                    battleUIManager.curScore += 10;
                    //�÷��̾� �ɷ� �ر�
                    characterControls.changeStateRPC(boxAbility, true);


                    //��¦�� Į Ȱ��ȭ
                    if (boxAbility == CharacterControls.PlayerStateType.RightControl)
                    {
                        characterControls.backSwords.SetActive(true);
                    }
                    //ȿ���� ���
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.PowerUp);
                    //����Ʈ ����
                    GameObject effect = gameManager.CreateObj("congratulation 9", GameManager.PoolTypes.EffectType);
                    effect.SetActive(true);
                    effect.transform.position = transform.position;
                }
                #endregion
            }
            else if (boxType == BoxType.Paper)//���̸� ��ġ�� ���� 
            {

                ControlAdavancedBox(true);
            }
        }
    }

    #region ���� �ڽ� ��Ʈ��
    public void ControlAdavancedBox(bool isOpen) 
    {
        //ȿ���� ���
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);

        //�ڽ��� ���� ���·� ��ȭ
        changeForm(isOpen);
        //���� ����
        customPaper.SetActive(isOpen);
        //ī�޶� ����
        if(isOpen)
            gameManager.cameraControl(cameraTarget);
        else if (!isOpen)
            gameManager.cameraControl(gameManager.player.transform);

        //�÷��̾� ������ ����(�ȳ��� ���¸�, ��� �����ؼ� �ʿ���)
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, !isOpen);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, !isOpen);
        //���̽�ƽ ũ�� ����
        battleUIManager.JoySizeControl(!isOpen);
        //�÷��̾� ����
        if (isOpen)
            characterControls.xyRPC(0, 0);
    }
    #endregion
 

    #region �ڽ� �̹��� ��ȯ
    public void changeForm(bool isOpen)
    {
        //ä�� �̹��� ��Ȱ��ȭ
        boxChat.SetActive(!isOpen);
        if (isOpen) //�� ��
        {
            //�ڽ� �̹��� ��ȭ
            spriteRenderer.sprite = openBoxImage;
        }
        else if (!isOpen)//���� ��
        {
            //�ڽ� �̹��� ��ȭ
            spriteRenderer.sprite = closeBoxImage;
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
        {
            Destroy(this.gameObject);
            Debug.Log("�ڽ� ����");
        }
    }
}
