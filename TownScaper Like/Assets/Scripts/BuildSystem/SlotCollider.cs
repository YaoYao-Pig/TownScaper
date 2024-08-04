using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotCollider : MonoBehaviour
{
    public CubeVertex cubeVertex;

    public string GetSlotName(CubeVertex _cv)
    {
        return  "SlotCollider_" + Grid.cubeVertexList.IndexOf(_cv);
    }
    public void CreateSlotCollider(CubeVertex _cv)
    {

        string name = GetSlotName(_cv);
        cubeVertex = _cv;

        GameObject slotCollider = new GameObject(name);
        slotCollider.transform.SetParent(transform);
        slotCollider.transform.localPosition = _cv.worldPosition;

        //Vector3 b = Vector3.up * (cubeVertex.y) * Grid.cellHeight;

        GameObject top = new GameObject("TOP",typeof(MeshCollider),typeof(SlotTop));
        Mesh _m = _cv.vertex.CreateCursorMesh();

        

        ReverseTriangle(_m);
        top.GetComponent<MeshCollider>().sharedMesh = _m;

        top.layer = LayerMask.NameToLayer("SlotCollider");
        //top.GetComponent<SlotSlideType>().type = SlotSlideType.SlotType.TOP;
        top.transform.SetParent(slotCollider.transform);
        top.transform.localPosition += Vector3.up * (cubeVertex.y) * Grid.cellHeight  + Vector3.up * Grid.cellHeight/2;



        //slide.GetComponent<SlotSlideType>().type = SlotSlideType.SlotType.SLIDE;
        int i = 0;
        foreach(var sq in cubeVertex.vertex.selfQuadList)
        {
            GameObject slide = new GameObject("SLIDE"+i++.ToString(), typeof(MeshCollider), typeof(SlotSlide));

            slide.GetComponent<MeshCollider>().sharedMesh = CreateCursorMesh(_cv,sq,out slide.GetComponent<SlotSlide>().neighor);
            slide.layer = LayerMask.NameToLayer("SlotCollider");
            slide.transform.SetParent(slotCollider.transform);
            slide.transform.localPosition += Vector3.up * (cubeVertex.y) * Grid.cellHeight + Vector3.up * (Grid.cellHeight / 2 - Grid.cellHeight);
        }




        GameObject bottom = new GameObject("BOTTOM", typeof(MeshCollider), typeof(SlotBottom));
        bottom.GetComponent<MeshCollider>().sharedMesh = _cv.vertex.CreateCursorMesh();

        bottom.layer = LayerMask.NameToLayer("SlotCollider");
        bottom.transform.SetParent(slotCollider.transform);
        bottom.transform.localPosition += Vector3.up * (cubeVertex.y) * Grid.cellHeight + Vector3.up * (Grid.cellHeight / 2 - Grid.cellHeight);





       // bottom.GetComponent<SlotSlideType>().type = SlotSlideType.SlotType.BOTTOM;

    }

    private void ReverseTriangle(Mesh _mesh)
    {
        Vector3[] normals = _mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }
        _mesh.normals = normals;
        for (int i = 0; i < _mesh.subMeshCount; i++)
        {
            int[] triangles = _mesh.GetTriangles(i);
            for (int j = 0; j < triangles.Length; j += 3)
            {
                int temp = triangles[j];
                triangles[j] = triangles[j + 1];
                triangles[j + 1] = temp;
            }
            _mesh.SetTriangles(triangles, i);
        }

    }
    public void DestroyCollider(CubeVertex _cv)
    {
        Destroy(transform.Find(GetSlotName(_cv)).gameObject);
        Resources.UnloadUnusedAssets();
    }

    public Mesh CreatTopAndBottomMesh(CubeVertex _cv)
    {
        Mesh result = new Mesh();
        List<Vector3> meshVertecs = new List<Vector3>();
        List<int> meshIndexs = new List<int>();

        meshVertecs.Add(_cv.vertex.currentWorldPosition);
        foreach (var sq in _cv.vertex.selfQuadList)
        {
            Vertex pre;
            Vertex next;
            _cv.vertex.GetPreAndNextVertex(sq, _cv.vertex, out pre, out next);


            meshVertecs.Add((pre.currentWorldPosition + _cv.vertex.currentWorldPosition) / 2.0f);
            meshVertecs.Add(sq.centerVertex.currentWorldPosition);
            meshVertecs.Add((next.currentWorldPosition + _cv.vertex.currentWorldPosition) / 2.0f);

            meshIndexs.AddRange(new List<int>(){
                               0,meshVertecs.Count-3,meshVertecs.Count -2,
                               0,meshVertecs.Count - 2 ,meshVertecs.Count-1});
        }
        result.vertices = meshVertecs.ToArray();
        result.triangles = meshIndexs.ToArray();
        return result;
    }

    public Mesh CreateCursorMesh(CubeVertex _cv,Quad _sq, out CubeVertex _neighbor)
    {
        Mesh result = new Mesh();
        List<Vector3> meshVertecs = new List<Vector3>();
        List<int> meshIndexs = new List<int>();

        meshVertecs.Add(_cv.vertex.currentWorldPosition);

        Vertex pre;
        Vertex next;
        Vertex other;
            _cv.vertex.GetPreAndNextVertex(_sq, _cv.vertex, out pre, out next,out other);
        _neighbor = other.yVertexList[_cv.y];
            Vector3 a_low = (pre.currentWorldPosition+ _cv.vertex.currentWorldPosition)/2.0f;
            Vector3 b_low = (next.currentWorldPosition+ _cv.vertex.currentWorldPosition)/2.0f;
            Vector3 a_high =a_low+ Vector3.up * Grid.cellHeight;
            Vector3 b_high= b_low+ Vector3.up * Grid.cellHeight;

            meshVertecs.Add(a_low);
            meshVertecs.Add(b_low);
            meshVertecs.Add(a_high);
            meshVertecs.Add(b_high);

            meshIndexs.AddRange(new List<int>(){
                               meshVertecs.Count-4,meshVertecs.Count-3,meshVertecs.Count -2,
                               meshVertecs.Count-4,meshVertecs.Count - 2 ,meshVertecs.Count-1});
        result.vertices = meshVertecs.ToArray();
        result.triangles = meshIndexs.ToArray();
        return result;
    }

}

public class SlotSlideType:MonoBehaviour
{
    public enum SlotType{
        TOP,BOTTOM,SLIDE
    }
    public SlotType type;

}

public class SlotTop : MonoBehaviour
{

}
public class SlotBottom : MonoBehaviour
{

}
public class SlotSlide : MonoBehaviour
{
    public CubeVertex neighor;
}