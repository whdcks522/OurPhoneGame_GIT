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
    [Header("바람의 효과")]
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
                //점차 안 보이는 코루틴
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
    public void windOnRPC()//False: 못부셔셔 진화한 경우
    {
        //게임오브젝트 활성화
        gameObject.SetActive(true);
        //체력 관리
        curTime = 0;
        //효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Summon);
        //점차 보이는 코루틴
        StartCoroutine(visibleWind(1));
    }

    [PunRPC]
    public void windOffRPC()
    {
        gameObject.SetActive(false);
    }

    IEnumerator visibleWind(float target) //목표값
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
