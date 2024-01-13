using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "TipData", menuName = "Scriptable Ojbect/TipData")]

public class TipData : ScriptableObject
{
    public string[] tipArr;

    public string returnTip()
    {
        int r = Random.Range(0, tipArr.Length);
        string str = "Tip" + r + ". " + tipArr[r];
        return str;
    }
}
