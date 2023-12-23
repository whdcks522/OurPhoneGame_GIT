using Assets.PixelHeroes.Scripts.ExampleScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{
    public Box hostBox;
    BattleUIManager battleUIManager;

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        //�����Ƽ� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }

    public void OnClick() //��ư Ŭ��
    {
        hostBox.ControlAdavancedBox(false);
    }
}
