using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UpdatePlayersUI : MonoBehaviourPunCallbacks
{
	private Dictionary<int, GameObject> _players = new Dictionary<int, GameObject>();

	private void Start()
	{
		StartCoroutine(SpawnPause(-1));
	}

	public override void OnJoinedRoom()
	{
		StartCoroutine(SpawnPause(-1));
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		StartCoroutine(SpawnPause(-1));
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		StartCoroutine(SpawnPause(2));
	}

	[PunRPC]
	public void UpdatePlayersHealth(Dictionary<int, int> playersHealth)
	{
		UpdatePlayersDictionary();

		foreach (var player in PhotonNetwork.PlayerList)
		{
			foreach (var key in playersHealth.Keys)
			{
				if (player.ActorNumber == key)
				{
					_players[key].GetComponent<PlayerHealth>().SetHealth(playersHealth[key]);
					_players[key].GetComponent<PlayerHealth>().UpdateMushrooms();
					break;
				}
			}
		}
	}

	private void UpdatePlayersName()
	{
		UpdatePlayersDictionary();

		foreach (var player in PhotonNetwork.PlayerList) _players[player.ActorNumber].GetComponentInChildren<TextMeshPro>().text = player.NickName;
	}

	private void UpdatePlayersDictionary()
	{
		_players.Clear();

		foreach (var player in PhotonNetwork.PlayerList)
		{
			GameObject needPlayer = FindPlayerByActor(player.ActorNumber);

			if (needPlayer != null)
			{
				_players.Add(player.ActorNumber, needPlayer);
			}
		}
	}

	[PunRPC]
	public void UpdateWinImage(int playerActorNumber, int playerPoints)
	{
		string playerNickName = "";

		foreach (var player in PhotonNetwork.PlayerList)
			if (player.ActorNumber == playerActorNumber) playerNickName = player.NickName;

		foreach (var player in PhotonNetwork.PlayerList)
			if (_players[player.ActorNumber].GetComponent<PlayerLook>().photonView.IsMine) _players[player.ActorNumber].GetComponent<PlayerLook>().GameEnd($"{playerNickName} победил");
	}

	private GameObject FindPlayerByActor(int actorNumber)
	{
		foreach (var player in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
			if (player.OwnerActorNr == actorNumber && player.gameObject.CompareTag("Player")) return player.gameObject;

		return null;
	}

	private IEnumerator SpawnPause(int number)
	{
		yield return new WaitForSeconds(1f);

		switch (number)
		{
			case -1:
				UpdatePlayersDictionary();
				UpdatePlayersName();
			break;

			case 0:
				UpdatePlayersName();
			break;

			case 2:
				UpdatePlayersDictionary();
			break;
		}
	}
}
