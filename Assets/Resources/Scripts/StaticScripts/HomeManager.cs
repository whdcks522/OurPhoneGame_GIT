using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    public GameObject multyEnterBtn;
    BattleUIManager battleUIManager;
    private void Start()
    {
        battleUIManager = BattleUIManager.Instance;
        battleUIManager.battleType = BattleUIManager.BattleType.Rest;
    }

    bool isOnline = false;
    private void LateUpdate()//���ͳ��� �۵��ϴ��� Ȯ��
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // ���ͳ� ������ �ȵǾ�����
            isOnline = false;
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // �����ͷ� ���ͳ� ������ �Ǿ�����
            isOnline = true;
        }
        else
        {
            // �������̷� ������ �Ǿ�����
            isOnline = true;
        }
        if (isOnline != multyEnterBtn.activeSelf)//��Ƽ ��ư ��Ȱ��ȭ
            multyEnterBtn.SetActive(isOnline);
    }


    public void loadSingleSel()//�̱� ������ �̵�
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        battleUIManager.battleType = BattleUIManager.BattleType.Single;

        SceneManager.LoadScene("SingleSelect");
    }

    public void loadSingleMul()//��Ƽ ������ �̵�
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        battleUIManager.battleType = BattleUIManager.BattleType.Multy;

        SceneManager.LoadScene("Lobby");
    }

    public void loadSettings()//���� ������ �̵�
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //�� ��ȯ
        SceneManager.LoadScene("Settings");
    }

    public void JsonControl(int _value)
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //�� ��ȯ
        battleUIManager.jsonManager.DataClear(_value);
    }
}
