using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex 
{
    public Coord coord;
    public Vector3 initeWorldPosition;
    public Vector3 currentWorldPosition;
    public Vector3 offset = Vector3.zero;
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
}
