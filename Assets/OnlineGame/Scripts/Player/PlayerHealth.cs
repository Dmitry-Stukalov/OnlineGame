using System;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
	[SerializeField] private int _maxHealth;
	[SerializeField] private PlayerHealthMushrooms _playerHealthMushrooms;
	private int _health = 10;
	private int _currentHealth = 0;
	public bool IsDead { get; set; } = false;

	public event Action OnGetDamage;
	public event Action OnHealthChange;
	public event Action OnDeath;
	public event Action OnHeal;

	private void Start()
	{
		_health = _maxHealth;
	}

	[PunRPC]
	public void GetDamage(int damageValue) => _health -= damageValue;

	[PunRPC]
	public void Heal(int healValue) => _health += healValue;

	[PunRPC]
	public void CheckHealth()
	{
		if (_health >= _maxHealth)
		{
			_health = _maxHealth;
		}

		if (_health <= 0)
		{
			_health = 0;
			IsDead = true;

			OnDeath?.Invoke();
		}

		OnHealthChange?.Invoke();
	}
	public int GetHealth() => _health;

	public void SetHealth(int newHealth) => _health = newHealth;

	[PunRPC]
	public void UpdateMushrooms()
	{
		var dif = _currentHealth - _health;

		if (dif > 0) _playerHealthMushrooms.HideMushroom(dif);
		else if (dif < 0) _playerHealthMushrooms.ShowMushroom(dif);

		_currentHealth = _health;
	}
}