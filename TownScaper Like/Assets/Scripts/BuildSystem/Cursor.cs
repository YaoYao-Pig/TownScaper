using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public void UpdateCursor(CubeVertex target)
    {
        GetComponent<MeshFilter>().mesh.Clear();
        Vertex vertex = target.vertex;

        GetComponent<MeshFilter>().mesh=vertex.CreateCursorMesh();
       // transform.localPosition = target.worldPosition;


    }
}
