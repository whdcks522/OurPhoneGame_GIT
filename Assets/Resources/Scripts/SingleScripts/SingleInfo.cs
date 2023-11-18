using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SingleInfo : MonoBehaviour
{
    [Header("��� �Ұ����� ���ΰ�")]
    public bool isInVisible;


    public enum SingleType
    {
        Train, StarFall, BlockCrash, Fly
    }
    [Header("�̱� ���� Ÿ��")]
    public SingleType singleType;

    [Header("�̵� �� �� ���� �̸�")]
    public string sceneInnerTitle;
    [Header("�̵� �� �� �ܺ� �̸�")]
    public string sceneOutterTitle;
    [Header("�̵� �� �� ����")]
    [TextArea]
    public string sceneDesc;


    [Header("�̵� �� �� ��ǥ ������")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;
    [Header("�� ����")]
    public int sceneLevel;

    //[Header("�̵� �� ������ ���Ծ��� �ִ� ����")]
    int maxScore = 0;
    [Header("�̵� �� ������ ���Ծ��� �ִ� ���")]//���� ���� �ص� �Ǵ��� ������
    public BattleUIManager.RankType maxRank = BattleUIManager.RankType.E;


    [Header("�̵� �� ���� ���� ��ũ��Ʈ")]
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
        if (isInVisible)//1. ó������ �� ������ ������ ���
        {
            gameObject.SetActive(false);
            return;
        }

        btn = GetComponent<Button>();
        shader = transform.GetChild(0).gameObject;

        scoreText = transform.GetChild(2).GetComponent<Text>();

        if (PreInfo != null) //2. ���� ���� ������ B ���� �� ���
        {
            PreInfo.ScoreSynchronize();
            if ((int)PreInfo.maxRank > 2) 
            {
                btn.interactable = false;
                shader.SetActive(false);
                return;
            }
        }   

        //3. ���� ����
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
        //�ڵ� �󿡼� ����ϴ� �̸�
        singleSelectManager.singlePanelInnerTitle = sceneInnerTitle;
        //���� ����ڿ��� �������� ���� �̸�
        singleSelectManager.singlePanelOutterTitle.text = sceneOutterTitle;
        //���� ����ڿ��� �������� ���� ����
        singleSelectManager.singlePanelDesc.text = sceneDesc;
        //���� ����
        singleSelectManager.battleUIManager.curLevel = sceneLevel;
        //enum Ÿ�� ����, JSON���� ���
        singleSelectManager.battleUIManager.singleType = singleType;
        //���� ����ڿ��� �������� ��� ����
        singleSelectManager.singleRankText[0].text = "<color=#AA00FF> S </color>: " + Sscore;
        singleSelectManager.singleRankText[1].text = "<color=#0000FF> A </color>: " + Ascore;
        singleSelectManager.singleRankText[2].text = "<color=#00AA00> B </color>: " + Bscore;
        singleSelectManager.singleRankText[3].text = "<color=#FF0000> C </color>: " + Cscore;
        singleSelectManager.singleRankText[4].text = "<color=#FFFF00> D </color>: " + Dscore;
        singleSelectManager.singleRankText[5].text = "<color=#FFFFFF> E </color>: " + Escore;

        //���� ���� ����
        singleSelectManager.battleUIManager.Sscore = Sscore;
        singleSelectManager.battleUIManager.Ascore = Ascore;
        singleSelectManager.battleUIManager.Bscore = Bscore;
        singleSelectManager.battleUIManager.Cscore = Cscore;
        singleSelectManager.battleUIManager.Dscore = Dscore;
        singleSelectManager.battleUIManager.Escore = Escore;

        //���� ȿ����
        singleSelectManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //���� ��ư ����ȭ
        singleSelectManager.btnStart.SetActive(true);
    }
}
