using Photon.Pun;
using System.Collections;
using UnityEngine;

public class HealCard : Card, IHealCard
{
	public override void SetTarget(Transform player)
	{
		TargetPlayer = player;
		IsMove = true;
	}

	public override void PlayCard(GameObject enemy)
	{
		EnemyPlayer = enemy;

		Heal(EffectValue);

		StartCoroutine(DestroyPause());
	}

	public override void DestroyCard()
	{
		transform.SetParent(null, true);
		PhotonNetwork.Destroy(gameObject);
	}

	public void Heal(int healValue)
	{
		int actorNumber = EnemyPlayer.GetComponent<PhotonView>().OwnerActorNr;

		EnemyPlayer.GetComponent<PhotonView>().RPC("Heal", PhotonNetwork.CurrentRoom.GetPlayer(actorNumber), healValue);
	}

	private IEnumerator DestroyPause()
	{
		yield return new WaitForSeconds(EffectTime);

		DestroyCard();
	}

	private void FixedUpdate()
	{
		if (IsMove)
		{
			transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.position, 0.1f);
		}
	}
}
