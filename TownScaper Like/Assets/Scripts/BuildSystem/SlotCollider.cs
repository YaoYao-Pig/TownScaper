using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlotCollider : MonoBehaviour
{

    public string GetSlotName(CubeVertex _cv)
    {
        return  "SlotCollider_" + Grid.cubeVertexList.IndexOf(_cv);
    }
    public void CreateSlotCollider(CubeVertex _cv)
    {

        string name = GetSlotName(_cv);
        

        GameObject slotCollider = new GameObject(name,typeof(SlotRoot));
        slotCollider.GetComponent<SlotRoot>().cubeVertex = _cv;
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
        top.transform.localPosition += Vector3.up * (slotCollider.GetComponent<SlotRoot>().cubeVertex.y) * Grid.cellHeight  + Vector3.up * Grid.cellHeight/2;




        //Slide
        //slide.GetComponent<SlotSlideType>().type = SlotSlideType.SlotType.SLIDE;

        List<Quad> quads = slotCollider.GetComponent<SlotRoot>().cubeVertex.vertex.selfQuadList;
        //int a = quads.Count-1;

        /*        for(int i=0;i< quads.Count; ++i)
                {
                    GameObject slide = new GameObject("SLIDE" + i++.ToString(), typeof(MeshCollider), typeof(SlotSlide));
                    if (i == 0)
                        slide.GetComponent<MeshCollider>().sharedMesh = CreateSlideMesh(quads[0],quads[2], _cv, out slide.GetComponent<SlotSlide>().neighor);
                    else if (i == 1)
                        slide.GetComponent<MeshCollider>().sharedMesh = CreateSlideMesh(quads[2],quads[1], _cv, out slide.GetComponent<SlotSlide>().neighor);
                    else if (i == 2)
                        slide.GetComponent<MeshCollider>().sharedMesh = CreateSlideMesh(quads[1], quads[3], _cv, out slide.GetComponent<SlotSlide>().neighor);
                    else if (i == 3)
                        slide.GetComponent<MeshCollider>().sharedMesh = CreateSlideMesh(quads[3], quads[0], _cv, out slide.GetComponent<SlotSlide>().neighor);
                    slide.layer = LayerMask.NameToLayer("SlotCollider");
                    slide.transform.SetParent(slotCollider.transform);
                    slide.transform.localPosition += Vector3.up * (slotCollider.GetComponent<SlotRoot>().cubeVertex.y) * Grid.cellHeight + Vector3.up * (Grid.cellHeight / 2 - Grid.cellHeight);
                }*/
        int i = 0;
        foreach (var sq in slotCollider.GetComponent<SlotRoot>().cubeVertex.vertex.selfQuadList)
        {
            GameObject slide = new GameObject("SLIDE" + i++.ToString(), typeof(MeshCollider), typeof(SlotSlide));

            slide.GetComponent<MeshCollider>().sharedMesh = CreateSlideMesh( _cv, sq,out slide.GetComponent<SlotSlide>().neighor);
            //CreateSlideMesh(_cv,sq,out slide.GetComponent<SlotSlide>().neighor);
            slide.layer = LayerMask.NameToLayer("SlotCollider");
            slide.transform.SetParent(slotCollider.transform);
            slide.transform.localPosition += Vector3.up * (slotCollider.GetComponent<SlotRoot>().cubeVertex.y) * Grid.cellHeight + Vector3.up * (Grid.cellHeight / 2 - Grid.cellHeight);
        }




        GameObject bottom = new GameObject("BOTTOM", typeof(MeshCollider), typeof(SlotBottom));
        bottom.GetComponent<MeshCollider>().sharedMesh = _cv.vertex.CreateCursorMesh();

        bottom.layer = LayerMask.NameToLayer("SlotCollider");
        bottom.transform.SetParent(slotCollider.transform);
        bottom.transform.localPosition += Vector3.up * (slotCollider.GetComponent<SlotRoot>().cubeVertex.y) * Grid.cellHeight + Vector3.up * (Grid.cellHeight / 2 - Grid.cellHeight);





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


    public Mesh CreateSlideMesh(Quad _a,Quad _b, CubeVertex _cv, out CubeVertex _neighbor)
    {
        Mesh result = new Mesh();
        List<Vector3> meshVertecs = new List<Vector3>();
        List<int> meshIndexs = new List<int>();

        Vector3 a_centerPosition = _a.centerVertex.currentWorldPosition;
        Vector3 b_centerPosition = _b.centerVertex.currentWorldPosition;

        //找到两个公共点

        List<Vertex> both = _a.GetBothPoint(_b);
        Vector3 bothMid= (both[0].currentWorldPosition+ both[1].currentWorldPosition)/ 2.0f;

        //neighbor
        if(Vector3.Distance(both[0].currentWorldPosition, _cv.vertex.currentWorldPosition) <= 0.001f)
        {
            _neighbor = both[1].yVertexList[_cv.y];
        }
        else
        {
            _neighbor = both[0].yVertexList[_cv.y];
        }


        Vector3 a_centerPosition_high = a_centerPosition + Vector3.up * Grid.cellHeight;
        Vector3 b_centerPosition_high = b_centerPosition + Vector3.up * Grid.cellHeight;
        Vector3 bothMid_high = bothMid + Vector3.up * Grid.cellHeight;

        meshVertecs.Add(a_centerPosition);
        meshVertecs.Add(a_centerPosition_high);
        meshVertecs.Add(bothMid_high);
        meshVertecs.Add(bothMid);
        meshVertecs.Add(bothMid_high);
        meshVertecs.Add(b_centerPosition_high);

        meshIndexs.AddRange(new List<int>(){
                               0,1,2,
                               0,2 ,3,
                                3,2,5,
                                3,5,4});


        result.vertices = meshVertecs.ToArray();
        result.triangles = meshIndexs.ToArray();
        return result;
    }


    public Mesh CreateSlideMesh(CubeVertex _cv,Quad _sq, out CubeVertex _neighbor)
    {
        Mesh result = new Mesh();
        List<Vector3> meshVertecs = new List<Vector3>();
        List<int> meshIndexs = new List<int>();

        meshVertecs.Add(_cv.vertex.currentWorldPosition);

        Vertex pre;
        Vertex next;
        Vertex other;
        _cv.vertex.GetPreAndNextVertex(_sq, _cv.vertex, out pre, out next,out other);
        _neighbor = pre.yVertexList[_cv.y]; //other.yVertexList[_cv.y];
        Vector3 a_low = (pre.currentWorldPosition+ _cv.vertex.currentWorldPosition)/2.0f;
        Vector3 b_low = (next.currentWorldPosition+ _cv.vertex.currentWorldPosition)/2.0f;
        Vector3 a_high =a_low+ Vector3.up * Grid.cellHeight;
        Vector3 b_high= b_low+ Vector3.up * Grid.cellHeight;
        meshVertecs.Add(a_low);
        meshVertecs.Add(a_high);
        meshVertecs.Add(b_high);
        meshVertecs.Add(b_low);

        meshIndexs.AddRange(new List<int>(){
                               0,1,2,
                               0,2 ,3});
        result.vertices = meshVertecs.ToArray();
        result.triangles = meshIndexs.ToArray();
        return result;
    }

}

public class SlotRoot:MonoBehaviour
{
    public CubeVertex cubeVertex;

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