using UnityEngine;
using System.Collections.Generic;

public class Clocks : MonoBehaviour
{
	[SerializeField] private List<MeshFilter> _stages;
	[SerializeField] private MeshFilter _mesh;
	private int _currentStage = 0;

	public void ChangeStage()
	{
		_currentStage++;

		if (_currentStage > 16)	_currentStage = 0;

		_mesh.sharedMesh = _stages[_currentStage].sharedMesh;
	}

	public void ToStart()
	{
		_currentStage = 0;

		_mesh.sharedMesh = _stages[_currentStage].sharedMesh;
	}
}
