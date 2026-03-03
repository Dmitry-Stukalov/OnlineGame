using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
	[SerializeField] private GameObject _menu;

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if (Keyboard.current.escapeKey.wasPressedThisFrame) _menu.SetActive(!_menu.activeSelf);
	}
}
