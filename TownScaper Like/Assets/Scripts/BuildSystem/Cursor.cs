using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public void UpdateCursor(CubeVertex _target,CubeVertex _select ,RaycastType _type,GameObject _g)
    {
        
        GetComponent<MeshFilter>().mesh.Clear();

        Debug.Log(_type);
        transform.localPosition = Vector3.zero;
        if (_type == RaycastType.GROUND)
        {
            Vertex vertex = _target.vertex;

            GetComponent<MeshFilter>().mesh = vertex.CreateCursorMesh();
            // transform.localPosition = target.worldPosition;
        }
        else if(_type== RaycastType.TOP)
        {
            Vertex vertex = _select.vertex;

            GetComponent<MeshFilter>().mesh = vertex.CreateCursorMesh();
            transform.position = Vector3.up*Grid.cellHeight*((float)_select.y+0.6f);
        }
        else if (_type == RaycastType.SILDE)
        {
            Vertex vertex = _select.vertex;

            GetComponent<MeshFilter>().mesh = _g.GetComponent<MeshCollider>().sharedMesh;
            transform.position= Vector3.up * Grid.cellHeight * ((float)_select.y/2f);
        }

    }
}
public enum RaycastType
{
    GROUND,TOP,SILDE,BOTTOM, NONE
}