using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    
    public Transform parent;
    public int followDelay;//따라가는 지연시간

    public Queue<Vector3> parentVecQueue;
    private Vector3 followVec;

    private void Awake()
    {
        parentVecQueue = new Queue<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        //플레이어가 멈추면 정지,Input Pos
        //if (!parentPos.Contains(parent.position))
        parentVecQueue.Enqueue(parent.position);

        //Output Pos
        if (parentVecQueue.Count > followDelay) followVec = parentVecQueue.Dequeue();
        //안찼다면
        else if (parentVecQueue.Count < followDelay) followVec = parent.position;

 
        transform.position = followVec;
    }
}
