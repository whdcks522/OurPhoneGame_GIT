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
    AudioManager audioManager;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        bulletShotter = GetComponent<BulletShotter>();
        player = enemy.player;
        characterControls = enemy.characterControls;

        bulletShotter.gameManager = enemy.gameManager;

        audioManager = enemy.battleUIManager.audioManager;
    }

    void Update()
    {
        if (enemy.maxTime <= enemy.curTime) 
        {
            enemy.reloadRPC(1f);

            tripleCor = StartCoroutine(tripleShot());
        }
    }

    private void OnDisable()
    {
        if(tripleCor != null)
        StopCoroutine(tripleCor);
    }


    Coroutine tripleCor;
    WaitForSeconds wait0_15 = new WaitForSeconds(0.15f);
    IEnumerator tripleShot() 
    {
        //�� ��ȯ
        int dir = 1;
        if (player.transform.position.x < transform.position.x)//�÷��̾ ���ʿ� �ִ� ���
            dir = -1;
        enemy.TurnRPC(dir);
        //���� �ִϸ��̼� ���
        

        

        //����
        yield return wait0_15;

        for (int i = 0; i < 3; i++) 
        {
            yield return wait0_15;
            audioManager.PlaySfx(AudioManager.Sfx.Arrow);
            bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);
        } 
    }
}
