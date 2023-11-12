using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Lobby")]
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    public Text StatusText;
    public Text ErrorText;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    BattleUIManager BattleUIManager;
    private readonly string gameVersion = "1";

    private void Awake()
    {
        BattleUIManager = BattleUIManager.Instance;
    }

    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {

        if (num == -2)//왼쪽 화살표
        {
            BattleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
            --currentPage;
        }
        else if (num == -1)//오른쪽 화살표
        {
            BattleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Paper);
            ++currentPage;
        }
        else //입장 버튼
        {
            BattleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
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
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
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

    #region 서버연결
    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        //네트워크 전체에서 룸에 있는 인원 빼면 로비에 있는 인원 수 나옴
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "명 로비 / "
            + PhotonNetwork.CountOfPlayersInRooms + "명 방 / "
            + PhotonNetwork.CountOfPlayers + "명 접속 중";
    }

    private void Start()
    {
        Connect();
    }


    
    public void Connect() 
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    } //AuthManager에서 이미 써서 필요 없음

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();//AuthManager에서 이미 써서 필요 없음

    public override void OnJoinedLobby()//로비에 들어오면 닉네임 설정
    {
        //if (AuthManager.Instance.User != null)
           // PhotonNetwork.LocalPlayer.NickName = AuthManager.Instance.playerEmail;
        //else
            PhotonNetwork.LocalPlayer.NickName = "NickName" + Random.Range(0, 10000);//NickNameInput.text


        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        //입장 효과음
        BattleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);
        //리스트 조정
        myList.Clear();
    }

    public void Disconnect() //아래도 같이 호출되나봄
    {
        //입장 효과음
        BattleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

        PhotonNetwork.Disconnect();

        //AuthManager.Instance.Destroy();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("AuthScene");
    }
    #endregion

    #region 방

    public void CreateRoom() //방 생성
    {

        //옛날 방식
        //PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 2 });//수정함

        //최신 방식
        string sceneName = "PvP";
        //if (chapDropdown.value == 0) sceneName = "Chap1 ";
        //else if (chapDropdown.value == 1) sceneName = "Chap2 ";

        string roomName = RoomInput.text == "" ? "_Room" + Random.Range(0, 100) : RoomInput.text;

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 1,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "IsAllowedToEnter", true },
                { "IsAllowedToExit", true },
                { "SceneName", sceneName } // 이동하고자 하는 Scene의 이름 저장
            },
            CustomRoomPropertiesForLobby = new string[] { "IsAllowedToEnter", "IsAllowedToExit", "SceneName" } // 로비에서도 이 속성을 보여주기 위해 추가
        };


        PhotonNetwork.CreateRoom(sceneName + roomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        //입장 효과음
        BattleUIManager.audioManager.PlaySfx(AudioManager.Sfx.Door);

        //옛날 방식
        //PhotonNetwork.LoadLevel("Chap1_Scene");

        // 룸 입장 전에 RoomOptions에서 설정한 bool 변수를 체크하여 조건에 따라 입장 허용 여부를 확인
        bool canEnterRoom = (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsAllowedToEnter"];
        if (canEnterRoom)
        {
            //들어갈 룸의 Scene 이름 확인
            string sceneName = (string)PhotonNetwork.CurrentRoom.CustomProperties["SceneName"];
            //좌표 설정
            //if (sceneName == "Chap1 ") sceneName = "Chap1_Scene";
            //else if (sceneName == "Chap2 ") sceneName = "Chap2_Scene";
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

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }//같은 이름의 룸을 만들면 실패
    public override void OnJoinRandomFailed(short returnCode, string message) { ErrorText.text = "입장 할 방이 없습니다."; }//같은 이름의 룸을 만들면 실패( RoomInput.text = ""; CreateRoom(); )

    //PhotonNetwork.PlayerList[]:배열로 하나 하나 접근
    //PhotonNetwork.CurrentRoom.Name: 현재 방 이름
    //PhotonNetwork.CurrentRoom.PlayerCount: 방에 있는 사람 수
    //PhotonNetwork.CurrentRoom.MaxPlayers: 방 최대 사람 수
    #endregion
    //----------
}
