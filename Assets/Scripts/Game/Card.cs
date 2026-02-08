using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
	public DeckOfCards Deck { get; set; }
	public PlayerHealth EnemyPlayer { get; set; }
	public ParticleSystem Effect { get; set; }
	public Transform TargetPlayer { get; set; }
	public int EffectValue { get; set; }
	public int PointValue { get; set; }
	public bool IsMove { get; set; } = false;


	public abstract void SetTarget(Transform player);
	public abstract void PlayCard();
	public abstract void ReleaseCard();
}
