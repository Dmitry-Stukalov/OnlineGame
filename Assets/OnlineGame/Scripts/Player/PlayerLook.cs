using Photon.Pun;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class PlayerLook : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject CardPlace;
	[SerializeField] private PlayerHealth _playerHealth;
	[SerializeField] private float LookSpeed;
	[SerializeField] private PlayerName _playerName;
	[SerializeField] private GameObject _winImage;
	[SerializeField] private GameObject _restartButton;
	[SerializeField] private TextMeshProUGUI _winText;
	[SerializeField] private TintsScaler _tintsScaler;

	private GameObject DeathCameraCenter; 
	private Camera DeathCamera;

	[Header("RayCast")]
	[SerializeField] private Camera MyCamera;
	[SerializeField] private GameObject TintForPlay;
	[SerializeField] private GameObject TintForStartPlay;
	private GameObject _enemy;
	Vector3 ScreenCenter;


	private List<Card> _myCards = new List<Card>();
	private GameManager _gameManager;
	private float _startRotation;
	private Vector2 MouseAxis;
	private InputAction LookAction;
	private float RotationX;
	private float RotationY;
	private int _myPoints;
	private bool IsCanPlayCard = false;
	private bool IsChooseTarget = false;
	private bool IsLockCamera = false;
	public bool IsHaveCard = false;

	public void Initializing(float startRotation, GameObject deathCameraObject, Camera deathCamera)
	{
		if (!photonView.IsMine) return;

		_startRotation = startRotation;
		DeathCameraCenter = deathCameraObject;
		DeathCamera = deathCamera;

		LookAction = InputSystem.actions.FindAction("Look");

		_gameManager = FindFirstObjectByType<GameManager>();

		_gameManager.OnThinkingPhaseStart += ThinkingPhaseStart;
		_gameManager.OnThinkingPhaseEnd += ThinkingPhaseEnd;
		_gameManager.OnPlayPhaseStart += PlayPhase;
		_gameManager.OnFinalPhaseStart += FinalPhase;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		ScreenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

		_playerHealth.OnHealthChange += SetHealth;
		_playerHealth.OnDeath += Death;

		if (!PhotonNetwork.IsMasterClient)
		{
			TintForStartPlay.SetActive(false);
			_restartButton.SetActive(false);
		}

		_tintsScaler.Initializing();

	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Card"))
		{
			GetCard(other.gameObject.GetComponent<Card>());
			other.gameObject.GetComponent<Card>().IsMove = false;
			other.transform.SetParent(CardPlace.transform, false);
			other.transform.localRotation = Quaternion.Euler(90, 90, 90);
			other.transform.localPosition = Vector3.zero;
		}
	}

	private void GetCard(Card card)
	{
		_myCards.Add(card);
		IsHaveCard = true;
	}

	public int GetPoint() => _myPoints;

	private void PlayCard(Card card, GameObject enemy)
	{
		if (!photonView.IsMine) return;
		if (enemy == null) return;

		card.PlayCard(enemy);
		_myCards.Remove(card);

		_myPoints += card.PointValue;

		IsCanPlayCard = false;

		if (_myCards.Count == 0 ) IsHaveCard = false;
	}

	public void ThinkingPhaseStart()
	{
		if (!photonView.IsMine) return;
		IsCanPlayCard = true;
		IsChooseTarget = false;
	}

	public void ThinkingPhaseEnd()
	{
		if (!photonView.IsMine) return;

		IsCanPlayCard = false;
	}

	public void PlayPhase()
	{
		if (!photonView.IsMine) return;
		if (IsChooseTarget) PlayCard(_myCards[0], _enemy);

		IsChooseTarget = false;

		for (int i = 0; i < _myCards.Count; i++)
		{
			_myCards[i].DestroyCard();
			_myCards.RemoveAt(i);
		}
	}

	public void FinalPhase()
	{
		if (!photonView.IsMine) return;

		SetPoints();
		_playerHealth.CheckHealth();
	}

	public void GameEnd(string text)
	{
		if (!photonView.IsMine) return;

		_winImage.SetActive(true);
		_winText.text = text;

		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
	}

	private void SetHealth()
	{
		if (!PhotonNetwork.IsMasterClient) _gameManager.photonView.RPC("SetHealth", RpcTarget.MasterClient, photonView.OwnerActorNr, _playerHealth.GetHealth());
		else _gameManager.SetHealth(1, _playerHealth.GetHealth());
	}

	private void SetPoints()
	{
		if (!PhotonNetwork.IsMasterClient) _gameManager.photonView.RPC("SetPoint", RpcTarget.MasterClient, photonView.OwnerActorNr, _myPoints);
		else _gameManager.SetPoint(1, _myPoints);
	}

	private void Death()
	{
		MyCamera.enabled = false;
		DeathCamera.enabled = true;
	}

	private void Alive()
	{
		Debug.Log(DeathCamera);
		Debug.Log(MyCamera);
		GameObject.FindWithTag("DeathCamera").GetComponent<Camera>().enabled = false;
		//DeathCamera.enabled = false;
		GameObject.FindWithTag("MainCamera").GetComponent<Camera>().enabled = true;
		//MyCamera.enabled = true;
	}

	public void RestartGame()
	{
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;

		//_myPoints = 0;
		//_playerHealth.SetHealth(10);
		//_winImage.SetActive(false);

		_gameManager.RestartGame();

		photonView.RPC("RestartGameForAll", RpcTarget.All);
	}

	[PunRPC]
	public void RestartGameForAll()
	{
		if (!MyCamera.enabled)
		{
			MyCamera.enabled = true;
			DeathCamera.enabled = false;
		}

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		_myPoints = 0;
		_playerHealth.SetHealth(10);
		_playerHealth.IsDead = false;
		_winImage.SetActive(false);
	}

	private void Update()
	{
		if (!photonView.IsMine) return;

		if (Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			if (Cursor.visible)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
			}
		}

		if (Keyboard.current.spaceKey.wasPressedThisFrame && PhotonNetwork.IsMasterClient)
		{
			_gameManager.photonView.RPC("StartGame", RpcTarget.All);
			TintForStartPlay.SetActive(false);
		}

		IsLockCamera = Cursor.visible;

		if (Mouse.current.leftButton.wasPressedThisFrame && _myCards.Count != 0 && IsCanPlayCard && _enemy != null) IsChooseTarget = true;
		if (Mouse.current.rightButton.wasPressedThisFrame && _myCards.Count != 0 && IsCanPlayCard)
		{
			IsChooseTarget = true;
			_enemy = gameObject;
		}
		if (Mouse.current.middleButton.wasPressedThisFrame && _myCards.Count != 0 && IsCanPlayCard && IsChooseTarget) IsChooseTarget = false;

		if (IsLockCamera) return;

		MouseAxis = LookAction.ReadValue<Vector2>();

		if (_playerHealth.IsDead)
		{
			RotationY += -MouseAxis.x * LookSpeed;

			DeathCameraCenter.transform.rotation = Quaternion.Euler(0, -RotationY, 0);
		}
		else
		{
			RotationX += -MouseAxis.y * LookSpeed;
			RotationX = Mathf.Clamp(RotationX, -20, 20);

			RotationY += -MouseAxis.x * LookSpeed;

			transform.rotation = Quaternion.Euler(RotationX, -RotationY + _startRotation, 0);
		}
	}

	private void FixedUpdate()
	{
		if (!photonView.IsMine || _playerHealth.IsDead) return;

		Ray ray = MyCamera.ScreenPointToRay(ScreenCenter);

		if (!IsChooseTarget)
		{
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				if (hit.transform.CompareTag("Player") && IsCanPlayCard)
				{
					TintForPlay.SetActive(true);
					_enemy = hit.transform.gameObject;
				}
				else
				{
					TintForPlay.SetActive(false);
					_enemy = null;
				}
			}
			else
			{
				TintForPlay.SetActive(false);
				_enemy = null;
			}
		}
		else TintForPlay.SetActive(false);
	}
}
