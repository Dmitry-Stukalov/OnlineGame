using Photon.Pun;
using System.Collections;
using UnityEngine;

public class FireCard : DamageCard
{
	public override void PlayCard(GameObject enemy)
	{
		photonView.RPC("PlayEffect", RpcTarget.All, enemy.transform.position);

		base.PlayCard(enemy);
	}

	[PunRPC]
	private void PlayEffect(Vector3 targetPosition)
	{
		Effect.transform.position = new Vector3(targetPosition.x, targetPosition.y - 1, targetPosition.z);
		Effect?.Play();
	}
}
