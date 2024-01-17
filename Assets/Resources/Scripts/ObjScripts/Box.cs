using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KoreanTyper;
using Photon.Pun.Demo.PunBasics;

public class Box : MonoBehaviour
{
    [Header("채팅 아이콘")]
    public GameObject boxChat;
    [Header("열린 박스 이미지")]
    public Sprite openBoxImage;
    [Header("닫힌 박스 이미지")]
    public Sprite closeBoxImage;

    [Header("박스 대사")]
    [TextArea]
    public string boxDesc;

    [Header("플레이어 스크립트")]
    public CharacterControls characterControls;
    [Header("플레이어의 해금할 능력")]
    public CharacterControls.PlayerStateType boxAbility;

    SpriteRenderer spriteRenderer;
    BattleUIManager battleUIManager;
    public GameManager gameManager;

    [Header("펼칠 종이(종이 사용 여부)")]
    public GameObject customPaper;
    Rigidbody2D rigid;
    public Transform cameraTarget;
    

    public enum BoxType { Train, Paper }
    public BoxType boxType;

    //헤드: Hair2#FFFFFF/0:0:0
    //눈: Human#FFFFFF/0:0:0
    //몸: Human#FFFFFF/0:0:0
    //머리카락: Hair2#FFFFFF/0:0:0
    //아머: PirateCostume#FFFFFF/0:0:0


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        battleUIManager = BattleUIManager.Instance;

        if (boxType == BoxType.Paper)//굳히기
            rigid.bodyType = RigidbodyType2D.Static;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            if (boxType == BoxType.Train)//훈련용 상자
            {
                #region 훈련용 상자 작동

                //타이핑
                characterControls.loopTypingRPC(CharacterControls.TypingType.None, boxDesc, false);

                if (boxChat.activeSelf)
                {
                    //열린 형태로 변화
                    changeForm(true);
                    //점수 증가
                    battleUIManager.curScore += 10;
                    //플레이어 능력 해금
                    characterControls.changeStateRPC(boxAbility, true);


                    //등짝의 칼 활성화
                    if (boxAbility == CharacterControls.PlayerStateType.RightControl)
                    {
                        characterControls.backSwords.SetActive(true);
                    }
                    //효과음 출력
                    battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.PowerUp);
                    //이펙트 생성
                    GameObject effect = gameManager.CreateObj("congratulation 9", GameManager.PoolTypes.EffectType);
                    effect.SetActive(true);
                    effect.transform.position = transform.position;
                }
                #endregion
            }
            else if (boxType == BoxType.Paper)//종이를 펼치는 상자 
            {

                ControlAdavancedBox(true);
            }
        }
    }

    #region 향상된 박스 컨트롤
    public void ControlAdavancedBox(bool isOpen) 
    {
        //효과음 출력
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);

        //박스를 열린 형태로 변화
        changeForm(isOpen);
        //종이 열기
        customPaper.SetActive(isOpen);
        //카메라 관리
        if(isOpen)
            gameManager.cameraControl(cameraTarget);
        else if (!isOpen)
            gameManager.cameraControl(gameManager.player.transform);

        //플레이어 움직임 제한(안놓은 상태면, 계속 조작해서 필요함)
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.LeftControl, !isOpen);
        characterControls.changeStateRPC(CharacterControls.PlayerStateType.RightControl, !isOpen);
        //조이스틱 크기 조절
        battleUIManager.JoySizeControl(!isOpen);
        //플레이어 정지
        if (isOpen)
            characterControls.xyRPC(0, 0);
    }
    #endregion
 

    #region 박스 이미지 전환
    public void changeForm(bool isOpen)
    {
        //채팅 이미지 비활성화
        boxChat.SetActive(!isOpen);
        if (isOpen) //연 것
        {
            //박스 이미지 변화
            spriteRenderer.sprite = openBoxImage;
        }
        else if (!isOpen)//닫은 것
        {
            //박스 이미지 변화
            spriteRenderer.sprite = closeBoxImage;
        }
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Outline")) //맵 밖으로 나가지면 종료
        {
            Destroy(this.gameObject);
            Debug.Log("박스 나감");
        }
    }
}
