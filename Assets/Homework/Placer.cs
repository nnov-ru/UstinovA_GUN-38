using UnityEngine;
using UnityEngine.AI;

public class InitialSpherePlacer : MonoBehaviour
{
    [SerializeField] private GameObject[] _spheres;
    [SerializeField] private string _sphereTag = "Ball";
    [SerializeField] private float _yOffset = 0.5f;

    void Start()
    {
        // Вариант 1: Использовать массив из инспектора
        if (_spheres != null && _spheres.Length > 0)
        {
            PlaceSpheres(_spheres);
        }

        // Вариант 2: Найти все сферы по тегу
        GameObject[] taggedSpheres = GameObject.FindGameObjectsWithTag(_sphereTag);
        if (taggedSpheres.Length > 0)
        {
            PlaceSpheres(taggedSpheres);
        }
    }

    private void PlaceSpheres(GameObject[] spheres)
    {
        foreach (GameObject sphere in spheres)
        {
            if (NavMesh.SamplePosition(sphere.transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                sphere.transform.position = hit.position + Vector3.up * _yOffset;
                Debug.Log($"Размещена сфера: {sphere.name}");
            }
        }
    }
}
