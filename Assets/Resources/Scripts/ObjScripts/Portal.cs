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
    public Transform otherPortal;//��Ż ���� �߱�
    public Transform otherExit;//���� �̵��ϴ� ��ġ

    [Header("���� �ؽ�Ʈ")]
    public Text mainText;
    public string mainTitle;

    [Header("���� �ؽ�Ʈ")]
    public Text subText;
    public string subTitle;

    void Start()
    {
        if (isMainPortal) //���� ��Ż�̸� �� �߱�
        {
            lineRenderer = GetComponent<LineRenderer>();

            // �������� ���� ����
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
            if (isMainPortal)//���� ��Ż���� ���� ���
            {
                canUseSubPortal = true;
            }
            else if (!isMainPortal)//���� ��Ż���� ���� ���
            {
                canUseMainPortal = true;
            }
        }
    }
}
