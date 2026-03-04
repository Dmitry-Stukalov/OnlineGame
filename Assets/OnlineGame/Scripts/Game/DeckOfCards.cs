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
	[SerializeField] private AudioSource _distributionSound;
	[SerializeField] private List<GameObject> _cards;
	private List<int> _gameCards = new List<int>();
	private Dictionary<int, int> _cardsByActors = new Dictionary<int, int>();
	private Dictionary<GameObject, GameObject> _cardsObjectsByActors = new Dictionary<GameObject, GameObject>();
	public int CardsCount { get; set; } = 0;
	public int _cardsCount = -1;

	public event Action OnCardsDistribution;
	public event Action OnCardsEnd;


	public void SetCardsCount(int playersCount)
	{
		int index = 0;

		CardsCount = playersCount * 5;

		Debug.Log(CardsCount);

		_cardsCount = CardsCount;

		_gameCards.Clear();

		for (int i = 0; i < _cardsCount; i++) transform.GetChild(i).gameObject.SetActive(true);

		for (int i = 0; i < playersCount / 2; i++)
		{
			_gameCards.Add(0);
			_cardsCount--;
			Debug.Log("Добавлена карта смерти");
		}

		for (int i = 0; i < playersCount; i++)
		{
			_gameCards.Add(1);
			_cardsCount--;
		}

		index = _cardsCount / 2;

		for (int i = 0; i < index; i++)
		{
			_gameCards.Add(2);
			_cardsCount--;
		}

		index = _cardsCount;

		for (int i = 0; i < index; i++)
		{
			_gameCards.Add(3);
			_cardsCount--;
		}
	}

	public void GetRandomCards()
	{
		int randomNumber = -1;

		if (_cardsCount == -1) SetCardsCount(PhotonNetwork.CountOfPlayers);

		foreach (var player in PhotonNetwork.PlayerList)
		{
			randomNumber = UnityEngine.Random.Range(0, _gameCards.Count);

			_cardsByActors[player.ActorNumber] = _gameCards[randomNumber];

			_gameCards.RemoveAt(randomNumber);
		}

		if (_gameCards.Count == 0) OnCardsEnd?.Invoke();

		for (int i = CardsCount - 1; i >  _gameCards.Count - 1; i--) transform.GetChild(i).gameObject.SetActive(false);

		Debug.Log($"Карт осталось: {_gameCards.Count}");

		photonView.RPC("Distribution", RpcTarget.All, _cardsByActors);
	}

	[PunRPC]
	public void Distribution(Dictionary<int, int> cardsByActors)
	{
		ReceiveCard(cardsByActors);

		_distributionSound.Play();

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
				_cardsObjectsByActors[player] = card;
				break;
			}
		}
	}

	//[PunRPC]
	//public void SetCardsParents(int playerActorNumber)
	//{
	//	foreach (var key in _cardsObjectsByActors.Keys)
	//	{
	//		if (playerActorNumber == key.GetPhotonView().OwnerActorNr/* && !key.GetPhotonView().IsMine*/)
	//		{
	//			_cardsObjectsByActors[key].transform.SetParent(key.transform.GetChild(1).transform, false);
	//			return;
	//		}
	//	}
	//}
}	
