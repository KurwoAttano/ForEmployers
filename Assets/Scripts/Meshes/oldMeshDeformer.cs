using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class oldMeshDeformer : MonoBehaviour
{
    public Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;

    public float force = 10f;
    public float forceOffset = 0.1f;
    public float springForce = 20f;
    public float damping = 5f;

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;

        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
            displacedVertices[i] = originalVertices[i];
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        //!
        /*if (Input.GetMouseButton(0))
        {
            HandleInput();
        }*/
        HandleInput();

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();

        //!!!
        GetComponent<MeshCollider>().sharedMesh = deformingMesh;
    }

    ContactPoint currentContact;
    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            HandleInput();
            currentContact = contact;
            //Debug.Log("contact");
        }
    }

    //void HandleInput(ContactPoint contact)
    void HandleInput()
    {
        //Ray inputRay = new Ray(transform.position, Vector3.down / 1.8f);
        //Ray inputRay = new Ray(transform.position, transform.position - contact.point / 1.8f);
        Ray inputRay = new Ray(transform.position, currentContact.point - transform.position);

        //Debug.DrawRay(Vector3.zero, contact.point, Color.cyan, 1); //!
        //Debug.DrawRay(contact.point - transform.position, Vector3.up, Color.cyan, 1); //!
        //Debug.DrawRay(transform.position, currentContact.point - transform.position, Color.cyan, 1); //!
        // Добиться что б в дебаге куан линия была из точки косания, а потом перенести решение выше

        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit, 0.55f))
        {
            oldMeshDeformer deformer = this;
            if (deformer)
            {
                Vector3 point = hit.point - transform.position;
                point += (hit.point - transform.position) * forceOffset;

                Vector3 rPoint = new Vector3(point.x, point.y, point.z);
                Vector3 rot = new Vector3(
                    transform.rotation.eulerAngles.x * Mathf.Deg2Rad, 
                    transform.rotation.eulerAngles.y * Mathf.Deg2Rad, 
                    transform.rotation.eulerAngles.z * Mathf.Deg2Rad);
                rPoint = new Vector3(
                    rPoint.x,
                    rPoint.y * Mathf.Cos(rot.x) + rPoint.z * Mathf.Sin(rot.x),
                    rPoint.y * Mathf.Sin(rot.x) + rPoint.z * Mathf.Cos(rot.x));
                rPoint = new Vector3(
                    rPoint.x * Mathf.Cos(rot.y) + rPoint.z * Mathf.Sin(rot.y),
                    rPoint.y,
                    rPoint.x * Mathf.Sin(rot.y) + rPoint.z * Mathf.Cos(rot.y));
                rPoint = new Vector3(
                    rPoint.x * Mathf.Cos(rot.z) + rPoint.y * Mathf.Sin(rot.z),
                    rPoint.x * Mathf.Sin(rot.z) + rPoint.y * Mathf.Cos(rot.z),
                    rPoint.z);

                deformer.AddDeformingForce(rPoint, force);

                //Debug.Log(point); //!
                DrawDebugMarker(transform.position, Color.green); //!
                DrawDebugMarker(transform.position + rPoint, Color.magenta);

                /*Vector3 angle = new Vector3(0, -0.5f, 0);
                Vector3 rotateto = new Vector3(Mathf.PI/ 180 * 90, Mathf.PI / 180 * 0, Mathf.PI / 180 * 0);
                DrawDebugMarker(transform.position + angle, Color.yellow);
                //x
                angle = new Vector3(
                    angle.x, 
                    angle.y * Mathf.Cos(rotateto.x) + angle.z * Mathf.Sin(rotateto.x),
                    angle.y * Mathf.Sin(rotateto.x) + angle.z * Mathf.Cos(rotateto.x));
                //y
                angle = new Vector3(
                    angle.x * Mathf.Cos(rotateto.y) + angle.z * Mathf.Sin(rotateto.y),
                    angle.y,
                    angle.x * Mathf.Sin(rotateto.y) + angle.z * Mathf.Cos(rotateto.y));
                //z
                angle = new Vector3(
                    angle.x * Mathf.Cos(rotateto.z) + angle.y * Mathf.Sin(rotateto.z), 
                    angle.x * Mathf.Sin(rotateto.z) + angle.y * Mathf.Cos(rotateto.z),
                    angle.z);
                DrawDebugMarker(transform.position + angle, Color.magenta);*/
            }
        }
    }
    /*void HandleInput() // РАБОТАЕТ
    {
        //Ray inputRay = new Ray(transform.position, Vector3.down / 1.8f);
        Ray inputRay = new Ray(transform.position, Vector3.down / 1.8f);

        //Debug.DrawRay(transform.position, Vector3.down / 1.8f, Color.white, 1); //!

        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit, 0.55f)) {
            MeshDeformer deformer = GetComponent<MeshDeformer>();
            if (deformer) {
                Vector3 point = hit.point - transform.position;
                Debug.DrawRay(hit.point, new Vector3(0, 0.5f, 0.5f), Color.blue, 1); //!
                point += (transform.position - hit.point) * forceOffset;
                Debug.DrawRay(transform.position, new Vector3(0, 0.5f, 0.5f), Color.green, 1); //!
                deformer.AddDeformingForce(point, force);
            }
        }
    }*/

    public void AddDeformingForce(Vector3 point, float force) {
        for (int i = 0; i < displacedVertices.Length; i++)
            AddForceToVertex(i, point, force);
    }
    void AddForceToVertex(int i, Vector3 point, float force) {
        /*// Длинна от косания к вертексу
        float temp = Vector3.Distance(point, displacedVertices[i]);
        // Направление и длинна силы
        Vector3 pointToVertex = -point / temp + displacedVertices[i] / (0.5f + temp);
        // Ослабленная сила
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        // Скорость из силы
        float velocity = attenuatedForce * Time.deltaTime;
        // Направление на скорость
        vertexVelocities[i] += pointToVertex.normalized * velocity;*/

        //Смещение = a * (1 - b)

        //Vector3 pointToVertex = 

        // Длинна от косания к вертексу
        float temp = Vector3.Distance(point, displacedVertices[i]);
        // Направление и длинна силы
        Vector3 pointToVertex = -point / temp + displacedVertices[i] / (0.5f + temp);
        // Ослабленная сила
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        // Скорость из силы
        float velocity = attenuatedForce * Time.deltaTime;
        // Направление на скорость
        vertexVelocities[i] += pointToVertex.normalized * velocity;
    }
    void UpdateVertex(int i) {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * Time.deltaTime;
    }

    void DrawDebugMarker(Vector3 position, Color color, int duration = 1, float size = 0.05f)
    {
        Debug.DrawRay(position - new Vector3(size, 0, 0), new Vector3(size * 2, 0, 0), color, duration);
        Debug.DrawRay(position - new Vector3(0, size, 0), new Vector3(0, size * 2, 0), color, duration);
        Debug.DrawRay(position - new Vector3(0, 0, size), new Vector3(0, 0, size * 2), color, duration);
    }
}
