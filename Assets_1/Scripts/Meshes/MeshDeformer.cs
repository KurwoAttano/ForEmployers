using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    public Mesh RendererMesh;
    public Mesh ColliderMesh;

    Vector3[] originalVertices;
    Vector3[] displacedVertices;
    Vector3[] vertexVelocities;

    List<Vector3> hitPoints = new List<Vector3>();
    List<Vector3> hitDirection = new List<Vector3>();

    //public float force = 10f;
    //// Пружинаня сила
    //public float springForce = 20f;
    //// Затухание
    //public float damping = 5f;

    public float force = 1f;
    // Пружинаня сила
    public float springForce = 20f;
    // Затухание
    public float damping = 5f;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        GetComponent<MeshFilter>().mesh = GetComponent<MeshCollider>().sharedMesh;
        ColliderMesh = GetComponent<MeshCollider>().sharedMesh;
        RendererMesh = GetComponent<MeshFilter>().mesh;

        // Оригинальные вершины
        originalVertices = RendererMesh.vertices;
        // Смещенные вершины
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
            displacedVertices[i] = originalVertices[i];
        // Скорости вершин
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    private void Update()
    {
        UpdateVertexes();

        GetComponent<MeshCollider>().sharedMesh = RendererMesh;
        //RendererMesh = ColliderMesh;

        //hitPoints.Clear();
        //hitDirection.Clear();
    }
    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            AddDeformingForce(contact.point, force);
        }

        UpdateVertexes();
        //Debug.Log(hitPoints.Count);

        hitPoints.Clear();
        hitDirection.Clear();
    }

    private void UpdateVertexes()
    {
        for (int i = 0; i < displacedVertices.Length; i++)
            DisplaceVertex(i);
        RendererMesh.vertices = displacedVertices;
        RendererMesh.RecalculateNormals();
    }

    public void AddDeformingForce(Vector3 point, float force)
    {
        // Направление
        //Vector3 direction = -(point - transform.position);
        Vector3 direction = transform.position - point;

        // Поиск ближайшей вершины
        /*float minDist = Vector3.Distance(displacedVertices[0], point);
        int VertexI = 0;
        for (int i = 1; i < displacedVertices.Length; i++)
        {
            float nextDist = Vector3.Distance(displacedVertices[i] + transform.position, point);
            if (nextDist < minDist)
            {
                minDist = nextDist;
                VertexI = i;
            }
        }*/

        DrawDebugMarker(point, Color.blue); //!
        //DrawDebugMarker(RendererMesh.vertices[VertexI] + transform.position, Color.magenta); //!

        /*// Ослабленная сила
        float attenuatedForce = force / (1f + direction.sqrMagnitude);
        // Скорость из силы
        float velocity = attenuatedForce * Time.deltaTime;
        // Направление на скорость
        //vertexVelocities[VertexI] += direction.normalized * velocity;
        // Запись контакта*/

        //float velocity = (direction * force) * Time.deltaTime;

        hitPoints.Add(point);
        //hitDirection.Add(direction.normalized * velocity);
        hitDirection.Add(direction.normalized * force);

        //Debug.Log(direction.normalized * force * Time.deltaTime + " else " + direction.normalized * force);
    }
    void DisplaceVertex(int i)
    {
        // ПЕРЕДЕЛАТЬ

        /*Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * Time.deltaTime;*/

        Vector3 velocity = new Vector3();
        for (int j = 0; j < hitPoints.Count; j++)
        {
            velocity += hitDirection[j].normalized + (displacedVertices[i] - hitPoints[j]).normalized;
            //Debug.Log(hitDirection[j] + " + " + (displacedVertices[i] - hitPoints[j]).normalized);
        }

        Vector3 displacement = originalVertices[i] - displacedVertices[i];
        velocity += displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;

        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * Time.deltaTime;

        //DrawDebugMarker(velocity + transform.position, Color.magenta);
        //Debug.Log(hitPoints.Count);

        DrawDebugMarker(Vector3.zero, Color.white);
        DrawDebugMarker(velocity, Color.green);
    }


    void DrawDebugMarker(Vector3 position, Color color, int duration = 1, float size = 0.05f)
    {
        Debug.DrawRay(position - new Vector3(size, 0, 0), new Vector3(size * 2, 0, 0), color, duration);
        Debug.DrawRay(position - new Vector3(0, size, 0), new Vector3(0, size * 2, 0), color, duration);
        Debug.DrawRay(position - new Vector3(0, 0, size), new Vector3(0, 0, size * 2), color, duration);
    }
}
