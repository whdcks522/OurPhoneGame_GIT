using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoreanTyper;

public class Box : MonoBehaviour
{
    [Header("채팅 아이콘")]
    public GameObject boxChat;
    [Header("박스 이미지")]
    public Sprite openBoxImage;

    [Header("박스 대사")]
    [TextArea]
    public string boxDesc;

    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;
    [Header("플레이어의 해금할 능력")]
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
                //점수 증가
                battleUIManager.curScore += 10;
                //battleUIManager.bigScoreText.text = battleUIManager.curScore.ToString()+"/"+battleUIManager.Sscore;
                //채팅 이미지 비활성화
                boxChat.SetActive(false);
                //박스 이미지 변화
                spriteRenderer.sprite = openBoxImage;
                //플레이어 능력 해금
                characterControls.changeStateRPC(boxAbility, true);

                //등짝의 칼 활성화
                if (boxAbility == CharacterControls.PlayerStateType.RightControl) 
                {
                    characterControls.backSwords.SetActive(true);
                }
            }
        }
    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
        {
            Destroy(this.gameObject);
        }
    }
}
