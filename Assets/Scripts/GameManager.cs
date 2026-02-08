using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;
using System.Threading;
using System.Linq;


public class GameManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _player;
	[SerializeField] private List<SpawnPoint> _spawnPoints;
	[SerializeField] private DeckOfCards _deckOfCards;
	private List<Transform> _playersTransform = new List<Transform>();
	private Timer _thinkingPhaseTimer;
	private int _gamePhase = -1;

	private void Start()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		foreach (SpawnPoint spawnPoint in _spawnPoints)
		{
			PhotonNetwork.Instantiate(_player.name, _spawnPoints[0].GetSpawnPointTransform().position, Quaternion.identity);
			_spawnPoints[0].IsEmpty = false;
			break;
		}

		_thinkingPhaseTimer = new Timer(15f);
		_thinkingPhaseTimer.OnTimerEnd += ThinkingPhaseEnd;
		_thinkingPhaseTimer.SetPause();

		_deckOfCards.OnCardsDistribution += ThinkingPhaseStart;
	}

	//Срабатывает локально
	public override void OnJoinedRoom()
	{
		PhotonNetwork.Instantiate(_player.name, _spawnPoints[PhotonNetwork.PlayerList.Length - 1].GetSpawnPointTransform().position, Quaternion.identity);
		//_spawnPoints[PhotonNetwork.PlayerList.Length - 1].IsEmpty = false;
		//return;
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_spawnPoints[PhotonNetwork.PlayerList.Length - 1].IsEmpty = false;
	}

	public void StartGame()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_playersTransform.Clear();

		foreach (SpawnPoint spawnPoint in _spawnPoints)
		{
			if (!spawnPoint.IsEmpty) _playersTransform.Add(spawnPoint.transform);
		}

		Debug.Log("Игра началась");

		DistributionPhase();
	}

	private void DistributionPhase()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_deckOfCards.Distribution(_playersTransform);

		Debug.Log($"Раздаю карты {PhotonNetwork.PlayerList.Length} игрокам");
	}

	private void ThinkingPhaseStart()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_thinkingPhaseTimer.Continue();
		Debug.Log("Началась фаза размышлений");
	}

	private void ThinkingPhaseEnd()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_thinkingPhaseTimer.ResetTimer(true);

		PlayPhase();

		Debug.Log("Фаза размышлений закончилась");
	}

	private void PlayPhase()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		Debug.Log("Началась фаза разыгровки");
	}

	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_thinkingPhaseTimer.Tick(Time.deltaTime);
	}
}
