using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerName : MonoBehaviourPunCallbacks
{
	[SerializeField] private TextMeshPro _name;

	public void Initializing()
	{
		SetName();
	}

	public override void OnJoinedRoom()
	{
		SetName();
	}

	public override void OnPlayerEnteredRoom(Player player)
	{
		SetName();
	}

	public void SetName()
	{
		/*foreach (var player in FindObjectsByType<PhotonView>(sortMode: FindObjectsSortMode.None))
		{
			Debug.Log("Z");

			if (player.IsMine)
			{
				Debug.Log(PhotonNetwork.NickName);
				_name.text = PhotonNetwork.NickName;
			}
			else
			{
				Debug.Log(player.Owner.NickName);
				_name.text = player.Owner.NickName;
			}
		}*/

		//if (photonView.IsMine)
		//{
		//	Debug.Log(PhotonNetwork.NickName);
		//	_name.text = PhotonNetwork.NickName;
		//}
		//else
		//{
		//	Debug.Log(photonView.Owner.NickName);
		//	_name.text = photonView.Owner.NickName;
		//}
	}
}
