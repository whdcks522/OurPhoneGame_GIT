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

    Vector2 rigidVec;
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
        //�ӵ� �ʱ�ȭ
        enemy.xyRPC(0, 0);

        if (enemy.maxTime <= enemy.curTime && !enemy.isML) 
        {
            //����
            enemy.reloadRPC(1f, "Shot");

            //���� ��ȯ
            int dir = 1;
            if (player.transform.position.x < transform.position.x)//�÷��̾ ���ʿ� �ִ� ���
                dir = -1;
            enemy.TurnRPC(dir);

            //���� ���
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
        //����
        yield return wait;

        for (int i = 0; i < 2; i++) //2�� ���
        {
            yield return wait;
            audioManager.PlaySfx(AudioManager.Sfx.Arrow);
            bulletShotter.sortShot(BulletShotter.BulletShotType.Direction, Bullet.BulletEffectType.UnBreakable, gameObject, player, 0);
        } 
    }
}
