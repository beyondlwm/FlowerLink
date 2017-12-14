﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public int m_nType { get; set; }
    public int m_x { get; set; }
    public int m_y { get; set; }
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPointerClick(PointerEventData data)
    {
        if (CGameManager.Instance.SrcCell == null)
        {
            CGameManager.Instance.SrcCell = this;
        }
        else
        {
            CellPos srcCellPos = new CellPos(CGameManager.Instance.SrcCell.m_x, CGameManager.Instance.SrcCell.m_y);
            CellPos dstCellPos = new CellPos(this.m_x, this.m_y);
            List<CellPos> corners = new List<CellPos>();
            bool bCanLink = CGameManager.Instance.SrcCell.m_nType == m_nType &&
                CGameManager.Instance.CanLink(srcCellPos, dstCellPos, ref corners);
            if(bCanLink)
            {
                CGameManager.Instance.m_cellList[srcCellPos.y][srcCellPos.x] = null;
                CGameManager.Instance.m_cellList[dstCellPos.y][dstCellPos.x] = null;
                CGameManager.Instance.RemoveCell(CGameManager.Instance.SrcCell);
                CGameManager.Instance.RemoveCell(this);
            }
            CGameManager.Instance.SrcCell = null;
        }
    }
}