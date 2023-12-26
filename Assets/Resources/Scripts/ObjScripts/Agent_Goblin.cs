using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Agent_Goblin : MonoBehaviour
{
    public Enemy enemy;
    public BulletShotter bulletShotter;
    public GameObject player;
    public CharacterControls characterControls;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        bulletShotter = GetComponent<BulletShotter>();
        player = enemy.player;
        characterControls = enemy.characterControls;

        bulletShotter.gameManager = enemy.gameManager;
    }

    void Update()
    {
        if (enemy.maxTime <= enemy.curTime) 
        {
            enemy.reloadRPC();
            //float randomValue = Random.Range(-1f, 1f);

            tripleCor = StartCoroutine(tripleShot());
        }
    }

    private void OnDisable()
    {
        if(tripleCor != null)
        StopCoroutine(tripleCor);
    }


    Coroutine tripleCor;
    WaitForSeconds wait0_25 = new WaitForSeconds(0.25f);
    IEnumerator tripleShot() 
    {
        //고개 전환
        int dir = 1;
        if (player.transform.position.x < transform.position.x)//플레이어가 왼쪽에 있는 경우
            dir = -1;
        enemy.TurnRPC(dir);
        //저격 애니메이션 재생
        characterControls.Character.Animator.SetTrigger("Shot");

        //저격
        yield return wait0_25;
        yield return wait0_25;
        bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);

        yield return wait0_25;
        bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);

        yield return wait0_25;
        bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);
    }
}
