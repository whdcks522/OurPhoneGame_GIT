using System.Collections;
using System.Collections.Generic;
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

    [Header("�̵� �� �� ��ǥ ������")]
    public int Sscore;
    public int Ascore;
    public int Bscore;
    public int Cscore;
    public int Dscore;
    public int Escore;

    public void Onclick() 
    {
        //�ڵ� �󿡼� ����ϴ� �̸�
        singleSelectManager.singlePanelInnerTitle = sceneInnerTitle;
        //���� ����ڿ��� �������� ���� �̸�
        singleSelectManager.singlePanelOutterTitle.text = sceneOutterTitle;
        //���� ����ڿ��� �������� ���� ����
        singleSelectManager.singlePanelDesc.text = sceneDesc;
        //���� ����ڿ��� �������� ��� ����
        singleSelectManager.singleRankText.text = "S: " + Sscore + " A: " + Ascore + '\n' +
                                                    "B: " + Bscore + " C: " + Cscore + '\n' +
                                                      "D: " + Dscore + " E: " + Escore;

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
