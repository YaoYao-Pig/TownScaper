using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : Shape
{
    public Vertex a;
    public Vertex b;
    public Vertex c;

    public Edge ab;
    public Edge bc;
    public Edge ca;

    

    

    private static int globalId=0;
    
    private List<Vertex> SortEdge(Vertex _a, Vertex _b, Vertex _c)
    {
        //根据x和z坐标排序
        List<Vertex> result = new List<Vertex>();
         
        if (_a.initeWorldPosition.x > _b.initeWorldPosition.x && _a.initeWorldPosition.x > _c.initeWorldPosition.x)
        {
            result.Add(_a);
            if (_b.initeWorldPosition.z > _c.initeWorldPosition.z)
            {
                result.Add(_b);
                result.Add(_c);
            }
            else
            {
                result.Add(_c);
                result.Add(_b);
            }
        }
        else if(_b.initeWorldPosition.x > _a.initeWorldPosition.x && _b.initeWorldPosition.x > _c.initeWorldPosition.x)
        {
            result.Add(_b);
            if (_a.initeWorldPosition.z > _c.initeWorldPosition.z)
            {
                result.Add(_a);
                result.Add(_c);
            }
            else
            {
                result.Add(_c);
                result.Add(_a);
            }
        }
        else
        {
            result.Add(_c);
            if (_a.initeWorldPosition.z > _b.initeWorldPosition.z)
            {
                result.Add(_a);
                result.Add(_b);
            }
            else
            {
                result.Add(_b);
                result.Add(_a);

            }
        }
        return result;
    }
    
    public Triangle(Vertex _a,Vertex _b,Vertex _c)
    {
        List<Vertex> vs = SortEdge(_a, _b, _c);
        a = vs[0]; b = vs[1]; c = vs[2];
        vertexs.Add(a);
        vertexs.Add(b);
        vertexs.Add(c);

        //判断边是否已经存在了，
        //  如果存在，那么存在的边的owners+1，把存在的边返回
        //  如果不存在，那么新建一个边，owners+1,返回新建的边
        ab = Grid.GetEdge(a, b,Grid.edgeList, this);
        bc = Grid.GetEdge(b, c,Grid.edgeList, this);
        ca = Grid.GetEdge(c, a,Grid.edgeList, this);
                                


        centerVertex = Vertex.CreateCenterVertex(vertexs);
        Grid.centerVertexList.Add(centerVertex);


        id = globalId++;


    }

    public List<Edge> GetOtherEdge(Edge _e)
    {
        if (_e.Equals(ab)) { return new List<Edge>() { bc, ca }; }
        else if (_e.Equals(bc)) { return new List<Edge>() { ca, ab }; }
        else if (_e.Equals(ca)) { return new List<Edge>() { ab, bc }; }
        else { throw new System.Exception("Triangle::GetOtherEdge:No Match Edge"); }
    }


    public static List<Triangle> GenerateSingleRingTriangle(int _radius,List<Vertex> _hexList)
    {
        List<Triangle> result = new List<Triangle>();

        List<Vertex> inner = Vertex.GrabRing(_radius - 1 , _hexList);
        List<Vertex> outer = Vertex.GrabRing(_radius, _hexList);

        for (int i = 0; i < 6; ++i)
        {
            for(int j = 0; j < _radius; ++j)
            {
                //两个外圈，一个内圈
                Vertex a = outer[i * _radius + j];
                Vertex b = outer[(i * _radius + j + 1) % outer.Count];
                Vertex c = inner[(i * (_radius - 1) + j) % inner.Count];

                result.Add(new Triangle(a, b, c));
                if (j > 0)
                {
                    Vertex d = inner[i * (_radius - 1) + j - 1];
                    result.Add(new Triangle(a, c, d));
                }
            }
        }
        return result;
    }


}
