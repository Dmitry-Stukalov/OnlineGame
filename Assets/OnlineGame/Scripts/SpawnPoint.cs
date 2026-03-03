using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	[SerializeField] private GameObject _stump;
	[SerializeField] private GameObject _mushrooms;
	[SerializeField] private Transform _transform;
	public bool IsEmpty { get; set; } = true;

	public Transform GetSpawnPointTransform() => _transform;

	public GameObject GetSpawnStump() => _stump;

	public GameObject GetMushrooms() => _mushrooms;

	public void ActivateMushrooms() => _mushrooms.SetActive(true);
}
