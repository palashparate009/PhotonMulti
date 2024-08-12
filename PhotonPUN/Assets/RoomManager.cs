using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    public GameObject player;

    [Space]
    public Transform[] spawnPoints;

    [Space]
    public GameObject roomCam;

    [Space]
    public GameObject nameUI;
    public GameObject connectingUI;
    public GameObject connectionFailedUI;
    public GameObject joinGameScreenUI;
    public GameObject crosshairUI;
    //public GameObject LeaveGameUI;

    private string nickname = "unnamed";

    public string roomNameToJoin = "test";

    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;


    public List<Transform> availableSpawnPoints;
    private Dictionary<int, Transform> playerSpawnPoints;


    private void Awake()
    {
        instance = this;
        InitializeSpawnPoints();

    }
    private void InitializeSpawnPoints()
    {
        // Initialize the list with all spawn points
        availableSpawnPoints = new List<Transform>(spawnPoints);
        playerSpawnPoints = new Dictionary<int, Transform>();

    }
    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting.....");

        RoomOptions roomOptions = new RoomOptions();
        //roomOptions.MaxPlayers = 2;
        roomOptions.MaxPlayers = (byte)spawnPoints.Length;

        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, roomOptions, null);


        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("We are connected and are in the room");

        roomCam.SetActive(false);
        //LeaveGameUI.SetActive(true);
        crosshairUI.SetActive(true);   
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {

        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogError("No available spawn points!");
            return;
        }

        int index = Random.Range(0, availableSpawnPoints.Count);
        Transform spawnPoint = availableSpawnPoints[index];

        //Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        _player.GetComponent<Health>().isLocalPlayer = true;

        _player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;


        playerSpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber] = spawnPoint;

        availableSpawnPoints.RemoveAt(index);

    }
    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {

        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (playerSpawnPoints.TryGetValue(otherPlayer.ActorNumber, out Transform spawnPoint))
        {
            availableSpawnPoints.Add(spawnPoint);

            playerSpawnPoints.Remove(otherPlayer.ActorNumber);

            Debug.Log($"Player {otherPlayer.NickName} left, freeing spawn point.");
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
        connectionFailedUI.SetActive(true);
        connectingUI.SetActive(false);
        nameUI.SetActive(false);

    }
    //public void ConnectionFailedBackButton()
    //{
    //    connectionFailedUI.SetActive(false);
    //    connectingUI.SetActive(false);
    //    nameUI.SetActive(true);
    //    //joinGameScreenUI.SetActive(false);
    //}
    //public void NameScreenBackButton()
    //{
    //    connectionFailedUI.SetActive(false);
    //    connectingUI.SetActive(false);
    //    nameUI.SetActive(false);
    //    joinGameScreenUI.SetActive(true);
    //}

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom(); 
        //LeaveGameUI.SetActive(false);
        joinGameScreenUI.SetActive(true);
        Debug.Log("Left the room.");
    }

    //public override void OnLeftRoom()
    //{
    //    base.OnLeftRoom();
    //    Debug.Log("You have left the room.");
    //    // Optionally, handle any additional logic when leaving the room
    //    roomCam.SetActive(true);
    //    connectingUI.SetActive(false );
    //   joinGameScreenUI.SetActive(true );

    //}
}
