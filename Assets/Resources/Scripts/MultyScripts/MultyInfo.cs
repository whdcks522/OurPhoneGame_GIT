using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MultyInfo : MonoBehaviour
{
    public Sprite icon;

    public LobbyManager lobbyManager;

    [Header("�̵� �� �� ����")]
    [TextArea]
    public string sceneDesc;

    [Header("�̵� �� �� ���� �̸�")]
    public string sceneInnerTitle;

    [Header("�̵� �� �� �ܺ� �̸�")]
    public string sceneOutterTitle;

    [Header("�̵� �� ���� ������ ��")]
    public int sceneMax;

    //���� ��ư�� �̹���
    Sprite btnSprite;
    //���� ��ư�� ���͸���
    Material btnMaterial;

    private void Awake()
    {
        btnMaterial = transform.GetChild(0).GetComponent<Image>().material;
        btnSprite = transform.GetChild(1).GetComponent<Image>().sprite;
    }

    public void Onclick()
    {
        //�����ڿ��� �������� ���� �̸�
        lobbyManager.SceneInnerStr = sceneInnerTitle;
        //���� ����ڿ��� �������� ���� �̸�
        lobbyManager.SceneOutterText.text = sceneOutterTitle;
        //���� ����ڿ��� �������� ���� �����ڼ� ����
        lobbyManager.SceneMaxText.text = sceneMax+"��";
        lobbyManager.maxPlayerNumber = sceneMax;
        //���� ����ڿ��� �������� ���� ����
        lobbyManager.SceneDescText.text = sceneDesc;

        //���͸��� ����
        lobbyManager.cellMaterial = btnMaterial;
        //�̹��� ����
        lobbyManager.cellSprite = btnSprite;
        //���� ȿ����
        lobbyManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //���� ��ư ����ȭ
        lobbyManager.CreateRoomBtn.SetActive(true);
    }
}
