using Photon.Pun;
using UnityEngine;

public class IsMine : MonoBehaviour
{
	[SerializeField] private GameObject _camera;
	[SerializeField] private PlayerLook _playerLook;
	[SerializeField] private PlayerHealth _playerHealth;
	[SerializeField] private PhotonView _photonView;

	private void Start()
	{
		if (!_photonView.IsMine)
		{
			_camera.SetActive(false);
			_playerLook.enabled = false;
			_playerHealth.enabled = false;
		}
	}
}
