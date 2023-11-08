using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SingleInfo : MonoBehaviour
{
    public SingleSelectManager singleSelectManager;

    [Header("�̵� �� �� ���� �̸�")]
    public string sceneInnerTitle;
    [Header("�̵� �� �� �ܺ� �̸�")]
    public string sceneOutterTitle;
    [Header("�̵� �� �� ����")]
    [TextArea]
    public string sceneDesc;

    public bool isVisible = true;

    [Header("�̵� �� ���� ���� ��ũ��Ʈ")]
    public SingleInfo PreInfo;
    [Header("�̵� �� �� �ִ� ����")]
    public int Maxscore;
    
    [Header("�̵� �� �� ��ǥ ������")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;

    void Start() 
    {
        if (!isVisible) 
            gameObject.SetActive(false);
    }
    public void Onclick() 
    {
        //�ڵ� �󿡼� ����ϴ� �̸�
        singleSelectManager.singlePanelInnerTitle = sceneInnerTitle;
        //���� ����ڿ��� �������� ���� �̸�
        singleSelectManager.singlePanelOutterTitle.text = sceneOutterTitle;
        //���� ����ڿ��� �������� ���� ����
        singleSelectManager.singlePanelDesc.text = sceneDesc;
        //���� ����ڿ��� �������� ��� ����
        singleSelectManager.singleRankText[0].text = "<color=red> S </color>: " + Sscore;
        singleSelectManager.singleRankText[1].text = "<color=#FFAA00> A </color>: " + Ascore;
        singleSelectManager.singleRankText[2].text = "<color=#FFFF00> B </color>: " + Bscore;
        singleSelectManager.singleRankText[3].text = "<color=#00AA00> C </color>: " + Cscore;
        singleSelectManager.singleRankText[4].text = "<color=#00AAFF> D </color>: " + Dscore;
        singleSelectManager.singleRankText[5].text = "<color=#AA00FF> E </color>: " + Escore;


        //���� ���� ����
        singleSelectManager.battleUIManager.Sscore = Sscore;
        singleSelectManager.battleUIManager.Ascore = Ascore;
        singleSelectManager.battleUIManager.Bscore = Bscore;
        singleSelectManager.battleUIManager.Cscore = Cscore;
        singleSelectManager.battleUIManager.Dscore = Dscore;
        singleSelectManager.battleUIManager.Escore = Escore;

        //���� ȿ����
        singleSelectManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
    }
}
