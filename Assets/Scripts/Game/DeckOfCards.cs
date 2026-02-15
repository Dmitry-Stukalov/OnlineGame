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
	private Dictionary<int, int> _cardsByActors = new Dictionary<int, int>();
	private int[] _players;
	public int CardsCount { get; set; } = 0;

	public event Action OnCardsDistribution;


	public void SetCardsCount(int playersCount)
	{
		CardsCount = playersCount * 7;
	}

	public void GetRandomCards()
	{
		foreach (var player in PhotonNetwork.PlayerList) _cardsByActors[player.ActorNumber] = UnityEngine.Random.Range(0, _cards.Count);

		photonView.RPC("Distribution", RpcTarget.All, _cardsByActors);
	}

	[PunRPC]
	public void Distribution(Dictionary<int, int> cardsByActors)
	{
		ReceiveCard(cardsByActors);

		OnCardsDistribution?.Invoke();
	}

	private void ReceiveCard(Dictionary<int, int> cards)
	{
		int myCard = cards[PhotonNetwork.LocalPlayer.ActorNumber];

		CreateCard(myCard);
	}

	private void CreateCard(int cardID)
	{
		var card = PhotonNetwork.Instantiate(_cards[cardID].name, transform.position, Quaternion.identity);

		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (player.GetComponent<PhotonView>().IsMine)
			{
				card.GetComponent<Card>().SetTarget(player.transform);
				break;
			}
		}
	}
}	
