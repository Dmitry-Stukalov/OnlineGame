using UnityEngine;

public class ContinueButton : MonoBehaviour
{

	public void ContinueGame()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
