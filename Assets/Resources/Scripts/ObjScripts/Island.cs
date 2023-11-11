using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Island : MonoBehaviour
{
    public float speed; // 마찰력 계수
    private Rigidbody2D rigidbody;
    [Header("진행 방향")]
    public Vector3 dirVec;


    public enum IslandType
    {
        Conveyor
    }
    [Header("섬 타입")]
    public IslandType islandType;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        dirVec = dirVec.normalized;
    }


    private void FixedUpdate()
    {
        if (IslandType.Conveyor == islandType) 
        {
            Debug.Log("Move");
            rigidbody.AddForce(dirVec * speed * 1000);
        }
    }
}
