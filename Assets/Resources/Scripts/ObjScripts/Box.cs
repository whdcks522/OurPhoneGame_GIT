using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoreanTyper;

public class Box : MonoBehaviour
{
    [Header("ä�� ������")]
    public GameObject boxChat;
    [Header("�ڽ� �̹���")]
    public Sprite openBoxImage;

    [Header("�ڽ� ���")]
    [TextArea]
    public string boxDesc;

    [Header("�÷��̾� ��ũ��Ʈ")]
    public CharacterControls characterControls;
    [Header("�÷��̾��� �ر��� �ɷ�")]
    public CharacterControls.PlayerStateType boxAbility;

    SpriteRenderer spriteRenderer;
    BattleUIManager battleUIManager;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        battleUIManager = BattleUIManager.Instance;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            battleUIManager.typingControl(boxDesc);
            
            if (boxChat.activeSelf) 
            {
                //���� ����
                battleUIManager.curScore += 10;
                //battleUIManager.bigScoreText.text = battleUIManager.curScore.ToString()+"/"+battleUIManager.Sscore;
                //ä�� �̹��� ��Ȱ��ȭ
                boxChat.SetActive(false);
                //�ڽ� �̹��� ��ȭ
                spriteRenderer.sprite = openBoxImage;
                //�÷��̾� �ɷ� �ر�
                characterControls.changeStateRPC(boxAbility, true);

                //��¦�� Į Ȱ��ȭ
                if (boxAbility == CharacterControls.PlayerStateType.RightControl) 
                {
                    characterControls.backSwords.SetActive(true);
                }
            }
        }
    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //�� ������ �������� ����
        {
            Destroy(this.gameObject);
        }
    }
}
