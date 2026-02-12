using NUnit.Framework;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Pool;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;

public class DeckOfCards : MonoBehaviourPunCallbacks
{
	[SerializeField] private List<GameObject> _cards;
	private int[] _players;
	public int CardsCount { get; set; } = 0;

	public event Action OnCardsDistribution;


	public void SetCardsCount(int playersCount)
	{
		CardsCount = playersCount * 7;
	}

	public void CountPlayers()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_players = new int[PhotonNetwork.PlayerList.Length];

		GetRandomCards();
	}

	public void GetRandomCards()
	{
		for (int i = 0; i < _players.Length; i++)
		{
			_players[i] = UnityEngine.Random.Range(0, _cards.Count);
		}
	}

	[PunRPC]
	public void Distribution()
	{
		//Раздача карт игрокам

		for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
		{
			if (PhotonNetwork.PlayerList[i].IsLocal)
			{
				var card = PhotonNetwork.Instantiate(_cards[_players[i]].name, transform.position, Quaternion.identity);

				foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) if (player.GetComponent<PhotonView>().IsMine) card.GetComponent<Card>().SetTarget(player.transform);

				break;
			}
		}
		//var card = PhotonNetwork.Instantiate(_cards[_players[]].name, transform.position, Quaternion.identity);

		OnCardsDistribution?.Invoke();
	}
}
