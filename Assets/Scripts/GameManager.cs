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
	[SerializeField] private UpdatePlayersUI _updatePlayersUI;
	private Dictionary<int, int> _playersPoints = new Dictionary<int, int>();
	private List<Transform> _playersTransform = new List<Transform>();
	private List<PlayerHealth> _players = new List<PlayerHealth>();
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

		_thinkingPhaseTimer = new Timer(10f);
		_thinkingPhaseTimer.OnTimerEnd += () => photonView.RPC("ThinkingPhaseEnd", RpcTarget.All);
		_thinkingPhaseTimer.SetPause();

		_deckOfCards.OnCardsDistribution += () => photonView.RPC("ThinkingPhaseStart", RpcTarget.All);
	}

	//Срабатывает локально для клиентов
	public override void OnJoinedRoom()
	{
		var player = PhotonNetwork.Instantiate(_player.name, _spawnPoints[PhotonNetwork.PlayerList.Length - 1].GetSpawnPointTransform().position, Quaternion.identity);

		player.GetComponent<PlayerLook>().Initializing();

		for (int i = 0; i < PhotonNetwork.CountOfPlayers; i++) _spawnPoints[i].IsEmpty = false;
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

		if (PhotonNetwork.IsMasterClient)
			foreach (var player in FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None)) _players.Add(player);

		//Надо переделать нормально
		_playersTransform.Clear();

		foreach (SpawnPoint spawnPoint in _spawnPoints) 
			if (!spawnPoint.IsEmpty) _playersTransform.Add(spawnPoint.transform);

		DistributionPhase();
	}

	private void DistributionPhase()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_gamePhase = 1;

		_deckOfCards.GetRandomCards();
	}

	[PunRPC]
	private void ThinkingPhaseStart()
	{
		_gamePhase = 2;

		if (PhotonNetwork.IsMasterClient)  _thinkingPhaseTimer.Continue();
		OnThinkingPhaseStart?.Invoke();
	}

	[PunRPC]
	private void ThinkingPhaseEnd()
	{
		_gamePhase = 3;


		if (PhotonNetwork.IsMasterClient) _thinkingPhaseTimer.ResetTimer(true);

		photonView.RPC("PlayPhase", RpcTarget.All);

		OnThinkingPhaseEnd?.Invoke();
	}

	[PunRPC]
	private void PlayPhase()
	{
		_gamePhase = 4;
	}

	[PunRPC]
	private void FinalPhase()
	{
		int t = 0;

		_gamePhase = 0;

		if (PhotonNetwork.IsMasterClient) _playPhaseTimer.ResetTimer(false);

		for (int i = 0; i < _players.Count; i++)
			if (!_players[i].IsDead) t++;

		if (t <= 1) photonView.RPC("GameEnd", RpcTarget.All);
		else DistributionPhase();
	}

	[PunRPC]
	public void GameEnd()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		int maxPoints = 0;
		int playerActorNumber = 0;

		foreach (var key in _playersPoints.Keys)
		{
			if (_playersPoints[key] > maxPoints)
			{
				maxPoints = _playersPoints[key];
				playerActorNumber = key;
			}
		}

		_updatePlayersUI.photonView.RPC("UpdateWinImage", RpcTarget.All, playerActorNumber, maxPoints);
		Debug.Log("Игра закончилась");
	}


	[PunRPC]
	public void SetPoint(int playerActorNumber, int points)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_playersPoints[playerActorNumber] = points;
	}

	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		if (_thinkingPhaseTimer != null) _thinkingPhaseTimer.Tick(Time.deltaTime);

		if (_gamePhase == 4) _playPhaseTimer.Tick(Time.deltaTime);
	}
}
