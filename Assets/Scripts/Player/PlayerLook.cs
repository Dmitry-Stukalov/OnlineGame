using Photon.Pun;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerLook : MonoBehaviour
{
	[SerializeField] private GameObject CardPlace;
	[SerializeField] private PlayerHealth _playerHealth;
	[SerializeField] private PhotonView _photonView;
	[SerializeField] private float LookSpeed;
	private List<Card> _myCards = new List<Card>();
	private GameManager _gameManager;
	private Vector2 MouseAxis;
	private InputAction LookAction;
	private float RotationX;
	private float RotationY;

	private void Awake()
	{
		if (!_photonView.IsMine) return;

		LookAction = InputSystem.actions.FindAction("Look");

		_gameManager = FindFirstObjectByType<GameManager>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void OnTriggerEnter(Collider other)
	{
		Debug.Log("ZZZ");

		if (other.CompareTag("Card"))
		{
			other.gameObject.GetComponent<Card>().IsMove = false;
			other.transform.SetParent(CardPlace.transform, false);
			other.transform.localRotation = Quaternion.Euler(0, 90, 90);
			other.transform.localPosition = Vector3.zero;
		}
	}

	public void GetCard(Card card)
	{
		_myCards.Add(card);
	}

	public void PlayCard(Card card)
	{
		card.PlayCard();
		_myCards.Remove(card);
	}

	private void Update()
	{
		if (!_photonView.IsMine) return;
		if (_playerHealth.IsDead) return;

		if (Keyboard.current.spaceKey.wasPressedThisFrame && PhotonNetwork.IsMasterClient) _gameManager.StartGame();

		if (Keyboard.current.escapeKey.wasPressedThisFrame) Application.Quit();

		MouseAxis = LookAction.ReadValue<Vector2>();

		RotationX += -MouseAxis.y * LookSpeed;
		RotationX = Mathf.Clamp(RotationX, -20, 20);

		RotationY += -MouseAxis.x * LookSpeed;
		//RotationY = Mathf.Clamp(RotationY, -45, 45);

		transform.rotation = Quaternion.Euler(RotationX, -RotationY, 0);
	}
}
