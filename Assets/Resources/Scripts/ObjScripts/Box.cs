using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("채팅 아이콘")]
    public GameObject boxChat;
    [Header("박스 이미지")]
    public Sprite openBoxImage;

    [Header("박스 대사")]
    [TextArea]
    public string boxDesc;

    [Header("플레이어")]
    public CharacterControls characterControls;

    public CharacterControls.PlayerStateType boxAbility;

    SpriteRenderer spriteRenderer;
    BattleUIManager battleUIManager;
    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && boxChat.activeSelf) 
        {
            //효과음
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.RankUp);

            Debug.Log(boxDesc);
            boxChat.SetActive(false);
            spriteRenderer.sprite = openBoxImage;
        }   
    }

    private void FixedUpdate()
    {
        
    }
}
