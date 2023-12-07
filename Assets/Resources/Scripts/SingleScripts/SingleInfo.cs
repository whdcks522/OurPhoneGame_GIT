using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SingleInfo : MonoBehaviour
{
    [Header("싱글 게임 데이터")]
    public SingleInfoData singleInfoData;

    int maxScore = 0;
    [Header("이동 할 씬에서 나왔었던 최대 등급")]//다음 씬을 해도 되는지 보여줌
    public BattleUIManager.RankType maxRank = BattleUIManager.RankType.E;

    [Header("이동 할 씬의 선행 스크립트")]
    public SingleInfo PreInfo;
    [Header("싱글 게임 매니저")]
    public SingleSelectManager singleSelectManager;

    Button btn;

    Image shader;
    Image icon;
    Text scoreText;
    string[] rankStrArr = {"S ", "A ", "B ", "C ", "D ", "E "};

    private void Start()
    {
        ScoreSynchronize();
    }

    public void ScoreSynchronize() 
    {
        if (singleInfoData == null)//1. 처음부터 줄 생각이 없었던 경우
        {
            gameObject.SetActive(false);
            return;
        }

        btn = GetComponent<Button>();
        shader = transform.GetChild(0).GetComponent<Image>();
        icon = transform.GetChild(1).GetComponent<Image>();

        scoreText = transform.GetChild(2).GetComponent<Text>();

        if (PreInfo != null) //2. 선행 조건 게임이 B 이하 일 경우
        {
            PreInfo.ScoreSynchronize();
            if ((int)PreInfo.maxRank > 2) 
            {
                btn.interactable = false;
                shader.gameObject.SetActive(false);
                return;
            }
        }   

        //3. 정상 실행
        maxScore = singleSelectManager.battleUIManager.jsonManager.singleScore.LoadScore(singleInfoData.singleType, singleInfoData.sceneLevel);

        //쉐이더 설정
        shader.material = singleInfoData.shader;
        //아이콘 설정
        icon.sprite = singleInfoData.icon;

        if (maxScore >= singleInfoData.Sscore)
        {
            maxRank = BattleUIManager.RankType.S;
            scoreText.text = "<color=#AA00FF>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= singleInfoData.Ascore)
        {
            maxRank = BattleUIManager.RankType.A;
            scoreText.text = "<color=#0000FF>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= singleInfoData.Bscore)
        {
            maxRank = BattleUIManager.RankType.B;
            scoreText.text = "<color=#00AA00>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= singleInfoData.Cscore)
        {
            maxRank = BattleUIManager.RankType.C;
            scoreText.text = "<color=#FF0000>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= singleInfoData.Dscore)
        {
            maxRank = BattleUIManager.RankType.D;
            scoreText.text = "<color=#FFFF00>"+rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }
        else if (maxScore >= singleInfoData.Escore)
        {
            maxRank = BattleUIManager.RankType.E;
            scoreText.text = "<color=#FFFFFF>"+ rankStrArr[(int)maxRank] + maxScore.ToString() +"</color>";
        }

        
    }

    public void Onclick() 
    {
        //코드 상에서 사용하는 이름
        singleSelectManager.singlePanelInnerTitle = singleInfoData.sceneInnerTitle;
        //실제 사용자에게 보여지는 전장 이름
        singleSelectManager.singlePanelOutterTitle.text = singleInfoData.sceneOutterTitle;
        //실제 사용자에게 보여지는 전장 설명
        singleSelectManager.singlePanelDesc.text = singleInfoData.sceneDesc;
        //레벨 적용
        singleSelectManager.battleUIManager.curLevel = singleInfoData.sceneLevel;
        //enum 타입 선언, JSON에서 사용
        singleSelectManager.battleUIManager.singleInfoType = singleInfoData.singleType;
        //실제 사용자에게 보여지는 등급 설명
        singleSelectManager.singleRankText[0].text = "<color=#AA00FF> S </color>: " + singleInfoData.Sscore;
        singleSelectManager.singleRankText[1].text = "<color=#0000FF> A </color>: " + singleInfoData.Ascore;
        singleSelectManager.singleRankText[2].text = "<color=#00AA00> B </color>: " + singleInfoData.Bscore;
        singleSelectManager.singleRankText[3].text = "<color=#FF0000> C </color>: " + singleInfoData.Cscore;
        singleSelectManager.singleRankText[4].text = "<color=#FFFF00> D </color>: " + singleInfoData.Dscore;
        singleSelectManager.singleRankText[5].text = "<color=#FFFFFF> E </color>: " + singleInfoData.Escore;

        //전역 변수 설정
        singleSelectManager.battleUIManager.Sscore = singleInfoData.Sscore;
        singleSelectManager.battleUIManager.Ascore = singleInfoData.Ascore;
        singleSelectManager.battleUIManager.Bscore = singleInfoData.Bscore;
        singleSelectManager.battleUIManager.Cscore = singleInfoData.Cscore;
        singleSelectManager.battleUIManager.Dscore = singleInfoData.Dscore;
        singleSelectManager.battleUIManager.Escore = singleInfoData.Escore;

        //종이 효과음
        singleSelectManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //시작 버튼 가시화
        singleSelectManager.btnStart.SetActive(true);
    }
}
