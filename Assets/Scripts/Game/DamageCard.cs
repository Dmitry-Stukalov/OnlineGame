using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DamageCard : Card, IDamageCard
{
	public override void SetTarget(Transform player)
	{
		TargetPlayer = player;
		IsMove = true;
	}

	public override void PlayCard(GameObject enemy)
	{
		EnemyPlayer = enemy;

		DealDamage(EffectValue);

		Debug.Log("Card is played");

		StartCoroutine(DestroyPause());
	}

	public override void DestroyCard()
	{
		transform.SetParent(null, true);
		PhotonNetwork.Destroy(gameObject);
	}

	public void DealDamage(int damageValue)
	{
		int actorNumber = EnemyPlayer.GetComponent<PhotonView>().OwnerActorNr;

		EnemyPlayer.GetComponent<PhotonView>().RPC("GetDamage", PhotonNetwork.CurrentRoom.GetPlayer(actorNumber), damageValue);

		EnemyPlayer.GetComponent<PlayerHealth>().GetDamage(damageValue);
		Debug.Log($"Enemy get {damageValue} damage. {EnemyPlayer.GetComponent<PlayerHealth>().GetHealth()}");
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
