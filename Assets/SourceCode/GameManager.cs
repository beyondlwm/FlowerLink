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
    public CellPos(int nCellIndex)
    {
        x = nCellIndex % CGameManager.Instance.m_pConfig.m_nMapWidth;
        y = nCellIndex / CGameManager.Instance.m_pConfig.m_nMapWidth;
    }
}

public struct Record
{
    public string name;
    public float time;
}

public class CGameManager : CSingleton<CGameManager>
{
    public CGameConfig m_pConfig;
    public List<List<Cell>> m_cellList = new List<List<Cell>>();
    public MyDictionary<int, MyDictionary<int, Cell>> m_cellTypeMap = new MyDictionary<int, MyDictionary<int, Cell>>();
    public float m_fCellSize;
    public Cell m_availableSrcCell;
    public Cell m_availableDstCell;
    public float m_fGameTime;
    public string m_strPlayerName;
    public List<Record> m_recordList = new List<Record>();
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
    private GameObject cellPrefab = null;

    public bool FindAvailableLink(ref Cell srcCell, ref Cell dstCell)
    {
        List<CellPos> corners = new List<CellPos>();
        var iterType = m_cellTypeMap.GetEnumerator();
        while(iterType.MoveNext())
        {
            var iterCell = iterType.Current.Value.GetEnumerator();
            while (iterCell.MoveNext())
            {
                var nextIterCell = iterCell;
                if (nextIterCell.MoveNext())
                {
                    CellPos a = new CellPos(iterCell.Current.Key);
                    CellPos b = new CellPos(nextIterCell.Current.Key);
                    if (CanLink(a, b, ref corners))
                    {
                        srcCell = iterCell.Current.Value;
                        dstCell = nextIterCell.Current.Value;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void RebuildCellList()
    {
        List<int> cellIndexList = new List<int>();
        List<Cell> cellList = new List<Cell>();
        var iterType = m_cellTypeMap.GetEnumerator();
        while (iterType.MoveNext())
        {
            var iterCell = iterType.Current.Value.GetEnumerator();
            while (iterCell.MoveNext())
            {
                cellIndexList.Add(iterCell.Current.Key);
                cellList.Add(iterCell.Current.Value);
            }
        }
        m_cellTypeMap.Clear();
        cellIndexList = cellIndexList.OrderBy(a => System.Guid.NewGuid()).ToList();
        for(int i = 0; i < cellIndexList.Count; ++i)
        {
            int x = cellIndexList[i] % CGameManager.Instance.m_pConfig.m_nMapWidth;
            int y = cellIndexList[i] / CGameManager.Instance.m_pConfig.m_nMapWidth;
            cellList[i].m_x = x;
            cellList[i].m_y = y;
            cellList[i].gameObject.GetComponent<RectTransform>().position = new Vector3((x + 0.5f) * m_fCellSize, (y + 0.5f) * m_fCellSize, 0);
            Debug.Assert(m_cellList[y][x] != null);
            m_cellList[y][x] = cellList[i];
            m_cellTypeMap[cellList[i].m_nType][cellIndexList[i]] = cellList[i];
        }
    }

    public void RemoveCell(Cell cell)
    {
        m_cellList[cell.m_y][cell.m_x] = null;
        int nPosIndex = cell.m_x + cell.m_y * CGameManager.Instance.m_pConfig.m_nMapWidth;
        Debug.Assert(m_cellTypeMap.ContainsKey(cell.m_nType) && m_cellTypeMap[cell.m_nType].ContainsKey(nPosIndex));
        m_cellTypeMap[cell.m_nType].Remove(nPosIndex);
        if (m_cellTypeMap[cell.m_nType].Count == 0)
        {
            m_cellTypeMap.Remove(cell.m_nType);
        }
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

    public void GenerateCells()
    {
        float fCellSize = Mathf.Min((float)Screen.width / CGameManager.Instance.m_pConfig.m_nMapWidth,
            (float)Screen.height / CGameManager.Instance.m_pConfig.m_nMapHeight);
        CGameManager.Instance.m_fCellSize = fCellSize;
        cellPrefab = Resources.Load("Prefab/Cell") as GameObject;
        Debug.Assert(cellPrefab != null);
        cellPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(fCellSize, fCellSize);
        GameObject cellPanel = GameObject.Find("/Canvas/CellPanel");
        Debug.Assert(cellPanel != null);
        CGameManager.Instance.m_cellList.Clear();
        int nImageCount = CGameManager.Instance.m_pConfig.m_ImageList.Count;
        int nTotalCellCount = CGameManager.Instance.m_pConfig.m_nMapHeight *
            CGameManager.Instance.m_pConfig.m_nMapWidth;
        Debug.Assert(nTotalCellCount % 2 == 0);
        List<int> randomTypeList = new List<int>();
        int nHalfCellCount = nTotalCellCount / 2;
        for (int i = 0; i < nHalfCellCount; ++i)
        {
            randomTypeList.Add(i % nImageCount);
        }
        randomTypeList = randomTypeList.OrderBy(a => System.Guid.NewGuid()).ToList();
        List<int> anotherHalfList = randomTypeList.OrderBy(a => System.Guid.NewGuid()).ToList();
        randomTypeList.AddRange(anotherHalfList);
        int nCounter = 0;
        for (int i = 0; i < CGameManager.Instance.m_pConfig.m_nMapHeight; ++i)
        {
            List<Cell> cellList = new List<Cell>();
            CGameManager.Instance.m_cellList.Add(cellList);
            for (int j = 0; j < CGameManager.Instance.m_pConfig.m_nMapWidth; ++j)
            {
                GameObject newCellObj = UnityEngine.Object.Instantiate(cellPrefab) as GameObject;
                newCellObj.GetComponent<RectTransform>().position = new Vector3((j + 0.5f) * fCellSize, (i + 0.5f) * fCellSize, 0);
                newCellObj.transform.SetParent(cellPanel.transform);
                Cell newCell = newCellObj.GetComponent<Cell>();
                newCell.m_nType = randomTypeList[nCounter];
                newCell.m_x = j;
                newCell.m_y = i;
                newCellObj.GetComponent<Image>().sprite = CGameManager.Instance.m_pConfig.m_ImageList[newCell.m_nType];
                int nPosIndex = j + i * CGameManager.Instance.m_pConfig.m_nMapWidth;
                Debug.Assert(!CGameManager.Instance.m_cellTypeMap.ContainsKey(newCell.m_nType) || !CGameManager.Instance.m_cellTypeMap[newCell.m_nType].ContainsKey(nPosIndex));
                MyDictionary<int, Cell> value;
                if (!CGameManager.Instance.m_cellTypeMap.TryGetValue(newCell.m_nType, out value))
                {
                    value = new MyDictionary<int, Cell>();
                    CGameManager.Instance.m_cellTypeMap.Add(newCell.m_nType, value);
                }
                value.Add(nPosIndex, newCell);
                cellList.Add(newCell);
                ++nCounter;
            }
        }
        while (!CGameManager.Instance.FindAvailableLink(ref CGameManager.Instance.m_availableSrcCell, ref CGameManager.Instance.m_availableDstCell))
        {
            CGameManager.Instance.RebuildCellList();
        }
    }

    public void Restart()
    {
        for (int i = 0; i < m_cellList.Count; ++i)
        {
            for(int j = 0; j < m_cellList[i].Count; ++j)
            {
                if (m_cellList[i][j] != null)
                {
                    GameObject.Destroy(m_cellList[i][j].gameObject);
                }
            }
        }
        m_cellList.Clear();
        m_cellTypeMap.Clear();
        m_availableSrcCell = null;
        m_availableDstCell = null;
        m_fGameTime = 0;
        SrcCell = null;
        GenerateCells();
    }
}
