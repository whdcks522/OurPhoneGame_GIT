using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SingleInfoData", menuName = "Scriptable Ojbect/SingleInfo")]
public class SingleInfoData : ScriptableObject//스크립타블 오브젝트 상속
{
    public enum SingleInfoType 
    {
        Train, StarFall, Block, Fly, Dog
    }

    [Header("싱글 게임 타입")]
    public SingleInfoType singleType;

    [Header("싱글 게임 쉐이더")]
    public Material shader;
    [Header("싱글 게임 아이콘")]
    public Sprite icon;

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
}
