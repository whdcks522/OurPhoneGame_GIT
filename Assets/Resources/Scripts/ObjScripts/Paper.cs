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
        //귀찮아서 비활성화
        gameObject.SetActive(false);
    }

    public void OnClick() //버튼 클릭
    {
        hostBox.ControlAdavancedBox(false);
    }
}
