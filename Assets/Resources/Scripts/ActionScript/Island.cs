using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public float speed; // ������ ���
    private Rigidbody rigidbody;

    public enum IslandType
    {
        Conveyor
    }
    public IslandType islandType;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector2.zero; // �ʱ� �ӵ��� 0���� �����Ͽ� �������� �ʰ� �մϴ�.
    }


    private void FixedUpdate()
    {
        if (IslandType.Conveyor == islandType) 
        {
            rigidbody.AddForce(Vector2.right * speed);
        }
    }
}
