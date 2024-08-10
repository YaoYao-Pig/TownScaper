using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCpllapse : MonoBehaviour
{
    private GameManger gameManager;
    private GridManager gridManager;
    private ModuleLibrary moduleLibrary;


    public List<Slot> resetSlot = new List<Slot>();
    public List<Slot> curCollapseSlots = new List<Slot>();

    private Slot currentColapseSlot;

    private Stack<Slot> propagateSlotStack = new Stack<Slot>();

    public Slot currentPropagateSlot;


    public Stack<Slot> collapseSlotStack = new Stack<Slot>();
    public Stack<List<Slot>> collapseSlotStackList = new Stack<List<Slot>>();

   

    private void Awake()
    {
        gameManager = GetComponentInParent<GameManger>();
        gridManager = gameManager.gridManager;
        moduleLibrary = Instantiate(gridManager.moduleLibrary);

    }

    public void WFC()
    {
        Reset();
        CollapseAndPropagate();
        UpdateModule();
    }

    private void Reset()
    {
        while (resetSlot.Count > 0)
        {
            Slot curSlot = resetSlot[0];
            resetSlot.RemoveAt(0);
            CubeQuad[] neighors = curSlot.cubeQuad.neighbors;
            foreach(var cq in neighors)
            {
                if (cq != null && cq.isActive && !cq.slot.isReset)
                {
                    cq.slot.ResetSlot(moduleLibrary);
                    resetSlot.Add(cq.slot);
                    if (!curCollapseSlots.Contains(cq.slot))
                    {
                        curCollapseSlots.Add(cq.slot);
                    }
                    Debug.Log("重置");

                }
            }
        }
    }
    private void CollapseAndPropagate()
    {

        while (curCollapseSlots.Count > 0)
        {
            while (propagateSlotStack.Count == 0)
            {
                GetCollapseSlot();
                Collapse();
                Propagate();
            }

        }
    }

    private void GetCollapseSlot()
    {
        int minPossible = curCollapseSlots[0].possibleModule.Count;
        foreach (var slot in curCollapseSlots)
        {
            if (slot.possibleModule.Count < minPossible)
            {
                minPossible = slot.possibleModule.Count;
            }
        }


        bool findFirst = false;
        foreach (var slot in curCollapseSlots)
        {
            if (slot.possibleModule.Count == minPossible)
            {
                if (!findFirst)
                {
                    currentColapseSlot = slot; findFirst = true;
                }
                else if (currentColapseSlot.cubeQuad.index > slot.cubeQuad.index)
                {
                    currentColapseSlot = slot;
                }
            }

        }
    }
    private void Collapse()
    {
        bool backtrackAvailable = (currentColapseSlot.possibleModule.Count > 1);
        if (backtrackAvailable)
        {
            //存储当前一步用于坍缩的状态
            collapseSlotStack.Push(currentColapseSlot);
            collapseSlotStackList.Push(curCollapseSlots);

            foreach(Slot slot in gridManager.slotList)
            {
                slot.preModule.Push(slot.possibleModule);
            }
        }



        System.Random r = new System.Random(currentColapseSlot.cubeQuad.index);
        int choseModule = r.Next()%currentColapseSlot.possibleModule.Count;
        currentColapseSlot.Collapse(choseModule);
        curCollapseSlots.Remove(currentColapseSlot);
        propagateSlotStack.Push(currentColapseSlot);


        //移除当前选择
        if (backtrackAvailable)
        {
            List<Module> modules=currentColapseSlot.preModule.Pop();
            modules.Remove(currentColapseSlot.possibleModule[0]);
            currentColapseSlot.preModule.Push(modules);
        }

    }

    public void ConstrainPossibility(CubeQuad[] neighbors,Dictionary<int,HashSet<string>> possibleSockets,int i)//int i：代表前后左右
    {
        List<Module> possibleModules = neighbors[i].slot.possibleModule.ConvertAll(x => x);
        foreach(var module in neighbors[i].slot.possibleModule)
        {
            if (!possibleSockets[i].Contains(module.sockets[Module.neighorSocket[i]])){
                possibleModules.Remove(module);
                if (!propagateSlotStack.Contains(neighbors[i].slot)){
                    propagateSlotStack.Push(neighbors[i].slot);
                }
            }
        }
        neighbors[i].slot.possibleModule = possibleModules;
    }

    public void Propagate()
    {
        currentPropagateSlot = propagateSlotStack.Pop();
        Dictionary<int, HashSet<string>> possibleSockets = new Dictionary<int, HashSet<string>>();


        //前后左右上下都有可能与什么类型的块链接
        for(int i = 0; i < 6; ++i)
        {
            possibleSockets[i] = new HashSet<string>();
            foreach(var module in currentPropagateSlot.possibleModule)
            {
                foreach(var socket in NeighborDictionary.neighborDictionary[module.sockets[i]])
                {
                    possibleSockets[i].Add(socket);
                }
            }
        }

        CubeQuad[] neighbors = currentPropagateSlot.cubeQuad.neighbors;
        for(int i = 0; i < 6; ++i)
        {
            if (neighbors[i] != null && neighbors[i].isActive)
            {
                ConstrainPossibility(neighbors, possibleSockets, i);
                if (neighbors[i].slot.possibleModule.Count == 0)
                {
                    BackTrack();break;
                }
            }
        }
    }

    //回溯
    public void BackTrack()
    {
        collapseSlotStack.Pop();
        collapseSlotStackList.Pop();
        foreach(var slot in gridManager.slotList)
        {
            slot.possibleModule = slot.preModule.Pop();

        }
        propagateSlotStack.Clear();
        Collapse();

    }

    public void ClearBacktrackStack()
    {
        collapseSlotStack.Clear();
        collapseSlotStackList.Clear();
        foreach (var slot in gridManager.slotList)
        {
            slot.preModule.Clear();

        }
        collapseSlotStack=null;
        collapseSlotStackList = null;
    }

    private void UpdateModule()
    {
        ClearBacktrackStack();
       foreach (var slot in gridManager.slotList)
        {
            slot.UpdateModule(slot.possibleModule[0]);
        }
    }




}
