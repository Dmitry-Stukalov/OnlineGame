using System;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class PlayerHealth: MonoBehaviourPunCallbacks
{
	[SerializeField] private int _maxHealth;
	[SerializeField] private TextMeshPro _text;
	private int _health = 10;
	public bool IsDead { get; private set; } = false;

	public event Action OnGetDamage;
	public event Action OnHealthChange;
	public event Action OnDeath;
	public event Action OnHeal;

	private void Start()
	{
		_health = _maxHealth;
	}

	[PunRPC]
	public void GetDamage(int damageValue)
	{
		_health -= damageValue;

		//if (_health <= 0)
		//{
		//	_health = 0;
		//	IsDead = true;
		//	OnDeath?.Invoke();
		//}

		//UpdateText();
		//OnGetDamage?.Invoke();
		//OnHealthChange?.Invoke();
	}

	[PunRPC]
	public void Heal(int healValue)
	{
		_health += healValue;

		//if (_health >= 10) _health = 10;

		//UpdateText();
		//OnHeal?.Invoke();
		//OnHealthChange?.Invoke();
	}

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

		UpdateText();
		OnHealthChange?.Invoke();
	}
	public int GetHealth() => _health;

	public int SetHealth(int newHealth) => _health = newHealth;

	public void UpdateText() => _text.text = $"{_health}/{_maxHealth}";
}
