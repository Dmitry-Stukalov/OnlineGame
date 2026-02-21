using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using UnityEngine.Rendering;

public class MenuManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject CannotConnectToRoomObject;
	public string RoomName { get; set; } = "BaseName";
	public int PlayersCount { get; set; } = 5;
	public string PlayerName { get; set; } = "Мастер говна";

	public void CreateRoom()
	{
		Debug.Log($"Создаю комнату: {RoomName}, с количеством игроков {PlayersCount}. Мое имя: {PlayerName}");

		PhotonNetwork.NickName = PlayerName;
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = PlayersCount;
		PhotonNetwork.CreateRoom(RoomName, roomOptions, TypedLobby.Default);
	}

	public void JoinRoom()
	{
		PhotonNetwork.NickName = PlayerName;
		PhotonNetwork.JoinRoom(RoomName);
		//PhotonNetwork.JoinRandomRoom();
	}


	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel("PlayScene");
		}
		Debug.Log("Join to room");
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		CannotConnectToRoomObject.SetActive(true);
	}
}
