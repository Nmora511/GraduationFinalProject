using UnityEngine;
using Unity.AI.Navigation;

public class DynamicNavMesh : MonoBehaviour
{
    private NavMeshSurface _navMeshSurface;

    private void Awake()
    {
        _navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        ConstructPaths();
    }

    private void ConstructPaths()
    {
        _navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh generated successfully for current scenario.");
    }
}