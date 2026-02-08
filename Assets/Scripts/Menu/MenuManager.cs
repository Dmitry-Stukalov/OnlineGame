using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviourPunCallbacks
{

	public void CreateRoom()
	{
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 5;
		PhotonNetwork.CreateRoom("TestRoom", roomOptions, TypedLobby.Default);
	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRandomRoom();
	}


	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("PlayScene");
		Debug.Log("Join to room");
	}
}
