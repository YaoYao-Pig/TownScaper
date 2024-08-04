using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex 
{
    public Coord coord;
    public Vector3 initeWorldPosition;
    public Vector3 currentWorldPosition;
    public Vector3 offset = Vector3.zero;
    public List<CubeVertex> yVertexList = new List<CubeVertex>();
    public List<Quad> selfQuadList = new List<Quad>();
    public void Relax()
    {
        currentWorldPosition=initeWorldPosition+offset;
    }

    
    
    public static Vertex CreateVertex(Coord _c)
    {
        return new Vertex(_c, CaculateWorldPosition(_c));
    }

    public static Vertex CreateMidVertex(Vertex _a,Vertex _b)
    {
        return new Vertex(new Coord(0, 0, 0), (_a.initeWorldPosition + _b.initeWorldPosition) / 2.0f);
    }

    public static Vertex CreateCenterVertex(List<Vertex> _vs)
    {
        Vector3 result = Vector3.zero;
        foreach(var v in _vs)
        {
            result += v.initeWorldPosition;
        }
        result /= _vs.Count;
        return new Vertex(new Coord(0, 0, 0), result);
    }

    private Vertex(Coord _c,Vector3 _w)
    {
        coord = _c;initeWorldPosition = _w;
        currentWorldPosition = new Vector3(_w.x, _w.y, _w.z);
    }

    public static Vector3 CaculateWorldPosition(Coord _c)
    {
        return new Vector3(_c.q * Mathf.Sqrt(3) / 2, 0, -(float)_c.r - ((float)_c.q / 2)) * 2 * Grid.cellSize;
    }

    //获取某一个radius的所有vertex
    public static List<Vertex> GrabRing(int _radius,List<Vertex> _vertexList)
    {
        if (_radius <= 0)
        {
            return _vertexList.GetRange(0, 1);
        }
        else
        {
            return _vertexList.GetRange(_radius * (_radius - 1) * 3 + 1, _radius * 6);
        }
    }

    public override bool Equals(object obj)
    {
        return this == (Vertex)obj;
    }

    public override int GetHashCode()
    {
        return coord.GetHashCode();
    }

    public static bool operator==(Vertex _a,Vertex _b)
    {
        if(Mathf.Abs(_a.initeWorldPosition.x-_b.initeWorldPosition.x)<=0.001&&
           Mathf.Abs(_a.initeWorldPosition.y - _b.initeWorldPosition.y) <= 0.001&&
           Mathf.Abs(_a.initeWorldPosition.z - _b.initeWorldPosition.z) <= 0.001)
        {
            return _a.coord.Equals(_b.coord);
        }
        return false;
    }

    public static bool operator !=(Vertex _a, Vertex _b)
    {
        return !_a.coord.Equals(_b.coord);
    }
    public Mesh CreateCursorMesh()
    {
        Mesh result=new Mesh();
        List<Vector3> meshVertecs = new List<Vector3>();
        List<int> meshIndexs = new List<int>();

        meshVertecs.Add(this.currentWorldPosition);
        foreach (var sq in selfQuadList)
        {
            Vertex pre;
            Vertex next;
            GetPreAndNextVertex(sq,this,out pre,out next);

            
            meshVertecs.Add((pre.currentWorldPosition+ this.currentWorldPosition) /2.0f);
            meshVertecs.Add(sq.centerVertex.currentWorldPosition);
            meshVertecs.Add((next.currentWorldPosition+ this.currentWorldPosition) /2.0f);

            meshIndexs.AddRange(new List<int>(){
                               0,meshVertecs.Count-3,meshVertecs.Count -2,
                               0,meshVertecs.Count - 2 ,meshVertecs.Count-1});
        }
        result.vertices = meshVertecs.ToArray();
        result.triangles = meshIndexs.ToArray();
        return result;
    }


    private bool IsEqual(Vertex _a)
    {
        if (Vector3.Distance(this.currentWorldPosition, _a.currentWorldPosition) < 0.001f)
        {
            return true;
        }
        return false;
    }
    public void GetPreAndNextVertex(Quad _sq,Vertex _v,out Vertex _pre, out Vertex _next)
    {
        _pre = _sq.vertexs[_sq.vertexs.Count - 1];
        _next = null;
        int i = 0;
        foreach (var v in _sq.vertexs)
        {
            if (v.IsEqual(_v)) {
                _next = _sq.vertexs[(i+1)% _sq.vertexs.Count];
                break;
            }
            else
            {
                i++;
                _pre = v;
            }
            
        }
        //if (_next == null) throw new System.Exception("Vertex::GetPreAndNextVertex");
    }

    public void GetPreAndNextVertex(Quad _sq, Vertex _v, out Vertex _pre, out Vertex _next,out Vertex _other)
    {
        _pre = _sq.vertexs[_sq.vertexs.Count - 1];
        _next = null;
        _other = null;
        int i = 0;
        foreach (var v in _sq.vertexs)
        {
            if (v.IsEqual(_v))
            {
                _next = _sq.vertexs[(i + 1) % _sq.vertexs.Count];
                _other = _sq.vertexs[(i + 2) % _sq.vertexs.Count];
                break;
            }
            else
            {
                i++;
                _pre = v;
            }

        }
        //if (_next == null) throw new System.Exception("Vertex::GetPreAndNextVertex");
    }
}

public class CubeVertex
{
    public Vector3 worldPosition;
    public bool isActive;
    public List<CubeQuad> cubeQuadList = new List<CubeQuad>();
    public Vertex vertex;
    public int y;

    public CubeVertex(Vertex _v,int _y)
    {
        vertex = _v;
        isActive = false;
        y = _y;
        worldPosition = _v.currentWorldPosition + Vector3.up * (Grid.cellSize * _y);
    }


}