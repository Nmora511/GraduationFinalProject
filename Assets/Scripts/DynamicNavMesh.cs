using UnityEngine;
using Unity.AI.Navigation;

public class DynamicNavMesh : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;

    void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    void Start()
    {
        ConstructPaths();
    }

    public void ConstructPaths()
    {
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh generated successfully for current scenario.");
    }
}