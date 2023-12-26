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
            //장전
            enemy.reloadRPC(1f, "Shot");

            //방향 전환
            int dir = 1;
            if (player.transform.position.x < transform.position.x)//플레이어가 왼쪽에 있는 경우
                dir = -1;
            enemy.TurnRPC(dir);

            //실제 사격
            tripleCor = StartCoroutine(tripleShot());
        }
    }

    private void OnDisable()
    {
        if(tripleCor != null)
        StopCoroutine(tripleCor);
    }

    Coroutine tripleCor;
    WaitForSeconds wait = new WaitForSeconds(0.12f);
    IEnumerator tripleShot() 
    {
        //저격
        yield return wait;

        for (int i = 0; i < 3; i++) 
        {
            yield return wait;
            audioManager.PlaySfx(AudioManager.Sfx.Arrow);
            bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);
        } 
    }
}
