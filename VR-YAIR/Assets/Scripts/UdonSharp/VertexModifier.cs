using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VertexModifier : UdonSharpBehaviour
{
    public Mesh MeshToEdit;
    public MeshFilter MeshFiltertoedit;
    public MeshFilter ThisMeshFilter;
    public Vector3[] vertices;
    public GameObject PosicionTop;
    public Collider ThisCollider;

    public bool tryGetMesh;
    public float OnTriggerDeltaTime = 0;


    void Update()
    {
        OnTriggerDeltaTime += Time.deltaTime;
        //if(tryGetMesh)
        //{
        //    MeshToEdit = MeshFiltertoedit.mesh;
        //    vertices = MeshToEdit.vertices;
        //    tryGetMesh = false;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.GetComponent<MeshFilter>())
        //{
        //MeshFiltertoedit = other.GetComponent<MeshFilter>();
        //tryGetMesh = true;
        //}
        Debug.Log(string.Format("Lo toco {0}", other.name));
    }

    //private void OnTriggerStay(Collider other)
    //{
    //if (OnTriggerDeltaTime > 0.1f)
    //{
    //    //Debug.Log(string.Format("time: {0}", OnTriggerDeltaTime));
    //    //    for(var i = 0; i < vertices.Length; i++)
    //    //    {
    //    //        if (ThisMeshFilter.mesh.bounds.Contains(vertices[i]))
    //    //        {
    //    //            vertices[i] = ThisMeshFilter.mesh.bounds.ClosestPoint(vertices[i]);//new Vector3(vertices[i].x, PosicionTop.transform.position.y, vertices[i].z);
    //    //        }
    //    //    }
    //    OnTriggerDeltaTime = 0;
    //    //    MeshToEdit.vertices = vertices;
    //    //    MeshToEdit.RecalculateBounds();
    //}
    //}

}
