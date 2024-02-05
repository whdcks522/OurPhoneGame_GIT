using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class Portal : MonoBehaviour
{
    LineRenderer lineRenderer;

    [Header("���� ��Ż����")]
    public bool isMainPortal;

    [Header("����� ��Ż")]
    public Transform otherPortalPos;//�̵� �� ��Ż
    Portal otherPortal;

    [Header("���� �㰡(���ϸ� ��� �����̵���)")]
    public bool canWarp = true;

    [Header("���� �ؽ�Ʈ")]
    public Text mainText;
    public string mainTitle;

    [Header("���� �ؽ�Ʈ")]
    public Text subText;
    public string subTitle;

    void Start()
    {
        otherPortal = otherPortalPos.GetComponent<Portal>();

        if (isMainPortal) //���� ��Ż�̸� �� �߱�
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = true;

            // �������� ���� ����
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
            //�� ó�� ���� ��, true���� false�� ��ȯ
            canWarp = !canWarp;
            otherPortal.canWarp = !otherPortal.canWarp;

            if (!canWarp) //���� ��Ż�� �浹 ��, �����̵�
            {
                collision.transform.position = otherPortalPos.position;
            }
        }
    }
}
