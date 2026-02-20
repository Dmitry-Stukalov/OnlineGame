using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ConnectionWithServer : MonoBehaviourPunCallbacks
{
	private void Start()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to server!");
		SceneManager.LoadScene("Menu");
	}
}
