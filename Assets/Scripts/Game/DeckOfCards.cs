using NUnit.Framework;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Pool;
using ExitGames.Client.Photon;
using Photon.Pun;

public class DeckOfCards : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _card;
	public int CardsCount { get; set; } = 0;

	public event Action OnCardsDistribution;


	public void SetCardsCount(int playersCount)
	{
		CardsCount = playersCount * 7;
	}

	public void Distribution()
	{
		//Разрача карт игрокам


		var card = PhotonNetwork.Instantiate(_card.name, transform.position, Quaternion.identity);

		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) if (player.GetComponent<PhotonView>().IsMine) card.GetComponent<Card>().SetTarget(player.transform);

		OnCardsDistribution?.Invoke();
	}
}
