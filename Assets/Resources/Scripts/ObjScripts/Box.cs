using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("ä�� ������")]
    public GameObject boxChat;
    [Header("�ڽ� �̹���")]
    public Sprite openBoxImage;

    [Header("�ڽ� ���")]
    [TextArea]
    public string boxDesc;

    [Header("�÷��̾�")]
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
            //ȿ����
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
