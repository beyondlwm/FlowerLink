using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public struct CellPos
{
    public int x;
    public int y;
    public CellPos(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}
public class CGameManager : CSingleton<CGameManager>
{
    public CGameConfig m_pConfig;
    public List<List<Cell>> m_cellList = new List<List<Cell>>();
    public Cell SrcCell { get { return m_srcCell; } set {
            if (m_srcCell != null)
            {
                m_srcCell.gameObject.GetComponent<Image>().color = new Color(1, 1, 1);
            }
            m_srcCell = value;
            if (m_srcCell != null)
            {
                m_srcCell.gameObject.GetComponent<Image>().color = new Color(0, 0, 1);
            }
        } }
    private Cell m_srcCell;

    public void RemoveCell(Cell cell)
    {
        m_cellList[cell.m_y][cell.m_x] = null;
        GameObject.Destroy(cell.gameObject);
    }

    public bool CanLink(CellPos srcPos, CellPos dstPos, ref List<CellPos> corners)
    {
        corners.Clear();
        bool bRet = LinkDirect(srcPos, dstPos) ||
            LinkWith1Corner(srcPos, dstPos, ref corners) ||
            LinkWith2Corner(srcPos, dstPos, ref corners);
        return bRet;
    }

    private bool LinkDirect(CellPos srcPos, CellPos dstPos)
    {
        bool bRet = false;
        if (srcPos.x == dstPos.x && srcPos.y != dstPos.y)
        {
            bRet = true;
            if (Math.Abs(srcPos.y - dstPos.y) > 1)
            {
                int minY = srcPos.y < dstPos.y ? srcPos.y : dstPos.y;
                int maxY = srcPos.y < dstPos.y ? dstPos.y : srcPos.y;
                for (int curY = minY + 1; curY < maxY; ++curY)
                {
                    if (m_cellList[curY][srcPos.x] != null)
                    {
                        bRet = false;
                        break;
                    }
                }
            }
        }
        else if(srcPos.x != dstPos.x && srcPos.y == dstPos.y)
        {
            bRet = true;
            if (Math.Abs(srcPos.x - dstPos.x) > 1)
            {
                int minX = srcPos.x < dstPos.x ? srcPos.x : dstPos.x;
                int maxX = srcPos.x < dstPos.x ? dstPos.x : srcPos.x;
                for (int curX = minX + 1; curX < maxX; ++curX)
                {
                    if (m_cellList[srcPos.y][curX] != null)
                    {
                        bRet = false;
                        break;
                    }
                }
            }
        }
        return bRet;
    }

    private bool LinkWith1Corner(CellPos srcPos, CellPos dstPos, ref List<CellPos> corners)
    {
        bool bRet = false;
        CellPos corner = new CellPos(srcPos.x, dstPos.y);
        if (srcPos.x != dstPos.x && srcPos.y != dstPos.y)
        {
            if (m_cellList[corner.y][corner.x] == null)
            {
                bRet = LinkDirect(srcPos, corner) && LinkDirect(corner, dstPos);
            }
            if (!bRet)
            {
                corner.x = dstPos.x;
                corner.y = srcPos.y;
                if (m_cellList[corner.y][corner.x] == null)
                {
                    bRet = LinkDirect(srcPos, corner) && LinkDirect(corner, dstPos);
                }
            }
        }
        if (bRet)
        {
            corners.Add(corner);
        }
        return bRet;
    }

    private bool LinkWith2Corner(CellPos srcPos, CellPos dstPos, ref List<CellPos> corners)
    {
        bool bRet = false;
        CellPos corner1 = new CellPos(0,0);
        CellPos corner2 = new CellPos(0,0);
        for(int i = srcPos.x + 1; i < m_pConfig.m_nMapWidth; ++i)
        {
            if (m_cellList[srcPos.y][i] != null)
            {
                break;
            }
            else
            {
                corner1.x = i;
                corner1.y = srcPos.y;
                if (LinkWith1Corner(corner1, dstPos, ref corners))
                {
                    bRet = true;
                    break;
                }
            }
        }
        if(!bRet)
        {
            for (int i = srcPos.x - 1; i >= 0; --i)
            {
                if (m_cellList[srcPos.y][i] != null)
                {
                    break;
                }
                else
                {
                    corner1.x = i;
                    corner1.y = srcPos.y;
                    if (LinkWith1Corner(corner1, dstPos, ref corners))
                    {
                        bRet = true;
                        break;
                    }
                }
            }
        }
        if (!bRet)
        {
            for (int i = srcPos.y + 1; i < m_pConfig.m_nMapHeight; ++i)
            {
                if (m_cellList[i][srcPos.x] != null)
                {
                    break;
                }
                else
                {
                    corner1.x = srcPos.x;
                    corner1.y = i;
                    if (LinkWith1Corner(corner1, dstPos, ref corners))
                    {
                        bRet = true;
                        break;
                    }
                }
            }
        }
        if (!bRet)
        {
            for (int i = srcPos.y - 1; i >= 0; --i)
            {
                if (m_cellList[i][srcPos.x] != null)
                {
                    break;
                }
                else
                {
                    corner1.x = srcPos.x;
                    corner1.y = i;
                    if (LinkWith1Corner(corner1, dstPos, ref corners))
                    {
                        bRet = true;
                        break;
                    }
                }
            }
        }
        if(bRet)
        {
            corners.Insert(0, corner1);
        }
        return bRet;
    }
}
