using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("�� �ε� ���̵�ƿ�")]
    public GameObject loadFadeOut;
    Image loadFadeOutImage;
    [Header("�� �ε� �ؽ�Ʈ")]
    public Text loadText;
    [Header("�� �ε� ���� ������Ʈ")]
    public GameObject loadGameObject;
    WaitForSeconds wait0_35 = new WaitForSeconds(0.35f);
    //�κ� ������
    bool isJoinedLobby = false;

    public Sprite cellSprite;

    

    [Header("�� ������ �̸�")]
    public string SceneInnerStr;
    [Header("�� ����� �̸� �ؽ�Ʈ")]
    public Text SceneOutterText;
    [Header("�� �ִ� �ο� �ؽ�Ʈ")]
    public Text SceneMaxText;
    [Header("�� ���� �ؽ�Ʈ")]
    public Text SceneDescText;

    [Header("�÷��̾� �̸� ���� �ؽ�Ʈ")]
    public Text NameText;
    [Header("������ �� �ؽ�Ʈ")]
    public Text CountText;
    [Header("���� ���� �ؽ�Ʈ")]
    public Text NetworkText;
    [Header("���� �ؽ�Ʈ")]
    public Text ErrorText;

    [Header("�� ���� ��ư")]
    public GameObject CreateRoomBtn;
    [Header("�� ��ư��")]
    public Button[] CellBtn;
    [Header("���� �� �ε�")]
    public Button PreviousBtn;
    [Header("���� �� �ε�")]
    public Button NextBtn;

    [Header("������ ���� �̸�")]
    public InputField RoomInput;
    public int maxPlayerNumber;
    List<RoomInfo> myList = new List<RoomInfo>();

    int currentPage = 1, maxPage, multiple;

    public BattleUIManager battleUIManager;
    private readonly string gameVersion = "1";

    private void Awake()
    {
        battleUIManager = BattleUIManager.Instance;
        loadFadeOutImage = loadFadeOut.GetComponent<Image>();
    }

    private void Start()
    {
        Connect();

        //�����
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmMulty.Lobby);
        //�ؽ�Ʈ �ڵ� ����
        StartCoroutine(loadTextSwap());
    }

    #region �κ� ����
    public void Connect()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();//AuthManager���� �̹� �Ἥ �ʿ� ����

    public override void OnJoinedLobby()//�κ� ������ �г��� ����
    {
        //if (AuthManager.Instance.User != null)
        // PhotonNetwork.LocalPlayer.NickName = AuthManager.Instance.playerEmail;
        //else
        PhotonNetwork.LocalPlayer.NickName = "NickName" + Random.Range(0, 10000);//NickNameInput.text


        NameText.text = PhotonNetwork.LocalPlayer.NickName;

        //����Ʈ ����
        myList.Clear();

        isJoinedLobby = true;
    }
    #endregion 

    #region ���۽� ���̵� �ƿ�
    IEnumerator loadTextSwap()//�ε� �� �ؽ�Ʈ
    {
        loadText.text = "�κ� ������.";
        yield return wait0_35;

        loadText.text = "�κ� ������..";
        yield return wait0_35;

        loadText.text = "�κ� ������...";
        yield return wait0_35;

        if (isJoinedLobby)//�ε� �Ϸ�
            StartCoroutine(StartFadeOut());
        else//�ε� ��
            StartCoroutine(loadTextSwap());
    }

    IEnumerator StartFadeOut()//���̵� �ƿ�
    {
        //�ؽ�Ʈ�� �� ��Ȱ��ȭ
        loadText.gameObject.SetActive(false);
        loadGameObject.SetActive(false);

        Color color = loadFadeOutImage.color;
        float time = 1, minTime = 0;

        while (time > minTime)
        {
            time -= Time.deltaTime;
            float t = time / 1;//��� �ð�

            color.a = Mathf.Lerp(0, 1, t);
            loadFadeOutImage.color = color;

            yield return null;
        }

        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        loadFadeOut.SetActive(false);
    }
    #endregion 

    
    void Update()
    {
        //��Ʈ��ũ ���� ����
        NetworkText.text = PhotonNetwork.NetworkClientState.ToString();
        //��Ʈ��ũ ��ü���� �뿡 �ִ� �ο� ���� �κ� �ִ� �ο� �� ����
        CountText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "�� �κ� / "
            + PhotonNetwork.CountOfPlayersInRooms + "�� �� / "
            + PhotonNetwork.CountOfPlayers + "�� ���� ��";
    }

    #region �������� ��
    public void Disconnect() 
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //�� ��ȯ
        SceneManager.LoadScene("Home");
    }
    #endregion

    #region �� ����Ʈ ����
    // ����ư -2 , ����ư -1 , �� ����
    public void MyListClick(int num)
    {

        if (num == -2)//���� ȭ��ǥ
        {
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
            --currentPage;
        }
        else if (num == -1)//������ ȭ��ǥ
        {
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
            ++currentPage;
        }
        else //���� ��ư
        {
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        }

        MyListRenewal();
    }
    void MyListRenewal()
    {
        // �ִ� ������
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // ����, ������ư Ȱ��ȭ, ��Ȱ��ȭ
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
        multiple = (currentPage - 1) * CellBtn.Length;//�� �������� ù ��° ���� �ε���
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            //�� �̸�
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text =
                (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            //�÷��̾� ��
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text =
                (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
            //�̹���
            Image cImage = CellBtn[i].transform.GetChild(3).GetComponent<Image>();
            if (multiple + i < myList.Count && myList[multiple + i].CustomProperties.ContainsKey("RoomImage"))
            {
                string imagePath = (string)myList[multiple + i].CustomProperties["RoomImage"];

                // imagePath�� ����Ͽ� �̹����� �ε��ϰų� �ٸ� ����� ����Ͽ� �̹����� �����ϼ���.
                // ���� ��� Resources.Load�� ����� �� �ֽ��ϴ�.
                Sprite roomSprite = Resources.Load<Sprite>(imagePath);

                if (roomSprite != null)
                {
                    cImage.sprite = roomSprite;
                }
                else
                {
                    Debug.LogError("Failed to load image: " + imagePath);
                }
            }
            else
            {
                // �̹����� ���� ��� �⺻ �̹����� �����ϰų� �ƹ� �۾��� ���� �ʽ��ϴ�.
                cImage.sprite = null;
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)//�������� �ڵ����� ���� ����� �������� �װ��ΰ� ��
    {
        //�Ʒ��� �κ� ������ ������ ����Ʈ �ʱ�ȭ�ؼ� ������
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)//���� �����ϴ� ���̶��
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]); //�ٵ� �����ϰ� ���� �ʴٸ�, ���Ѵ�
                else myList[myList.IndexOf(roomList[i])] = roomList[i];     //���� ���� �ε����� ������ ����ȭ(�ο��� �ٲ��� ����ȭ)  
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion


    #region ��
    public void CreateRoom() //�� ����
    {
        //���� ���
        //PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 2 });//������

        string sceneName = SceneInnerStr;
        string roomName = "_Room_" + PhotonNetwork.LocalPlayer.NickName;

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayerNumber,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "IsAllowedToEnter", true },
                { "IsAllowedToExit", true },
                { "SceneName", sceneName },
                { "RoomImage", cellSprite.name } // ������ �̹����� ��θ� ����
            },
            CustomRoomPropertiesForLobby = new string[] { "IsAllowedToEnter", "IsAllowedToExit", "SceneName", "RoomImage" } // �κ񿡼��� �� �Ӽ��� �����ֱ� ���� �߰�
        };


        PhotonNetwork.CreateRoom(sceneName + roomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        //���� ȿ����
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

        //���� ���
        //PhotonNetwork.LoadLevel("Chap1_Scene");

        // �� ���� ���� RoomOptions���� ������ bool ������ üũ�Ͽ� ���ǿ� ���� ���� ��� ���θ� Ȯ��
        bool canEnterRoom = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsAllowedToEnter"];
        if (canEnterRoom)
        {
            //�� ���� Scene �̸� Ȯ��
            string sceneName = (string)PhotonNetwork.CurrentRoom.CustomProperties["SceneName"];
            //������ ����
            PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {
            ErrorText.text = "�̹� ������ ���� ���̹Ƿ�, �뿡 ���� �� �� �����ϴ�.";
            PhotonNetwork.LeaveRoom();
        }
    }

    
    public void JoinRandomRoom()//���� �� ����
    {
        //AuthManager.Instance.audioManager.PlaySfx(AudioManager.Sfx.DoorOpen, true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //���� �̸��� ���� ����� ����
    {
        ErrorText.text = "���� ����";
        //RoomInput.text = ""; CreateRoom(); 
    }
    public override void OnJoinRandomFailed(short returnCode, string message) //���� �̸��� ���� ����� ����
    {
        ErrorText.text = "���� ����";
        //RoomInput.text = "";
        //CreateRoom();


    }

    //PhotonNetwork.PlayerList[]:�迭�� �ϳ� �ϳ� ����
    //PhotonNetwork.CurrentRoom.Name: ���� �� �̸�
    //PhotonNetwork.CurrentRoom.PlayerCount: �濡 �ִ� ��� ��
    //PhotonNetwork.CurrentRoom.MaxPlayers: �� �ִ� ��� ��
    #endregion
}

