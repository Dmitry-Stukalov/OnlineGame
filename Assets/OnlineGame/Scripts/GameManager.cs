using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using Photon.Realtime;
using System.Threading;
using System.Linq;
using System;
using System.Collections;


public class GameManager : MonoBehaviourPunCallbacks
{
	[Header("Death")]
	[SerializeField] private GameObject DeathCameraCenter;
	[SerializeField] private Camera DeathCamera;

	[SerializeField] private GameObject _player;
	[SerializeField] private List<SpawnPoint> _spawnPoints;
	[SerializeField] private DeckOfCards _deckOfCards;
	[SerializeField] private UpdatePlayersUI _updatePlayersUI;
	[SerializeField] private Clocks _clocks;
	private Dictionary<int, int> _playersPoints = new Dictionary<int, int>();
	private Dictionary<int, int> _playerHealths = new Dictionary<int, int>();
	private List<Transform> _playersTransform = new List<Transform>();
	private List<PlayerHealth> _players = new List<PlayerHealth>();
	private Timer _thinkingPhaseTimer;
	private Timer _playPhaseTimer;					//Заглушка
	private int _gamePhase = -1;
	private bool IsCardsEnd = false;
	private bool IsGameStart = false;

	public event Action OnThinkingPhaseStart;
	public event Action OnThinkingPhaseEnd;
	public event Action OnPlayPhaseStart;
	public event Action OnFinalPhaseStart;

