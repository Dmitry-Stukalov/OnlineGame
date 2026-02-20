using Photon.Pun;
using UnityEngine;

public class SimpleHeal : HealCard
{
	public override void PlayCard(GameObject enemy)
	{
		photonView.RPC("PlayEffect", RpcTarget.All, enemy.transform.position);

		base.PlayCard(enemy);
	}

	[PunRPC]
	private void PlayEffect(Vector3 targetPosition)
	{
		Effect.transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
		Effect?.Play();
	}
}
