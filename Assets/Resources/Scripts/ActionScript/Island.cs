using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public float speed; // 마찰력 계수
    private Rigidbody rigidbody;

    public enum IslandType
    {
        Conveyor
    }
    public IslandType islandType;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector2.zero; // 초기 속도를 0으로 설정하여 움직이지 않게 합니다.
    }


    private void FixedUpdate()
    {
        if (IslandType.Conveyor == islandType) 
        {
            rigidbody.AddForce(Vector2.right * speed);
        }
    }
}
