using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkUI : MonoBehaviourPunCallbacks
{
    //public bool isConnectedToServer = false;

    // Start is called before the first frame update
    void Start()
    {
        //ConnectToServer();
    }

    private void Update()
    {
        
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Try Connect To Server...");
    }
    public void DisconnectFromServer()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            Debug.Log("Disconnected from Room!");
            if (PhotonNetwork.IsMasterClient) { 
                PhotonNetwork.Disconnect();
                Debug.Log("Disconnected from Server!");
            }
        }
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server!");
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room!");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player entered the Room!");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game!");

        #if UNITY_STANDALONE
        Application.Quit();
        #endif  
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
