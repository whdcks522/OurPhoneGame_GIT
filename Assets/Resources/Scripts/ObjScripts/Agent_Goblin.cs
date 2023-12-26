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
        //�� ��ȯ
        int dir = 1;
        if (player.transform.position.x < transform.position.x)//�÷��̾ ���ʿ� �ִ� ���
            dir = -1;
        enemy.TurnRPC(dir);
        //���� �ִϸ��̼� ���
        characterControls.Character.Animator.SetTrigger("Shot");

        //����
        yield return wait0_25;
        yield return wait0_25;
        bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);

        yield return wait0_25;
        bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);

        yield return wait0_25;
        bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);
    }
}
