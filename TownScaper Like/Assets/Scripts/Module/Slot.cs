using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] public List<Module> possibleModule=new List<Module>();
    public GameObject module;
    public CubeQuad cubeQuad;
    public Material material;

    public bool isReset=false;

    ///GirdManger
    ///---Slot
    ///------GameObject("Module")
    ///---------MeshFilter
    ///---------MeshRenderer
    ///
    public Stack<List<Module>> preModule = new Stack<List<Module>>();
    public void Awake()
    {
        module = new GameObject("Module", typeof(MeshFilter), typeof(MeshRenderer));
        module.transform.SetParent(transform);
        module.transform.localPosition = Vector3.zero;
    }


    public void Initialized(CubeQuad _cq,ModuleLibrary _ml, Material _m)
    {

        cubeQuad = _cq;

        this.cubeQuad.slot = this;
        ResetSlot(_ml);
        material =_m;

    }

    public void ResetSlot(ModuleLibrary _ml)
    {
        possibleModule = _ml.GetModule(cubeQuad.bits).ConvertAll(x=>x);
        isReset = true;
    }

    private Slot() {; }
    private void RotateModule(Mesh _mesh,int _rotateTimes)
    {
        if (_rotateTimes != 0)
        {
            Vector3[] vertexs = _mesh.vertices;
            for(int i = 0; i < vertexs.Length; ++i)
            {
                vertexs[i] = Quaternion.AngleAxis(90 * _rotateTimes, Vector3.up) * vertexs[i];
            }
            _mesh.vertices = vertexs;

        }
    }

    public void FlipModule(Mesh _mesh,bool _flip)
    {
        if (_flip)
        {
            Vector3[] vertexs = _mesh.vertices;
            for (int i = 0; i < vertexs.Length; ++i)
            {
                vertexs[i] =new Vector3(-vertexs[i].x, vertexs[i].y, vertexs[i].z);

            }
            _mesh.vertices = vertexs;
            _mesh.triangles = _mesh.triangles.Reverse().ToArray();
        }
    }

    public void ReShapeModule(Mesh _mesh, CubeQuad _cubeQuad)
    {
        Vector3[] vertexs = _mesh.vertices;
        Quad quad = _cubeQuad.quad;
        for(int i = 0; i < vertexs.Length; ++i)
        {
            Vector3 ad_x = Vector3.Lerp(quad.a.currentWorldPosition, quad.d.currentWorldPosition,(vertexs[i].x+0.5f));
            Vector3 bc_x = Vector3.Lerp(quad.b.currentWorldPosition, quad.c.currentWorldPosition, (vertexs[i].x + 0.5f));
            vertexs[i] = Vector3.Lerp(ad_x, bc_x, (vertexs[i].z + 0.5f))
                        + Vector3.up * vertexs[i].y * Grid.cellHeight - quad.centerVertex.currentWorldPosition;//这里还有疑问
        }
        _mesh.vertices = vertexs;
    }
    public void UpdateModule(Module _module)
    {
        module.GetComponent<MeshFilter>().mesh = _module.mesh;
        RotateModule(module.GetComponent<MeshFilter>().mesh, _module.rotation);
        FlipModule(module.GetComponent<MeshFilter>().mesh,_module.flip);
        ReShapeModule(module.GetComponent<MeshFilter>().mesh, cubeQuad);
        module.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        module.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        
        module.GetComponent<MeshRenderer>().material = material;
    }

    public void Collapse(int i)
    {
        possibleModule = new List<Module>() { possibleModule[i] };

        isReset = false;

    }
}
