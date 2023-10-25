using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class StarFall : MonoBehaviour
{
    public GameObject player;
    public GameObject star;
    public Transform[] starFallPoints;
    

    float maxTime = 0.5f;
    float curTime = 0f;
    int recentPos;
    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            curTime = 0f;

            GameObject bullet = Instantiate(star);
            int ranPos = Random.Range(0, starFallPoints.Length);
            if (ranPos == recentPos) 
            {
            
            }
            recentPos = ranPos;
            bullet.transform.position = starFallPoints[ranPos].position;
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            bulletRigid.velocity = (player.transform.position - bullet.transform.position).normalized * bulletComponent.bulletSpeed;

            float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
            bullet.transform.Rotate(rotVec);
            
        }
    }
}
