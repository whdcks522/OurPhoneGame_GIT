using Assets.PixelHeroes.Scripts.ExampleScripts;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wind : MonoBehaviourPunCallbacks
{
    float maxTime = 5f;
    float curTime;
    
    

    WaitForSeconds wait0_1 = new WaitForSeconds(0.1f);
    Vector2 localVec = Vector2.zero;

    public enum WindType
    {
        Normal, Loop
    }
    [Header("�ٶ��� ȿ��")]
    public WindType windType;

    SpriteRenderer spriteRenderer;
    BattleUIManager battleUIManager;

    private void Update()
    {
        if (windType != WindType.Loop) 
        {
            curTime += Time.deltaTime;
            if (curTime >= maxTime)
            {
                curTime = 0;
                //���� �� ���̴� �ڷ�ƾ
                StartCoroutine(visibleWind(0));
            }
        }
    }

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [PunRPC]
    public void windOnRPC()//False: ���μż� ��ȭ�� ���
    {
        //���ӿ�����Ʈ Ȱ��ȭ
        gameObject.SetActive(true);
        //ü�� ����
        curTime = 0;
        //ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);
        //���� ���̴� �ڷ�ƾ
        StartCoroutine(visibleWind(1));
    }

    [PunRPC]
    public void windOffRPC()
    {
        gameObject.SetActive(false);
    }

    IEnumerator visibleWind(float target) //��ǥ��
    {
        bool isVisible = target == 1 ? true : false;
        float start = isVisible ? 0 : 1;

        if (isVisible) // 0 -> 1 
        {
            while (start < target) 
            {
                yield return wait0_1;
                start += 0.05f;

                localVec = new Vector2(200, 5 * start);
                transform.localScale = localVec;

                spriteRenderer.material.SetFloat("_AlphaControl", start);
                
            }
        }
        else if (!isVisible) // 1 -> 0
        {
            while (start > target)
            {
                yield return wait0_1;
                start -= 0.05f;

                localVec = new Vector2(200, 5 * start);
                transform.localScale = localVec;

                spriteRenderer.material.SetFloat("_AlphaControl", start);
            }
            windOffRPC();
        }
    }
}
