using NUnit.Framework;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Pool;
using ExitGames.Client.Photon;

public class DeckOfCards : MonoBehaviour
{
	[SerializeField] private GameObject _card;
	public int CardsCount { get; set; } = 0;
	private ObjectPool<GameObject> Cards;

	public event Action OnCardsDistribution;


	private void Start()
	{
		Cards = new ObjectPool<GameObject>
		(
			createFunc: () => Instantiate(_card),
			actionOnGet: obj => obj.SetActive(true),
			actionOnRelease: obj => obj.SetActive(false),
			actionOnDestroy: Destroy,
			defaultCapacity: 5,    // Начальный размер
			maxSize: 20            // Максимум, остальное уничтожается
		);
	}

	public void SetCardsCount(int playersCount)
	{
		CardsCount = playersCount * 7;
	}

	public void Distribution(List<Transform> players)
	{
		//Разрача карт игрокам

		foreach (Transform player in players)
		{
			var card = Cards.Get();
			card.GetComponent<Card>().SetTarget(player);
		}

		Debug.Log("Z");
		OnCardsDistribution?.Invoke();
	}

	public void ReleaseCard(GameObject card)
	{
		Cards.Release(card);
		card.transform.position = transform.position;
		card.transform.rotation = transform.rotation;
	}
}
