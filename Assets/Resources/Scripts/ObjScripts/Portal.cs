using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    LineRenderer lineRenderer;

    [Header("메인 포탈인지")]
    public bool isMainPortal;

    [Header("연결된 포탈")]
    public Transform otherPortalPos;//이동 할 포탈
    Portal otherPortal;

    [Header("워프 허가(안하면 계속 순간이동함)")]
    public bool canWarp = true;

    [Header("메인 텍스트")]
    public Text mainText;
    public string mainTitle;

    [Header("서브 텍스트")]
    public Text subText;
    public string subTitle;

    void Start()
    {
        otherPortal = otherPortalPos.GetComponent<Portal>();

        if (isMainPortal) //메인 포탈이면 선 긋기
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = true;

            // 시작점과 끝점 설정
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, otherPortal.transform.position);

            mainText.text = mainTitle;
            subText.text = subTitle;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("EnemyBullet"))
        {
            //맨 처음 입장 시, true에서 false로 전환
            canWarp = !canWarp;
            otherPortal.canWarp = !otherPortal.canWarp;

            if (!canWarp) //서브 포탈과 충돌 시, 순간이동
            {
                collision.transform.position = otherPortalPos.position;
            }
        }
    }
}
