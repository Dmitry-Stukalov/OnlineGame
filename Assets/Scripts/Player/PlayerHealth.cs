using System;
using UnityEngine;

public class PlayerHealth: MonoBehaviour
{
	private int _health = 10;
	public bool IsDead { get; private set; }

	public event Action OnGetDamage;
	public event Action OnDeath;
	public event Action OnHeal;

	public void GetDamage(int damageValue)
	{
		_health -= damageValue;

		if (_health <= 0)
		{
			_health = 0;
			IsDead = true;
			OnDeath?.Invoke();
		}

		OnGetDamage?.Invoke();
	}

	public void Heal(int healValue)
	{
		_health += healValue;

		if (_health >= 10) _health = 10;

		OnHeal?.Invoke();
	}

	public int GetHealth() => _health;
}