	//Только для мастер клиента
	private void Awake()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			_spawnPoints[0].IsEmpty = false;
			return;
		}

		var player = PhotonNetwork.Instantiate(_player.name, _spawnPoints[0].GetSpawnPointTransform().position, Quaternion.identity);

		_spawnPoints[0].ActivateMushrooms();

		player.GetComponent<PlayerLook>().Initializing(_spawnPoints[0].GetSpawnStump().transform.rotation.eulerAngles.y, DeathCameraCenter, DeathCamera);
		player.GetComponent<PlayerHealthMushrooms>().SetMushrooms(_spawnPoints[0].GetSpawnStump().GetComponentInChildren<SpawnPoint>().GetMushrooms());
		_spawnPoints[0].IsEmpty = false;

		_playPhaseTimer = new Timer(5f);
		_playPhaseTimer.OnTimerEnd += () => photonView.RPC("FinalPhase", RpcTarget.All);

		_thinkingPhaseTimer = new Timer(16f);
		_thinkingPhaseTimer.OnSecondTick += () => photonView.RPC("ClocksTick", RpcTarget.All);
		_thinkingPhaseTimer.OnTimerEnd += () => photonView.RPC("ThinkingPhaseEnd", RpcTarget.All);
		_thinkingPhaseTimer.SetPause();

		_deckOfCards.OnCardsDistribution += () => photonView.RPC("ThinkingPhaseStart", RpcTarget.All);
		_deckOfCards.OnCardsEnd += () => IsCardsEnd = true;

		_playerHealths[1] = 10;
	}

	//Срабатывает локально для клиентов
	public override void OnJoinedRoom()
	{
		if (!IsGameStart)
		{
			var player = PhotonNetwork.Instantiate(_player.name, _spawnPoints[PhotonNetwork.PlayerList.Length - 1].GetSpawnPointTransform().position, Quaternion.identity);

			player.GetComponent<PlayerLook>().Initializing(_spawnPoints[PhotonNetwork.PlayerList.Length - 1].transform.rotation.eulerAngles.y, DeathCameraCenter, DeathCamera);
			player.GetComponent<PlayerHealthMushrooms>().SetMushrooms(_spawnPoints[PhotonNetwork.PlayerList.Length - 1].GetSpawnStump().GetComponentInChildren<SpawnPoint>().GetMushrooms());

			StartCoroutine(FindPlayersOnJoin());
		}
	}

	//Срабатывает у всех
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		StartCoroutine(NewPlayerInitializing(newPlayer));

		//var player = FindPlayerByActor(newPlayer.ActorNumber);

		//_spawnPoints[PhotonNetwork.PlayerList.Length - 1].ActivateMushrooms();
		//_spawnPoints[PhotonNetwork.PlayerList.Length - 1].IsEmpty = false;

		//player.GetComponent<PlayerHealthMushrooms>().SetMushrooms(_spawnPoints[PhotonNetwork.PlayerList.Length - 1].GetSpawnStump().GetComponentInChildren<SpawnPoint>().GetMushrooms());

		//if (PhotonNetwork.IsMasterClient)
		//	_playerHealths[newPlayer.ActorNumber] = 10;
	}

	private IEnumerator NewPlayerInitializing(Player newPlayer)
	{
		yield return new WaitForSeconds(1f);

		var player = FindPlayerByActor(newPlayer.ActorNumber);

		_spawnPoints[PhotonNetwork.PlayerList.Length - 1].ActivateMushrooms();
		_spawnPoints[PhotonNetwork.PlayerList.Length - 1].IsEmpty = false;

		player.GetComponent<PlayerHealthMushrooms>().SetMushrooms(_spawnPoints[PhotonNetwork.PlayerList.Length - 1].GetSpawnStump().GetComponentInChildren<SpawnPoint>().GetMushrooms());

		if (PhotonNetwork.IsMasterClient)
			_playerHealths[newPlayer.ActorNumber] = 10;
	}

	private IEnumerator FindPlayersOnJoin()
	{
		yield return new WaitForSeconds(0.5f);

		for (int i = 0; i < PhotonNetwork.CountOfPlayers; i++) _spawnPoints[i].IsEmpty = false;

		foreach (var p in PhotonNetwork.PlayerList)
		{
			_spawnPoints[p.ActorNumber - 1].ActivateMushrooms();
			FindPlayerByActor(p.ActorNumber)?.GetComponent<PlayerHealthMushrooms>().SetMushrooms(_spawnPoints[p.ActorNumber - 1].GetSpawnStump().GetComponentInChildren<SpawnPoint>().GetMushrooms());
		}
	}

	//Срабатывает у всех
	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		_spawnPoints[PhotonNetwork.PlayerList.Length - 1].IsEmpty = true;
		_spawnPoints[otherPlayer.ActorNumber - 1].DeactivateMushrooms();
	}

	public void RestartGame()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		//Воскресить мертвых
		_players[0].photonView.RPC("RestartGameForAll", RpcTarget.All);

		//Перезагрузить ХП
		foreach (var key in _playerHealths.Keys.ToArray()) _playerHealths[key] = 10;

		//Перезагрузить очки
		foreach (var key in _playersPoints.Keys.ToArray()) _playersPoints[key] = 0;

		//Перезагрузить колоду
		_deckOfCards.SetCardsCount(PhotonNetwork.CountOfPlayers);

		_gamePhase = 0;

		_updatePlayersUI.photonView.RPC("UpdatePlayersHealth", RpcTarget.All, _playerHealths);

		IsCardsEnd = false;

		StartCoroutine(DistributionPhasePause());

		//Воскресить мертвых
		//_players[0].photonView.RPC("RestartGameForAll", RpcTarget.All);
	}

	[PunRPC]
	public void StartGame()
	{
		if (_gamePhase != -1) return;

		_gamePhase = 0;

		IsGameStart = true;

		if (PhotonNetwork.IsMasterClient)
			foreach (var player in FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None)) _players.Add(player);

		//Надо переделать нормально
		_playersTransform.Clear();

		foreach (SpawnPoint spawnPoint in _spawnPoints) 
			if (!spawnPoint.IsEmpty) _playersTransform.Add(spawnPoint.transform);

		_updatePlayersUI.photonView.RPC("UpdatePlayersHealth", RpcTarget.All, _playerHealths);

		StartCoroutine(DistributionPhasePause());
	}

	private void DistributionPhase()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_gamePhase = 1;

		_deckOfCards.GetRandomCards();
	}

	private IEnumerator DistributionPhasePause()
	{
		yield return new WaitForSeconds(1);

		DistributionPhase();
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

		//photonView.RPC("ToStart", RpcTarget.All);
		_clocks.ToStart();

		photonView.RPC("PlayPhase", RpcTarget.All);

		OnThinkingPhaseEnd?.Invoke();
	}

	[PunRPC]
	private void PlayPhase()
	{
		_gamePhase = 4;

		OnPlayPhaseStart?.Invoke();
	}

	[PunRPC]
	private void FinalPhase()
	{
		int t = 0;
		int playerNumberActor = 0;

		_gamePhase = 0;

		OnFinalPhaseStart?.Invoke();

		if (PhotonNetwork.IsMasterClient) _playPhaseTimer.ResetTimer(false);
		else return;

		/*foreach (var player in PhotonNetwork.PlayerList)
		{
            if (_playerHealths[player.ActorNumber] > 0)
            {
				t++;

				playerNumberActor = player.ActorNumber;
            }
        }*/

		/*for (int i = 0; i < _players.Count; i++)
		{
			if (_players[i].GetHealth() > 0)
			{
				t++;

				playerNumberActor = _players[i].photonView.OwnerActorNr;
			}
		}*/

		/*if (t <= 1 || IsCardsEnd) photonView.RPC("GameEnd", RpcTarget.All, playerNumberActor);
		else StartCoroutine(DistributionPhasePause()); //DistributionPhase();*/

		StartCoroutine(CheckHealthPhase());
	}

	private IEnumerator CheckHealthPhase()
	{
		//if (!PhotonNetwork.IsMasterClient) return;
		yield return new WaitForSeconds(0.5f);

		int t = 0;
		int playerNumberActor = 0;

		foreach (var player in PhotonNetwork.PlayerList)
		{
			if (_playerHealths[player.ActorNumber] > 0)
			{
				t++;

				playerNumberActor = player.ActorNumber;
			}
		}

		if (t <= 1 || IsCardsEnd) photonView.RPC("GameEnd", RpcTarget.All, playerNumberActor);
		else StartCoroutine(DistributionPhasePause()); //DistributionPhase();
	}

	[PunRPC]
	public void GameEnd(int survivePlayerActorNumber)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		IsGameStart = false;

		_deckOfCards._cardsCount = -1;

		if (survivePlayerActorNumber != 0) _updatePlayersUI.photonView.RPC("UpdateWinImage", RpcTarget.All, survivePlayerActorNumber, _playersPoints[survivePlayerActorNumber]);
		else
		{
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
		}
	}

	[PunRPC]
	public void SetPoint(int playerActorNumber, int points)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_playersPoints[playerActorNumber] = points;
	}

	[PunRPC]
	public void SetHealth(int playerActorNumber, int health)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		_playerHealths[playerActorNumber] = health;

		_updatePlayersUI.photonView.RPC("UpdatePlayersHealth", RpcTarget.All, _playerHealths);
    }

	private GameObject FindPlayerByActor(int actorNumber)
	{
		foreach (var player in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
			if (player.OwnerActorNr == actorNumber && player.gameObject.CompareTag("Player")) return player.gameObject;

		return null;
	}

	//[PunRPC]
	//public void SetCardParent(int playerActorNumber)
	//{
	//	_deckOfCards.photonView.RPC("SetCardsParents", RpcTarget.All, playerActorNumber);
	//}

	[PunRPC]
	public void ClocksTick()
	{
		_clocks.ChangeStage();
	}

	private void Update()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		if (_thinkingPhaseTimer != null) _thinkingPhaseTimer.Tick(Time.deltaTime);

		if (_gamePhase == 4) _playPhaseTimer.Tick(Time.deltaTime);
	}
}
