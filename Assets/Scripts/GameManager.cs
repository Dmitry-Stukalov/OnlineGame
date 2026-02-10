using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;
using System.Threading;
using System.Linq;
using System;


public class GameManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _player;
	[SerializeField] private List<SpawnPoint> _spawnPoints;
	[SerializeField] private DeckOfCards _deckOfCards;
	private List<Transform> _playersTransform = new List<Transform>();
	private Timer _thinkingPhaseTimer;
	private Timer _playPhaseTimer;					//Заглушка
	private int _gamePhase = -1;

	public event Action OnThinkingPhaseStart;
	public event Action OnThinkingPhaseEnd;

	//Только для мастер клиента
	private void Awake()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			_spawnPoints[0].IsEmpty = false;
			return;
		}

		var player = PhotonNetwork.Instantiate(_player.name, _spawnPoints[0].GetSpawnPointTransform().position, Quaternion.identity);
		player.GetComponent<PlayerLook>().Initializing();
		_spawnPoints[0].IsEmpty = false;

		_playPhaseTimer = new Timer(10f);
		_playPhaseTimer.OnTimerEnd += FinalPhase;

		_thinkingPhaseTimer = new Timer(5f);
		_thinkingPhaseTimer.OnTimerEnd += ThinkingPhaseEnd;
		_thinkingPhaseTimer.SetPause();

		_deckOfCards.OnCardsDistribution += ThinkingPhaseStart;
	}

	//Срабатывает локально для клиентов
	public override void OnJoinedRoom()
	{
		var player = PhotonNetwork.Instantiate(_player.name, _spawnPoints[PhotonNetwork.PlayerList.Length - 1].GetSpawnPointTransform().position, Quaternion.identity);

		player.GetComponent<PlayerLook>().Initializing();

		for (int i = 0; i < PhotonNetwork.CountOfPlayers; i++) _spawnPoints[i].IsEmpty = false;

		_playPhaseTimer = new Timer(10f);
		_playPhaseTimer.OnTimerEnd += FinalPhase;

		_thinkingPhaseTimer = new Timer(5f);
		_thinkingPhaseTimer.OnTimerEnd += ThinkingPhaseEnd;
		_thinkingPhaseTimer.SetPause();

		_deckOfCards.OnCardsDistribution += ThinkingPhaseStart;
	}

	//Срабатывает у всех
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		_spawnPoints[PhotonNetwork.PlayerList.Length - 1].IsEmpty = false;
	}

	//Срабатывает у всех
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		_spawnPoints[PhotonNetwork.PlayerList.Length - 1].IsEmpty = true;
	}

	[PunRPC]
	public void StartGame()
	{
		if (_gamePhase != -1) return;

		_gamePhase = 0;

		//Надо переделать нормально
		_playersTransform.Clear();

		foreach (SpawnPoint spawnPoint in _spawnPoints)
		{
			if (!spawnPoint.IsEmpty) _playersTransform.Add(spawnPoint.transform);
		}

		Debug.Log($"Игра началась, игроков: {_playersTransform.Count}");

		DistributionPhase();
	}

	private void DistributionPhase()
	{
		_gamePhase = 1;

		Debug.Log($"Раздаю карты {PhotonNetwork.PlayerList.Length} игрокам");

		_deckOfCards.Distribution();
	}

	private void ThinkingPhaseStart()
	{
		_gamePhase = 2;

		Debug.Log("Началась фаза размышлений");

		_thinkingPhaseTimer.Continue();
		OnThinkingPhaseStart?.Invoke();

	}

	private void ThinkingPhaseEnd()
	{
		_gamePhase = 3;

		Debug.Log("Фаза размышлений закончилась");

		_thinkingPhaseTimer.ResetTimer(true);

		PlayPhase();

		OnThinkingPhaseEnd?.Invoke();
	}

	private void PlayPhase()
	{
		_gamePhase = 4;

		Debug.Log("Началась фаза разыгровки");
	}

	private void FinalPhase()
	{
		_gamePhase = -1;

		_playPhaseTimer.ResetTimer(false);

		Debug.Log("Раунд закончился");
	}

	private void Update()
	{
		if (_thinkingPhaseTimer != null) _thinkingPhaseTimer.Tick(Time.deltaTime);

		if (_gamePhase == 4) _playPhaseTimer.Tick(Time.deltaTime);
	}
}
