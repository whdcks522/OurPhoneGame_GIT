using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingleSelectManager : MonoBehaviour
{
    public BattleUIManager battleUIManager;
    [Header("���̵� �ƿ��� ������ �̹���")]
    public GameObject fadeGameObject;
    Image fadeImage;
    [Header("�ε��� �����ִ� �̹���")]
    public GameObject loadGameObject;
    Text loadText;
    [Header("��ũ�ѹ� 0���� �ʱ�ȭ�� ����")]
    public Scrollbar verticalScrollbar;

    public string singlePanelInnerTitle;
    public Text singlePanelOutterTitle;
    public Text singlePanelDesc;
    public Text singleRankText;


    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        fadeImage = fadeGameObject.GetComponent<Image>();
        loadText = fadeGameObject.transform.GetChild(0).GetComponent<Text>();

        verticalScrollbar.value = 1;
    }

    public void enterSingleScene() 
    {
        battleUIManager.battleType = BattleUIManager.BattleType.Single;
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

        // ������ �� ���̵� �ƿ� ȿ�� ����
        StartCoroutine(StartFadeOut(singlePanelInnerTitle));
    }

    IEnumerator StartFadeOut(string _targetScene)
    {
        fadeGameObject.SetActive(true);
        Color color = fadeImage.color;
        float time = 0, maxTime = 1;

        while (time  < maxTime)
        {
            time += Time.deltaTime;
            float t = time / maxTime;//��� �ð�

            color.a = Mathf.Lerp(0, 1, t);
            fadeImage.color = color;

            yield return null;
        }

        loadGameObject.SetActive(true);

        //yield return new WaitForSeconds(0.2f);

        //SceneManager.LoadScene(_targetScene);
        StartCoroutine(LoadSceneAsyncCoroutine(_targetScene));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)//�񵿱������� scene �ε�(�� �ɸ��� ���)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // �ε��� ������ �ٷ� Ȱ��ȭ���� ����
        loadText.gameObject.SetActive(true);

        // �ε��� ���� ������ ���
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // 0.9�� �ε��� ������ ���� ��

            loadText.text = "�ε� ��: " + (progress * 100) + "%";
            if (progress >= 1f)
            {
                asyncLoad.allowSceneActivation = true; // Ȱ��ȭ
            }

            yield return null;
        }
    }
}
