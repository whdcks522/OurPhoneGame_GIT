using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("���� ��ư�� �̹���")]
    public Sprite btnImage;

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
        //�̹��� ����
        lobbyManager.cellImage = btnImage;
        //���� ȿ����
        lobbyManager.battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
        //���� ��ư ����ȭ
        lobbyManager.CreateRoomBtn.SetActive(true);
    }
}
