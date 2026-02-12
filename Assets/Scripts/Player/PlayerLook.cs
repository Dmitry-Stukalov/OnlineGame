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

	[Header("RayCast")]
	[SerializeField] private Camera MyCamera;
	[SerializeField] private GameObject TintForPlay;
	private PlayerHealth _enemy;
	Vector3 ScreenCenter;
	private List<Card> _myCards = new List<Card>();
	private GameManager _gameManager;
	private Vector2 MouseAxis;
	private InputAction LookAction;
	private float RotationX;
	private float RotationY;
	private bool IsCanPlayCard = false;
	private bool IsChooseTarget = false;
	public bool IsHaveCard = false;

	public void Initializing()
	{
		if (!photonView.IsMine) return;

		LookAction = InputSystem.actions.FindAction("Look");

		_gameManager = FindFirstObjectByType<GameManager>();

		_gameManager.OnThinkingPhaseStart += ThinkingPhaseStart;
		_gameManager.OnThinkingPhaseEnd += ThinkingPhaseEnd;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		ScreenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Card"))
		{
			GetCard(other.gameObject.GetComponent<Card>());
			other.gameObject.GetComponent<Card>().IsMove = false;
			other.transform.SetParent(CardPlace.transform, false);
			other.transform.localRotation = Quaternion.Euler(0, 90, 90);
			other.transform.localPosition = Vector3.zero;
		}
	}

	private void GetCard(Card card)
	{
		_myCards.Add(card);
		IsHaveCard = true;
	}

	private void PlayCard(Card card, PlayerHealth enemy)
	{
		if (!photonView.IsMine) return;
		if (_enemy == null) return;

		card.PlayCard(enemy);
		_myCards.Remove(card);

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
		if (IsChooseTarget) PlayCard(_myCards[0], _enemy);

		IsCanPlayCard = false;
		IsChooseTarget = false;

		for (int i = 0; i < _myCards.Count; i++)
		{
			_myCards[i].DestroyCard();
			_myCards.RemoveAt(i);
		}
	}

	private void Update()
	{
		if (!photonView.IsMine) return;
		if (_playerHealth.IsDead) return;

		if (Keyboard.current.spaceKey.wasPressedThisFrame && PhotonNetwork.IsMasterClient) _gameManager.photonView.RPC("StartGame", RpcTarget.All);

		if (Keyboard.current.escapeKey.wasPressedThisFrame) Application.Quit();

		if (Mouse.current.leftButton.wasPressedThisFrame && _myCards.Count != 0 && IsCanPlayCard && _enemy != null) IsChooseTarget = true;//PlayCard(_myCards[0], _enemy);
		if (Mouse.current.rightButton.wasPressedThisFrame && _myCards.Count != 0 && IsCanPlayCard && IsChooseTarget) IsChooseTarget = false;

		MouseAxis = LookAction.ReadValue<Vector2>();

		RotationX += -MouseAxis.y * LookSpeed;
		RotationX = Mathf.Clamp(RotationX, -20, 20);

		RotationY += -MouseAxis.x * LookSpeed;
		//RotationY = Mathf.Clamp(RotationY, -45, 45);

		transform.rotation = Quaternion.Euler(RotationX, -RotationY, 0);
	}

	private void FixedUpdate()
	{
		if (!photonView.IsMine) return;

		Ray ray = MyCamera.ScreenPointToRay(ScreenCenter);

		if (!IsChooseTarget)
		{
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				Debug.Log($"Кидаю луч. IsCanPlayCard = {IsCanPlayCard}");
				if (hit.transform.CompareTag("Player") && IsCanPlayCard)
				{
					Debug.Log("Попал");
					TintForPlay.SetActive(true);
					_enemy = hit.transform.GetComponent<PlayerHealth>();
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
