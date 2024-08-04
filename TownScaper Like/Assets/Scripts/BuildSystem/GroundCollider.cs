using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    public void CreateGroundCollider(List<Quad> _groundQuad)
    {
        foreach(var q in _groundQuad)
        {
            Vector3[] meshVertexs = new Vector3[]
            {
                q.a.currentWorldPosition,
                q.b.currentWorldPosition,
                q.c.currentWorldPosition,
                q.d.currentWorldPosition,
            };
            int[] meshIndexs = new int[]
            {
                0,1,2,
                0,2,3
            };

            Mesh mesh = new Mesh();
            mesh.vertices = meshVertexs;
            mesh.triangles = meshIndexs;

            GameObject goundCollider = new GameObject("GoundCollider_" + _groundQuad.IndexOf(q).ToString(),
                typeof(MeshCollider),typeof(GroundColliderQuad));
            goundCollider.transform.SetParent(transform);

            goundCollider.GetComponent<MeshCollider>().sharedMesh = mesh;
            goundCollider.GetComponent<GroundColliderQuad>().subQuad = q;
            goundCollider.layer = LayerMask.NameToLayer("GoundCollider");
        }
    }
}

public class GroundColliderQuad : MonoBehaviour
{
    public Quad subQuad;
}
