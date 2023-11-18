using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;

public class SingleSelectManager : MonoBehaviour
{
    public BattleUIManager battleUIManager;
    [Header("페이드 아웃을 구현할 이미지")]
    public GameObject fadeGameObject;
    Image fadeImage;
    [Header("로딩중 보여주는 이미지")]
    public GameObject loadGameObject;
    Text loadText;
    [Header("스크롤바 0으로 초기화를 위함")]
    public Scrollbar verticalScrollbar;

    [Header("현재 선택한 게임의 코드(개발자) 상 이름")]
    public string singlePanelInnerTitle;
    [Header("현재 선택한 게임의 사용자 상 이름")]
    public Text singlePanelOutterTitle;
    [Header("현재 선택한 게임의 설명")]
    public Text singlePanelDesc;
    [Header("현재 선택한 게임의 랭크")]
    public Text[] singleRankText;

    [Header("시작 버튼")]
    public GameObject btnStart;


    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        fadeImage = fadeGameObject.GetComponent<Image>();
        loadText = fadeGameObject.transform.GetChild(0).GetComponent<Text>();

        //배경음 재생
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmSingle.SingleSel);
    }

    public void enterSingleScene()//씬에 입장
    {
        battleUIManager.battleType = BattleUIManager.BattleType.Single;
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

        // 시작할 때 페이드 아웃 효과 실행
        StartCoroutine(StartFadeIn(singlePanelInnerTitle));
        
    }

    IEnumerator StartFadeIn(string _targetScene)
    {
        fadeGameObject.SetActive(true);
        Color color = fadeImage.color;
        float time = 0, maxTime = 1;

        while (time  < maxTime)
        {
            time += Time.deltaTime;
            float t = time / maxTime;//대기 시간

            color.a = Mathf.Lerp(0, 1, t);
            fadeImage.color = color;

            yield return null;
        }

        loadText.gameObject.SetActive(true);
        loadGameObject.SetActive(true);
        //로딩 실행

        StartCoroutine(LoadSceneAsyncCoroutine(singlePanelInnerTitle));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)//비동기적으로 scene 로드(렉 걸릴때 사용)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 로딩이 끝나도 바로 활성화하지 않음

        // 로딩이 끝날 때까지 대기
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9는 로딩이 끝났을 때의 값

            loadText.text = "로딩 중: " + Mathf.Floor(progress * 100) + "%";
            if (progress >= 1f)
            {
                asyncLoad.allowSceneActivation = true; // 활성화
            }

            yield return null;
        }
    }
}
