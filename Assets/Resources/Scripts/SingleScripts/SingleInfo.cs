using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SingleInfo : MonoBehaviour
{
    [Header("�̱� ���� ������")]
    public SingleInfoData singleInfoData;

    int maxScore = 0;
    [Header("�̵� �� ������ ���Ծ��� �ִ� ���")]//���� ���� �ص� �Ǵ��� ������
    public BattleUIManager.RankType maxRank = BattleUIManager.RankType.E;

    [Header("�̵� �� ���� ���� ��ũ��Ʈ")]
    public SingleInfo PreInfo;
    [Header("�̱� ���� �Ŵ���")]
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
        if (singleInfoData == null)//1. ó������ �� ������ ������ ���
        {
            gameObject.SetActive(false);
            return;
        }

        btn = GetComponent<Button>();
        shader = transform.GetChild(0).GetComponent<Image>();
        icon = transform.GetChild(1).GetComponent<Image>();

        scoreText = transform.GetChild(2).GetComponent<Text>();

        if (PreInfo != null) //2. ���� ���� ������ B ���� �� ���
        {
            PreInfo.ScoreSynchronize();
            if ((int)PreInfo.maxRank > 2) 
            {
                btn.interactable = false;
                shader.gameObject.SetActive(false);
                return;
            }
        }   

        //3. ���� ����
        maxScore = singleSelectManager.battleUIManager.jsonManager.singleScore.LoadScore(singleInfoData.singleType, singleInfoData.sceneLevel);

        //���̴� ����
        shader.material = singleInfoData.shader;
        //������ ����
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
        //�ڵ� �󿡼� ����ϴ� �̸�
        singleSelectManager.singlePanelInnerTitle = singleInfoData.sceneInnerTitle;
        //���� ����ڿ��� �������� ���� �̸�
        singleSelectManager.singlePanelOutterTitle.text = singleInfoData.sceneOutterTitle;
        //���� ����ڿ��� �������� ���� ����
        singleSelectManager.singlePanelDesc.text = singleInfoData.sceneDesc;
        //���� ����
        singleSelectManager.battleUIManager.curLevel = singleInfoData.sceneLevel;
        //enum Ÿ�� ����, JSON���� ���
        singleSelectManager.battleUIManager.singleInfoType = singleInfoData.singleType;
        //���� ����ڿ��� �������� ��� ����
        singleSelectManager.singleRankText[0].text = "<color=#AA00FF> S </color>: " + singleInfoData.Sscore;
        singleSelectManager.singleRankText[1].text = "<color=#0000FF> A </color>: " + singleInfoData.Ascore;
        singleSelectManager.singleRankText[2].text = "<color=#00AA00> B </color>: " + singleInfoData.Bscore;
        singleSelectManager.singleRankText[3].text = "<color=#FF0000> C </color>: " + singleInfoData.Cscore;
        singleSelectManager.singleRankText[4].text = "<color=#FFFF00> D </color>: " + singleInfoData.Dscore;
        singleSelectManager.singleRankText[5].text = "<color=#FFFFFF> E </color>: " + singleInfoData.Escore;

        //���� ���� ����
        singleSelectManager.battleUIManager.Sscore = singleInfoData.Sscore;
        singleSelectManager.battleUIManager.Ascore = singleInfoData.Ascore;
        singleSelectManager.battleUIManager.Bscore = singleInfoData.Bscore;
        singleSelectManager.battleUIManager.Cscore = singleInfoData.Cscore;
        singleSelectManager.battleUIManager.Dscore = singleInfoData.Dscore;
        singleSelectManager.battleUIManager.Escore = singleInfoData.Escore;

        //���� ȿ����
        singleSelectManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //���� ��ư ����ȭ
        singleSelectManager.btnStart.SetActive(true);
    }
}
