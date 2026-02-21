using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class IsMine : MonoBehaviour
{
	[SerializeField] private GameObject _camera;
	[SerializeField] private PlayerLook _playerLook;
	[SerializeField] private PlayerHealth _playerHealth;
	[SerializeField] private List<GameObject> _UI;
	[SerializeField] private PhotonView _photonView;

	private void Start()
	{
		if (!_photonView.IsMine)
		{
			_camera.SetActive(false);
			_playerLook.enabled = false;
			_playerHealth.enabled = false;

			for (int i = 0; i < _UI.Count; i++) _UI[i].SetActive(false);
		}
	}
}
