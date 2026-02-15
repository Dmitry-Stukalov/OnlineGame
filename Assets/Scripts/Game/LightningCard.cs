using DigitalRuby.LightningBolt;
using NUnit.Framework.Constraints;
using Photon.Pun;
using System.Collections;
using System.IO.IsolatedStorage;
using UnityEngine;

public class LightningCard : DamageCard
{
	[SerializeField] private LightningBoltScript _lightning;
	private bool IsStartEffect = false;

	public override void PlayCard(GameObject enemy)
	{
		photonView.RPC("PlayEffect", RpcTarget.All, transform.position, enemy.transform.position);

		base.PlayCard(enemy);
	}

	[PunRPC]
	private void PlayEffect(Vector3 startPosition, Vector3 endPosition)
	{
		_lightning.StartPosition = startPosition;
		_lightning.EndPosition = endPosition;
		IsStartEffect = true;
	}

	private void Update()
	{
		if (IsStartEffect) _lightning.Trigger();
	}
}
