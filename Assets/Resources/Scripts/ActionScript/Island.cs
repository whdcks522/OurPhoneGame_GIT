using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public float speed; // ������ ���
    private Rigidbody rigidbody;
    [Header("���� ����")]
    public Vector3 dirVec;


    public enum IslandType
    {
        Conveyor
    }
    [Header("�� Ÿ��")]
    public IslandType islandType;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        dirVec = dirVec.normalized;
        //rigidbody.velocity = Vector2.zero; // �ʱ� �ӵ��� 0���� �����Ͽ� �������� �ʰ� �մϴ�.
    }


    private void FixedUpdate()
    {
        if (IslandType.Conveyor == islandType) 
        {
            rigidbody.AddForce(dirVec * speed * 1000);
        }
    }
}
