using Photon.Pun;
using UnityEngine;

public class FlamethrowerCard : DamageCard
{
	public override void PlayCard(GameObject enemy)
	{
		photonView.RPC("PlayEffect", RpcTarget.All);

		base.PlayCard(enemy);
	}

	[PunRPC]
	private void PlayEffect()
	{
		Effect?.Play();
	}
}
