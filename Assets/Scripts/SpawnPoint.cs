using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	[SerializeField] private Transform _transform;
	public bool IsEmpty { get; set; } = true;

	public Transform GetSpawnPointTransform() => _transform;
}
