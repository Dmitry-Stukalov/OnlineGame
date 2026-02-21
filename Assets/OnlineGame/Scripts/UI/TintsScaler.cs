using UnityEngine;

public class TintsScaler : MonoBehaviour
{
	private RectTransform _rectTransform;

	public void Initializing()
	{
		_rectTransform = GetComponent<RectTransform>();
		_rectTransform.sizeDelta = new Vector2(Screen.width / 12, Screen.height / 7);
	}
}
