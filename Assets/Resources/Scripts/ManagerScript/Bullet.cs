using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxTime;
    float curTime = 0f;
    public float bulletSpeed;

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            curTime = 0f;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("playerSword"))
        {
            Destroy(gameObject);
        }
    }
}
