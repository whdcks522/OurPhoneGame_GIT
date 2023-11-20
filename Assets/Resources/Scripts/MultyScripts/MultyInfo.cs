using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultyInfo : MonoBehaviour
{
    public Sprite icon;

    public LobbyManager lobbyManager;

    [Header("이동 할 씬 설명")]
    [TextArea]
    public string sceneDesc;

    [Header("이동 할 씬 내부 이름")]
    public string sceneInnerTitle;

    [Header("이동 할 씬 외부 이름")]
    public string sceneOutterTitle;

    [Header("이동 할 씬의 참가자 수")]
    public int sceneMax;

    [Header("현재 버튼의 이미지")]
    public Sprite btnImage;

    private void Awake()
    {
        btnImage = transform.GetChild(1).GetComponent<Image>().sprite;
    }

    public void Onclick()
    {
        //개발자에게 보여지는 전장 이름
        lobbyManager.SceneInnerStr = sceneInnerTitle;
        //실제 사용자에게 보여지는 전장 이름
        lobbyManager.SceneOutterText.text = sceneOutterTitle;
        //실제 사용자에게 보여지는 전장 참가자수 설명
        lobbyManager.SceneMaxText.text = sceneMax+"명";
        lobbyManager.maxPlayerNumber = sceneMax;
        //실제 사용자에게 보여지는 전장 설명
        lobbyManager.SceneDescText.text = sceneDesc;
        //이미지 전달
        lobbyManager.cellSprite = btnImage;
        //종이 효과음
        lobbyManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //시작 버튼 가시화
        lobbyManager.CreateRoomBtn.SetActive(true);
    }
}
