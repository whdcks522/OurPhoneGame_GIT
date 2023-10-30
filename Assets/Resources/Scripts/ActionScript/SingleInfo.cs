using System.Collections;
using System.Collections.Generic;
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

    [Header("이동 할 씬 목표 점수들")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;

    public void Onclick() 
    {
        //코드 상에서 사용하는 이름
        singleSelectManager.singlePanelInnerTitle = sceneInnerTitle;
        //실제 사용자에게 보여지는 전장 이름
        singleSelectManager.singlePanelOutterTitle.text = sceneOutterTitle;
        //실제 사용자에게 보여지는 전장 설명
        singleSelectManager.singlePanelDesc.text = sceneDesc;
        //실제 사용자에게 보여지는 등급 설명
        singleSelectManager.singleRankText.text = "S: " + Sscore + " A: " + Ascore + '\n' +
                                                    "B: " + Bscore + " C: " + Cscore + '\n' +
                                                      "D: " + Dscore + " E: " + Escore;

        //전역 변수 설정
        singleSelectManager.battleUIManager.Sscore = Sscore;
        singleSelectManager.battleUIManager.Ascore = Ascore;
        singleSelectManager.battleUIManager.Bscore = Bscore;
        singleSelectManager.battleUIManager.Cscore = Cscore;
        singleSelectManager.battleUIManager.Dscore = Dscore;
        singleSelectManager.battleUIManager.Escore = Escore;

        //종이 효과음
        singleSelectManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
    }
}
