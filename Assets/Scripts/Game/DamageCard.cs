using UnityEngine;

public class DamageCard : Card, IDamageCard
{

	public override void SetTarget(Transform player)
	{
		TargetPlayer = player;
		IsMove = true;
	}

	public override void PlayCard()
	{
		DealDamage(EffectValue);
		Debug.Log("Card is played");
	}

	public override void ReleaseCard() => Deck.ReleaseCard(gameObject);

	public void DealDamage(int damageValue)
	{
		EnemyPlayer.GetDamage(damageValue);
		Debug.Log($"Enemy get {damageValue} damage");
	}

	private void Update()
	{
		if (IsMove)
		{
			transform.position = Vector3.MoveTowards(transform.position, TargetPlayer.position, 0.5f);
		}
	}
}
