using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    
    public Transform parent;
    public int followDelay;//���󰡴� �����ð�

    public Queue<Vector3> parentVecQueue;
    private Vector3 followVec;

    private void Awake()
    {
        parentVecQueue = new Queue<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        //�÷��̾ ���߸� ����,Input Pos
        //if (!parentPos.Contains(parent.position))
        parentVecQueue.Enqueue(parent.position);

        //Output Pos
        if (parentVecQueue.Count > followDelay) followVec = parentVecQueue.Dequeue();
        //��á�ٸ�
        else if (parentVecQueue.Count < followDelay) followVec = parent.position;

 
        transform.position = followVec;
    }
}
