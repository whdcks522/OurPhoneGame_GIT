using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    [Header("�̵� ������Ʈ")]
    public AreaEffector2D playerEffector2D;
    public AreaEffector2D blockEffector2D;

    [Header("�߰� �ӵ�")]
    public int playerValue;
    public int blockValue;

    private void Awake()
    {
        playerEffector2D.forceMagnitude *= playerValue;
        blockEffector2D.forceMagnitude *= blockValue;
    }
}
