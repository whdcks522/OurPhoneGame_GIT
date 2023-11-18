using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    AreaEffector2D areaEffector2D;
    public enum IslandType 
    {
        ConveyorBelt
    }
    public IslandType islandType;


    private void Awake()
    {
        areaEffector2D = GetComponent<AreaEffector2D>();
        areaEffector2D.forceMagnitude *= 300;
    }
}
