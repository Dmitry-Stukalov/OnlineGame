using UnityEngine;

public class Mushroom : MonoBehaviour
{
	[SerializeField] private Animator _animator;
	public bool IsHide { get; set; } = true;

	public void Show()
	{
		_animator.SetBool("IsHide", false);
		IsHide = false;
	}

	public void Hide()
	{
		_animator.SetBool("IsHide", true);
		IsHide = true;
	}
}
