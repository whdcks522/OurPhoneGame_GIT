using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("씬 로드 페이드아웃")]
    public GameObject loadFadeOut;
    Image loadFadeOutImage;
    [Header("씬 로드 텍스트")]
    public Text loadText;
    [Header("씬 로드 게임 오브젝트")]
    public GameObject loadGameObject;
    WaitForSeconds wait0_35 = new WaitForSeconds(0.35f);
    //로비에 입장함
    bool isJoinedLobby = false;

    public Sprite cellSprite;

    

    [Header("씬 개발자 이름")]
    public string SceneInnerStr;
    [Header("씬 사용자 이름 텍스트")]
    public Text SceneOutterText;
    [Header("씬 최대 인원 텍스트")]
    public Text SceneMaxText;
    [Header("씬 설명 텍스트")]
    public Text SceneDescText;

    [Header("플레이어 이름 묘사 텍스트")]
    public Text NameText;
    [Header("참가자 수 텍스트")]
    public Text CountText;
    [Header("연결 상태 텍스트")]
    public Text NetworkText;
    [Header("오류 텍스트")]
    public Text ErrorText;

    [Header("방 생성 버튼")]
    public GameObject CreateRoomBtn;
    [Header("셀 버튼들")]
    public Button[] CellBtn;
    [Header("이전 셀 로드")]
    public Button PreviousBtn;
    [Header("다음 셀 로드")]
    public Button NextBtn;

    [Header("생성할 방의 이름")]
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

        //배경음
        battleUIManager.audioManager.PlayBgm(AudioManager.BgmMulty.Lobby);
        //텍스트 자동 변경
        StartCoroutine(loadTextSwap());
    }

    #region 로비에 접속
    public void Connect()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();//AuthManager에서 이미 써서 필요 없음

    public override void OnJoinedLobby()//로비에 들어오면 닉네임 설정
    {
        //if (AuthManager.Instance.User != null)
        // PhotonNetwork.LocalPlayer.NickName = AuthManager.Instance.playerEmail;
        //else
        PhotonNetwork.LocalPlayer.NickName = "NickName" + Random.Range(0, 10000);//NickNameInput.text


        NameText.text = PhotonNetwork.LocalPlayer.NickName;

        //리스트 조정
        myList.Clear();

        isJoinedLobby = true;
    }
    #endregion 

    #region 시작시 페이드 아웃
    IEnumerator loadTextSwap()//로딩 중 텍스트
    {
        loadText.text = "로비 입장중.";
        yield return wait0_35;

        loadText.text = "로비 입장중..";
        yield return wait0_35;

        loadText.text = "로비 입장중...";
        yield return wait0_35;

        if (isJoinedLobby)//로딩 완료
            StartCoroutine(StartFadeOut());
        else//로딩 중
            StartCoroutine(loadTextSwap());
    }

    IEnumerator StartFadeOut()//페이드 아웃
    {
        //텍스트와 별 비활성화
        loadText.gameObject.SetActive(false);
        loadGameObject.SetActive(false);

        Color color = loadFadeOutImage.color;
        float time = 1, minTime = 0;

        while (time > minTime)
        {
            time -= Time.deltaTime;
            float t = time / 1;//대기 시간

            color.a = Mathf.Lerp(0, 1, t);
            loadFadeOutImage.color = color;

            yield return null;
        }

        //입장 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        loadFadeOut.SetActive(false);
    }
    #endregion 

    
    void Update()
    {
        //네트워크 연결 상태
        NetworkText.text = PhotonNetwork.NetworkClientState.ToString();
        //네트워크 전체에서 룸에 있는 인원 빼면 로비에 있는 인원 수 나옴
        CountText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "명 로비 / "
            + PhotonNetwork.CountOfPlayersInRooms + "명 방 / "
            + PhotonNetwork.CountOfPlayers + "명 접속 중";
    }

    #region 단절됐을 때
    public void Disconnect() 
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        //입장 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //씬 전환
        SceneManager.LoadScene("Home");
    }
    #endregion

    #region 방 리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {

        if (num == -2)//왼쪽 화살표
        {
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
            --currentPage;
        }
        else if (num == -1)//오른쪽 화살표
        {
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
            ++currentPage;
        }
        else //입장 버튼
        {
            battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        }

        MyListRenewal();
    }
    void MyListRenewal()
    {
        // 최대 페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼 활성화, 비활성화
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;//각 페이지의 첫 번째 방의 인덱스
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            //방 이름
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text =
                (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            //플레이어 수
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text =
                (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
            //이미지
            Image cImage = CellBtn[i].transform.GetChild(3).GetComponent<Image>();
            if (multiple + i < myList.Count && myList[multiple + i].CustomProperties.ContainsKey("RoomImage"))
            {
                string imagePath = (string)myList[multiple + i].CustomProperties["RoomImage"];

                // imagePath를 사용하여 이미지를 로드하거나 다른 방법을 사용하여 이미지를 설정하세요.
                // 예를 들어 Resources.Load를 사용할 수 있습니다.
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
                // 이미지가 없을 경우 기본 이미지를 설정하거나 아무 작업을 하지 않습니다.
                cImage.sprite = null;
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)//서버에서 자동으로 룸의 목록을 가져오는 그거인가 봄
    {
        //아래에 로비에 접근할 때마다 리스트 초기화해서 괜찮음
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)//현재 존재하는 방이라면
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]); //근데 포함하고 있지 않다면, 더한다
                else myList[myList.IndexOf(roomList[i])] = roomList[i];     //받은 것의 인덱스를 추출해 동기화(인원만 바뀐경우 동기화)  
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion


    #region 방
    public void CreateRoom() //방 생성
    {
        //옛날 방식
        //PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 2 });//수정함

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
                { "RoomImage", cellSprite.name } // 선택한 이미지의 경로를 저장
            },
            CustomRoomPropertiesForLobby = new string[] { "IsAllowedToEnter", "IsAllowedToExit", "SceneName", "RoomImage" } // 로비에서도 이 속성을 보여주기 위해 추가
        };


        PhotonNetwork.CreateRoom(sceneName + roomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        //입장 효과음
        battleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

        //옛날 방식
        //PhotonNetwork.LoadLevel("Chap1_Scene");

        // 룸 입장 전에 RoomOptions에서 설정한 bool 변수를 체크하여 조건에 따라 입장 허용 여부를 확인
        bool canEnterRoom = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsAllowedToEnter"];
        if (canEnterRoom)
        {
            //들어갈 룸의 Scene 이름 확인
            string sceneName = (string)PhotonNetwork.CurrentRoom.CustomProperties["SceneName"];
            //실제로 입장
            PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {
            ErrorText.text = "이미 게임이 진행 중이므로, 룸에 입장 할 수 없습니다.";
            PhotonNetwork.LeaveRoom();
        }
    }

    
    public void JoinRandomRoom()//랜덤 방 입장
    {
        //AuthManager.Instance.audioManager.PlaySfx(AudioManager.Sfx.DoorOpen, true);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message) //같은 이름의 룸을 만들면 실패
    {
        ErrorText.text = "생성 실패";
        //RoomInput.text = ""; CreateRoom(); 
    }
    public override void OnJoinRandomFailed(short returnCode, string message) //같은 이름의 룸을 만들면 실패
    {
        ErrorText.text = "입장 실패";
        //RoomInput.text = "";
        //CreateRoom();


    }

    //PhotonNetwork.PlayerList[]:배열로 하나 하나 접근
    //PhotonNetwork.CurrentRoom.Name: 현재 방 이름
    //PhotonNetwork.CurrentRoom.PlayerCount: 방에 있는 사람 수
    //PhotonNetwork.CurrentRoom.MaxPlayers: 방 최대 사람 수
    #endregion
}

