using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rbPoint : MonoBehaviour {

    public Vector2 Pos { get; private set; }
    public bool Removed { get; set; }

    private rbController m_controller;

    private void Awake()
    {
        Pos = new Vector2(transform.position.x, transform.position.y);
        m_controller = FindObjectOfType<rbController>();
    }

    private void OnMouseDown()
    {
        /*
         * TODO: 
         * 1) make this point dissappear
         * 2) Update CH, score appropriately
         */
    }

    private void OnMouseEnter()
    {
        /* 
         * TODO: provide overview of what surface would
         * be added to score if this object was selected.
         */      
    }

    private void OnMouseExit()
    {
        /*
         * TODO: stop drawing score update
         */
    }
}
