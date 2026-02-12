using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Card : MonoBehaviourPunCallbacks
{
	public DeckOfCards Deck { get; set; }
	public PlayerHealth EnemyPlayer { get; set; }
	[field: SerializeField] public ParticleSystem Effect { get; set; }
	public Transform TargetPlayer { get; set; }
	[field: SerializeField] public int EffectValue { get; set; }
	[field: SerializeField] public int PointValue { get; set; }
	public bool IsMove { get; set; } = false;

	public abstract void SetTarget(Transform player);
	public abstract void PlayCard(PlayerHealth enemy);
	public abstract void DestroyCard();
}
