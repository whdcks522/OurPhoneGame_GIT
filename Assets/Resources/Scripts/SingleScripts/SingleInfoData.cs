using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleInfoData", menuName = "Scriptable Ojbect/SingleInfo")]
public class SingleInfoData : ScriptableObject//��ũ��Ÿ�� ������Ʈ ���
{
    public enum SingleInfoType 
    {
        Train, StarFall, Block, Fly, Dog
    }

    [Header("�̱� ���� Ÿ��")]
    public SingleInfoType singleType;

    [Header("�̱� ���� ���̴�")]
    public Material shader;
    [Header("�̱� ���� ������")]
    public Sprite icon;

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
}
