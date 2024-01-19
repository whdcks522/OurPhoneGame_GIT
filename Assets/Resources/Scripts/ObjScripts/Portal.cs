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
    public Transform otherPortal;//포탈 라인 긋기
    public Transform otherExit;//실제 이동하는 위치

    [Header("메인 텍스트")]
    public Text mainText;
    public string mainTitle;

    [Header("서브 텍스트")]
    public Text subText;
    public string subTitle;

    void Start()
    {
        if (isMainPortal) //메인 포탈이면 선 긋기
        {
            lineRenderer = GetComponent<LineRenderer>();

            // 시작점과 끝점 설정
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, otherPortal.transform.position);

            mainText.text = mainTitle;
            subText.text = subTitle;
        }
    }

    public bool canUseMainPortal = true;
    public bool canUseSubPortal = true;


    public bool canWarp = true;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (isMainPortal)//메인 포탈에서 나온 경우
            {
                canUseSubPortal = true;
            }
            else if (!isMainPortal)//서브 포탈에서 나온 경우
            {
                canUseMainPortal = true;
            }
        }
    }
}
