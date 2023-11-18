using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SingleInfo : MonoBehaviour
{
    [Header("사용 불가능한 씬인가")]
    public bool isInVisible;


    public enum SingleType
    {
        Train, StarFall, BlockCrash, Fly
    }
    [Header("싱글 게임 타입")]
    public SingleType singleType;

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
    [Header("씬 레벨")]
    public int sceneLevel;

    //[Header("이동 할 씬에서 나왔었던 최대 점수")]
    int maxScore = 0;
    [Header("이동 할 씬에서 나왔었던 최대 등급")]//다음 씬을 해도 되는지 보여줌
    public BattleUIManager.RankType maxRank = BattleUIManager.RankType.E;


    [Header("이동 할 씬의 선행 스크립트")]
    public SingleInfo PreInfo;

    public SingleSelectManager singleSelectManager;
    Button btn;
    GameObject shader;
    Text scoreText;
    string[] rankStrArr = {"S ", "A ", "B ", "C ", "D ", "E "};
    private void Start()
    {
        ScoreSynchronize();
    }

    public void ScoreSynchronize() 
    {
        if (isInVisible)//1. 처음부터 줄 생각이 없었던 경우
        {
            gameObject.SetActive(false);
            return;
        }

        btn = GetComponent<Button>();
        shader = transform.GetChild(0).gameObject;

        scoreText = transform.GetChild(2).GetComponent<Text>();

        if (PreInfo != null) //2. 선행 조건 게임이 B 이하 일 경우
        {
            PreInfo.ScoreSynchronize();
            if ((int)PreInfo.maxRank > 2) 
            {
                btn.interactable = false;
                shader.SetActive(false);
                return;
            }
        }   

        //3. 정상 실행
        maxScore = singleSelectManager.battleUIManager.jsonManager.singleScore.LoadScore(singleType, sceneLevel);

        if (maxScore >= Sscore)
        {
            maxRank = BattleUIManager.RankType.S;
            scoreText.text = "<color=#AA00FF>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= Ascore)
        {
            maxRank = BattleUIManager.RankType.A;
            scoreText.text = "<color=#0000FF>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= Bscore)
        {
            maxRank = BattleUIManager.RankType.B;
            scoreText.text = "<color=#00AA00>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= Cscore)
        {
            maxRank = BattleUIManager.RankType.C;
            scoreText.text = "<color=#FF0000>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= Dscore)
        {
            maxRank = BattleUIManager.RankType.D;
            scoreText.text = "<color=#FFFF00>"+rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= Escore)
        {
            maxRank = BattleUIManager.RankType.E;
            scoreText.text = "<color=#FFFFFF>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }

        
    }

    public void Onclick() 
    {
        //코드 상에서 사용하는 이름
        singleSelectManager.singlePanelInnerTitle = sceneInnerTitle;
        //실제 사용자에게 보여지는 전장 이름
        singleSelectManager.singlePanelOutterTitle.text = sceneOutterTitle;
        //실제 사용자에게 보여지는 전장 설명
        singleSelectManager.singlePanelDesc.text = sceneDesc;
        //레벨 적용
        singleSelectManager.battleUIManager.curLevel = sceneLevel;
        //enum 타입 선언, JSON에서 사용
        singleSelectManager.battleUIManager.singleType = singleType;
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
