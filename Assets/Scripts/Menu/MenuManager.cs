using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviourPunCallbacks
{
	public void CreateRoom()
	{
		PhotonNetwork.NickName = "Мастер говна";
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 5;
		PhotonNetwork.CreateRoom("TestRoom", roomOptions, TypedLobby.Default);
	}

	public void JoinRoom()
	{
		PhotonNetwork.NickName = "Адепт говна";
		PhotonNetwork.JoinRandomRoom();
	}


	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("PlayScene");
		}
		Debug.Log("Join to room");
	}
}
