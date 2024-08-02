using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module
{
    public string name;
    public Mesh mesh;
    public int rotation;
    public bool flip;

    public Module(string _name,Mesh _mesh,int _r,bool _flip)
    {
        name=_name; mesh = _mesh;rotation = _r;flip = _flip;
    }
}
