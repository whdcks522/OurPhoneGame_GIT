using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SingleInfo : MonoBehaviour
{
    public SingleSelectManager singleSelectManager;

    [Header("이동 할 씬 내부 이름")]
    public string sceneInnerTitle;
    [Header("이동 할 씬 외부 이름")]
    public string sceneOutterTitle;
    [Header("이동 할 씬 설명")]
    [TextArea]
    public string sceneDesc;

    public bool isVisible = true;
    public int sceneLevel;
    [Header("이동 할 씬의 선행 스크립트")]
    public SingleInfo PreInfo;
    [Header("이동 할 씬에서 나왔었던 최대 점수")]
    public int Maxscore;
    [Header("이동 할 씬에서 나왔었던 최대 등급")]
    public BattleUIManager.RankType MaxRank;

    [Header("이동 할 씬 목표 점수들")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;

    void Start() 
    {
        gameObject.SetActive(false);

        if (isVisible || sceneLevel == 0) 
            gameObject.SetActive(true);
    }
    public void Onclick() 
    {
        //코드 상에서 사용하는 이름
        singleSelectManager.singlePanelInnerTitle = sceneInnerTitle;
        //실제 사용자에게 보여지는 전장 이름
        singleSelectManager.singlePanelOutterTitle.text = sceneOutterTitle;
        //실제 사용자에게 보여지는 전장 설명
        singleSelectManager.singlePanelDesc.text = sceneDesc;
        //실제 사용자에게 보여지는 등급 설명
        singleSelectManager.singleRankText[0].text = "<color=#AA00FF> S </color>: " + Sscore;
        singleSelectManager.singleRankText[1].text = "<color=#0000FF> A </color>: " + Ascore;
        singleSelectManager.singleRankText[2].text = "<color=#00AA00> B </color>: " + Bscore;
        singleSelectManager.singleRankText[3].text = "<color=#FF0000> C </color>: " + Cscore;
        singleSelectManager.singleRankText[4].text = "<color=#FFFF00> D </color>: " + Dscore;
        singleSelectManager.singleRankText[5].text = "<color=#FFFFFF> E </color>: " + Escore;

        //전역 변수 설정
        singleSelectManager.battleUIManager.Sscore = Sscore;
        singleSelectManager.battleUIManager.Ascore = Ascore;
        singleSelectManager.battleUIManager.Bscore = Bscore;
        singleSelectManager.battleUIManager.Cscore = Cscore;
        singleSelectManager.battleUIManager.Dscore = Dscore;
        singleSelectManager.battleUIManager.Escore = Escore;

        //종이 효과음
        singleSelectManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //시작 버튼 가시화
        singleSelectManager.btnStart.SetActive(true);
    }
}
