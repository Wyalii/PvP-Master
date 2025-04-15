using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // Assign both spawn points in Inspector

    void Start()
    {
        // Only the local player runs this
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            int spawnIndex = playerIndex % spawnPoints.Length;

            Vector3 spawnPosition = spawnPoints[spawnIndex].position;

            PhotonNetwork.Instantiate("Characters/Death", spawnPosition, Quaternion.identity);
        }
    }
}
