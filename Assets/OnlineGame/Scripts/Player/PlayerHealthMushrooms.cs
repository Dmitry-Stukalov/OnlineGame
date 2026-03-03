using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Photon.Pun;

public class PlayerHealthMushrooms : MonoBehaviourPunCallbacks
{
	[SerializeField] private List<Mushroom> _mushrooms = new List<Mushroom>();

	[PunRPC]
	public void ShowMushroom(int count)
	{
		int t = 0;

		foreach (var m in _mushrooms)
		{
			if (m.IsHide)
			{
				m.Show();
				t++;
				
				if (t == count) return;
			}
		}
	}

	[PunRPC]
	public void HideMushroom(int count)
	{
		int t = 0;

		foreach (var m in _mushrooms)
		{
			if (!m.IsHide)
			{
				m.Hide();
				t++;

				if (t == count) return;
			}
		}
	}

	public void SetMushrooms(GameObject mushrooms)
	{
		for (int i = mushrooms.transform.childCount - 1; i >= 0; i--)
		{
			_mushrooms.Add(mushrooms.transform.GetChild(i).gameObject.GetComponent<Mushroom>());
		}

		HideMushroom(10);
	}
}
