using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class StarFallManager : MonoBehaviour
{
    public GameObject player;
    public GameObject star;
    public Transform[] starFallPoints;
    int maxIndex;
    

    float maxTime = 0.5f;
    float curTime = 0f;
    int recentPos;

    private void Awake()
    {
        maxIndex = starFallPoints.Length;
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (curTime > maxTime)
        {
            curTime = 0f;

            GameObject bullet = Instantiate(star);
            int ranPos = Random.Range(0, maxIndex);

            if (ranPos == recentPos) //���� �� �������� �ȵǵ��� ����
                ranPos = (ranPos + 1) / maxIndex;
            
            recentPos = ranPos;

            bullet.transform.position = starFallPoints[ranPos].position;
            Rigidbody bulletRigid = bullet.GetComponent<Rigidbody>();
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            Vector2 bulletVec = (player.transform.position - bullet.transform.position).normalized;

            bulletVec += 0.1f * Random.insideUnitCircle;//�������� 1�� �� �ȿ��� ���� ����2 ��ǥ �����
            bulletVec = bulletVec.normalized;

            bulletRigid.velocity = bulletVec * bulletComponent.bulletSpeed;

            float zValue = Mathf.Atan2(bulletRigid.velocity.x, bulletRigid.velocity.y) * 180 / Mathf.PI;
            Vector3 rotVec = Vector3.back * zValue + Vector3.back * 45.0f;
            bullet.transform.Rotate(rotVec);
            
        }
    }
}
