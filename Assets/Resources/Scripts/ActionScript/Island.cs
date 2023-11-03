using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public float speed; // 마찰력 계수
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector2.zero; // 초기 속도를 0으로 설정하여 움직이지 않게 합니다.
    }

    private void Update()
    {
        rb.AddForce(Vector2.right * speed);
    }
}
