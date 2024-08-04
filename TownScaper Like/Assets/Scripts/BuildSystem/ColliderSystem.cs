using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSystem : MonoBehaviour
{
    public GameManger gameManger;
    public GroundCollider groundCollider;
    private void Awake()
    {
        gameManger = GetComponentInParent<GameManger>();
        groundCollider = GetComponentInChildren<GroundCollider>();


    }
    private void Start()
    {
        groundCollider.CreateGroundCollider(gameManger.gridManager.GetGrid().subQuadList);
    }

}
