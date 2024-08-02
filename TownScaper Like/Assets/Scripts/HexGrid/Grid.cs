using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid 
{
    public static float cellSize;
    public static int maxY;
    public static float cellHeight;


    //hex顶点
    public List<Vertex> hexList = new List<Vertex>();
    
    //所有的vertex
    public List<Vertex> vertexList = new List<Vertex>();

    public List<Triangle> triangleList = new List<Triangle>();

    public static List<Edge> edgeList = new List<Edge>();
   

    public List<Quad> quadList = new List<Quad>();

    public List<Quad> subQuadList = new List<Quad>();
    public static List<Edge> subQuadEdgeList = new List<Edge>();

    public static List<Vertex> midVertexList = new List<Vertex>();
    public static List<Vertex> centerVertexList = new List<Vertex>();

    public List<CubeVertex> cubeVertexList = new List<CubeVertex>();
    public List<CubeQuad> cubeQuadList = new List<CubeQuad>();

    public List<Vertex> subQuadVertexList= new List<Vertex>();

    public Grid(int _maxRadius,float _cellsize,int _relaxTimes,int _maxY,float _cellHeight)
    {

        cellSize = _cellsize;
        maxY = _maxY; cellHeight = _cellHeight;
        hexList.AddRange(GenerateHexVertex(_maxRadius));
        
        vertexList.AddRange(hexList);

        triangleList.AddRange(GenerateTriangle(_maxRadius));

        //Debug.Log(edgeList.Count);
        
        GenerateTriangle();
        vertexList.AddRange(midVertexList);
        vertexList.AddRange(centerVertexList);
        DivideSubQuad();


        
        

        RelaxSubQuad(_relaxTimes);


        subQuadVertexList = GetAllSubQudList();

        GenerateCubeVertex(maxY);

        GenerateCube(maxY);
    }

    public List<Vertex> GetAllSubQudList()
    {
        HashSet<Vertex> subQuadVertexHashSet = new HashSet<Vertex>();
        List<Vertex> result = new List<Vertex>();
        foreach (var sq in subQuadList)
        {
            subQuadVertexHashSet.Add(sq.a);
            subQuadVertexHashSet.Add(sq.b);
            subQuadVertexHashSet.Add(sq.c);
            subQuadVertexHashSet.Add(sq.d);
        }
        foreach(var v in subQuadVertexHashSet)
        {
            result.Add(v);
        }
        return result;
    }

    //1.生成Hex顶点
    private List<Vertex> GenerateHexVertex(int _maxRadius)
    {
        List<Vertex> result = new List<Vertex>();
        List<Coord> coordList = new List<Coord>();
        //先生成所有coord;
        for(int r = 0; r < _maxRadius; ++r)
        {
            coordList.AddRange(Coord.GenerateSingleRingCoord(r));
        }

        foreach(var c in coordList)
        {
            result.Add(Vertex.CreateVertex(c));
        }


        return result;
    }

    //2.生成三角形


    public List<Triangle> GenerateTriangle(int _maxRadius)
    {
        List<Triangle> result = new List<Triangle>();
        for(int i = 0; i < _maxRadius; ++i)
        {
            result.AddRange(Triangle.GenerateSingleRingTriangle(i, hexList));
        }
        return result;
    }


    //判断边是否已经存在了，
    //  如果存在，那么存在的边的owners+1，把存在的边返回
    //  如果不存在，那么新建一个边，owners+1,返回新建的边
    public static Edge GetEdge(Vertex _a,Vertex _b,List<Edge> _es,Triangle _t=null)
    {
        Edge newEdge = new Edge(_a, _b);
        foreach(var e in _es)
        {
            if (newEdge.Equals(e))
            {
                if (_t != null)
                {
                    e.AddTriangleOwner(_t);
                    
                }
                    
                return e;
            }
        }
        //新边

        edgeList.Add(newEdge);
        if (_t != null)
            newEdge.AddTriangleOwner(_t);
        return newEdge;



    }
    //3.随机合并三角形
    public void GenerateTriangle()
    {
        List<Edge> removeEdge = new List<Edge>();

        HashSet<int> randomIndex = new HashSet<int>();
        while (randomIndex.Count != edgeList.Count)
        {
            randomIndex.Add(Random.Range(0, edgeList.Count));
        }

        foreach(var i in randomIndex)
        {
            if (edgeList[i].TriangleOwnersNumber() == 2)
            {
                //1.删除edge
                List<Triangle> tOwner = edgeList[i].GetTriangleOwners();
               
                removeEdge.Add(edgeList[i]);

                 List<Edge> otherEdge = new List<Edge>();
                HashSet<Vertex> hashVertex = new HashSet<Vertex>();
                //2.遍历edge的相邻三角形获取所有的非公共边
                foreach (var t in tOwner)
                {
                    //获取非公共边
                    List<Edge> oes=t.GetOtherEdge(edgeList[i]);
                    //oes.Add(edgeList[i]);
                    foreach (var _oe in oes)
                    {
                        _oe.EraseTriangleOwner(t); hashVertex.Add(_oe.b);
                        hashVertex.Add(_oe.a);
                        
                    }
                    otherEdge.AddRange(oes);
                }
             
                //3.用非公共边创建Quad
                Quad quad = new Quad(hashVertex,Grid.edgeList);
                quadList.Add(quad);

                //4.在非公共边上加入Quad并且删除三角形
                foreach(var oe in otherEdge)
                {
                    oe.AddQuadOwner(quad);
                }

                foreach(var et in tOwner)
                {
                    triangleList.Remove(et);
                    //删除三角形的同时也要删除centerVertexList当中的节点
                    centerVertexList.Remove(et.centerVertex);
                }


            }
        }
        foreach(var e in removeEdge)
        {
            edgeList.Remove(e);
        }
    }

    //4.生成SubQud
    public void DivideSubQuad()
    {
        //1.三角形
        subQuadList.AddRange(DivideTriangle());
        //2.四边形
        subQuadList.AddRange(DivideQuad());
    }
    private List<Quad> DivideTriangle()
    {
        List<Quad> result = new List<Quad>();
        foreach(var t in triangleList)
        {
            result.Add(new Quad(t.ab.midVertex, t.a, t.ca.midVertex, t.centerVertex, subQuadEdgeList));
            result.Add(new Quad(t.ca.midVertex, t.c, t.bc.midVertex, t.centerVertex, subQuadEdgeList));
            result.Add(new Quad(t.bc.midVertex, t.b, t.ab.midVertex, t.centerVertex, subQuadEdgeList));
            

        }
        return result;
    }
    private List<Quad> DivideQuad()
    {
        List<Quad> result = new List<Quad>();
        foreach (var q in quadList)
        {
            result.Add(new Quad(q.ab.midVertex, q.a, q.da.midVertex, q.centerVertex, subQuadEdgeList));
            result.Add(new Quad(q.bc.midVertex, q.b, q.ab.midVertex, q.centerVertex, subQuadEdgeList));
            result.Add(new Quad(q.cd.midVertex, q.c, q.bc.midVertex, q.centerVertex, subQuadEdgeList));
            result.Add(new Quad(q.da.midVertex, q.d, q.cd.midVertex, q.centerVertex, subQuadEdgeList));
        }
        return result;
    }

    //Relax SubQuad,使之平滑
    public void RelaxSubQuad(int _relaxTimes)
    {
        for (int i = 0; i < _relaxTimes; ++i)
        {
            foreach (var sq in subQuadList)
            {
                sq.CaculateRelax();
            }
        }
        foreach (var v in vertexList)
        {
            v.Relax();
        }
    }

    //5.生成三维空间点
    void GenerateCubeVertex(int _maxY)
    {
        foreach(var v in subQuadVertexList)
        {
            for(int i = 0; i < _maxY; ++i)
            {
                CubeVertex vc = new CubeVertex(v, i);
                cubeVertexList.Add(vc);
                v.yVertexList.Add(vc);
            }
        }
    }

    public void GenerateCube(int _maxY)
    {
        foreach(var sq in subQuadList)
        {
            for(int i = 0; i < _maxY-1; ++i)
            {
                CubeQuad cq = new CubeQuad(sq, i);
                cubeQuadList.Add(cq);
                sq.yQuadList.Add(cq);
            }
        }
    }
}
