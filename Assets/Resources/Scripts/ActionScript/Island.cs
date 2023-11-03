using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public float speed; // ������ ���
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector2.zero; // �ʱ� �ӵ��� 0���� �����Ͽ� �������� �ʰ� �մϴ�.
    }

    private void Update()
    {
        rb.AddForce(Vector2.right * speed);
    }
}
