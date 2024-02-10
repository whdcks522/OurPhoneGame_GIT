using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    [Header("이동 컴포넌트")]
    public AreaEffector2D playerEffector2D;
    public AreaEffector2D blockEffector2D;

    [Header("추가 속도")]
    public int playerValue;
    public int blockValue;

    private void Awake()
    {
        playerEffector2D.forceMagnitude *= playerValue;
        blockEffector2D.forceMagnitude *= blockValue;
    }
}
