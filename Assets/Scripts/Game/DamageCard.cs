using Photon.Pun;
using UnityEngine;

public class DamageCard : Card, IDamageCard
{
	public override void SetTarget(Transform player)
	{
		TargetPlayer = player;
		IsMove = true;
	}

	public override void PlayCard(PlayerHealth enemy)
	{
		EnemyPlayer = enemy;

		DealDamage(EffectValue);

		Debug.Log("Card is played");

		DestroyCard();
	}

	public override void DestroyCard()
	{
		transform.SetParent(null, true);
		PhotonNetwork.Destroy(gameObject);
	}

	public void DealDamage(int damageValue)
	{
		EnemyPlayer.GetDamage(damageValue);
		Debug.Log($"Enemy get {damageValue} damage. {EnemyPlayer.GetHealth()}");
	}

	private void FixedUpdate()
	{
		if (IsMove)
		{
			transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.position, 0.1f);
		}
	}
}
