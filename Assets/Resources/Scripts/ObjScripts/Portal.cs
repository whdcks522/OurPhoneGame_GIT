using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Portal : MonoBehaviour
{
    LineRenderer lineRenderer;
    public Transform otherPortal;
    public bool isMainPortal;
    void Start()
    {
        if (isMainPortal) 
        {
            lineRenderer = GetComponent<LineRenderer>();

            // 시작점과 끝점 설정
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, otherPortal.transform.position);
        }
    }
}
