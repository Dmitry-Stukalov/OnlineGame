using System;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth: MonoBehaviourPunCallbacks
{
	[SerializeField] private TextMeshPro _text;
	private int _health = 10;
	public bool IsDead { get; private set; }

	public event Action OnGetDamage;
	public event Action OnHealthChange;
	public event Action OnDeath;
	public event Action OnHeal;

	[PunRPC]
	public void GetDamage(int damageValue)
	{
		_health -= damageValue;

		if (_health <= 0)
		{
			_health = 0;
			IsDead = true;
			OnDeath?.Invoke();
		}

		UpdateText();
		OnGetDamage?.Invoke();
		OnHealthChange?.Invoke();
	}

	public void Heal(int healValue)
	{
		_health += healValue;

		if (_health >= 10) _health = 10;

		UpdateText();
		OnHeal?.Invoke();
		OnHealthChange?.Invoke();
	}

	public int GetHealth() => _health;

	public void UpdateText() => _text.text = $"{_health}/10";
}
