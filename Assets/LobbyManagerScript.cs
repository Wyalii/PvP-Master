using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManagerScript : MonoBehaviourPunCallbacks
{

    public Button queueButton;
    public Text statusText;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // Sync fighting scene across clients
        PhotonNetwork.ConnectUsingSettings();
        statusText.text = "Connecting to server...";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
        statusText.text = "Connected! Press Queue to start.";
    }

    public override void OnJoinedLobby()
    {
        queueButton.interactable = true;
        statusText.text = "Ready to Queue.";
    }

    public void OnClickQueue()
    {
        statusText.text = "Searching for opponent...";
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        statusText.text = "No room found. Creating one...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 });
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Joined room. Waiting for opponent...";

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        statusText.text = "Opponent found! Starting game...";
        PhotonNetwork.LoadLevel("FightingScene"); // This loads scene for both players
    }
}
