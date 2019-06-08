using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Tile
{
    public enum GateDirection
    {
        Left, Right
    }

    public GateDirection gateDirection;
    public bool open;

    private Animator animator;
    private int isOpenVar, isLeftGateVar;
    
    void Start()
    {
        base.Start();
        
        var gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gameManager.gates.Add(this);

        animator = GetComponent<Animator>();
        type = TileMap.TileType.Gate;

        isOpenVar = Animator.StringToHash("isOpen");
        isLeftGateVar = Animator.StringToHash("isLeftGate");
        
        animator.SetBool(isOpenVar, false);
        if (gateDirection == GateDirection.Left)
        {
            animator.SetBool(isLeftGateVar, true);
            animator.Play("gate_L_closed");
        }
        else
        {
            animator.SetBool(isLeftGateVar, false);
            animator.Play("gate_R_closed");
        }
    }

    void Update()
    {
        
    }

    public void Open()
    {
        open = true;
        collider.enabled = false;
        
        animator.SetBool(isOpenVar, true);
    }
}
