using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    public int m_nType { get; set; }
    public int m_x { get; set; }
    public int m_y { get; set; }
    private float m_fScaleTime = 0;
    public bool m_bShine { get { return _bShine; } set {
            if(_bShine != value)
            {
                _bShine = value;
                if (!_bShine)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                    m_fScaleTime = 0;
                }
            }
        } }
    private bool _bShine;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (m_bShine)
        {
            m_fScaleTime += Time.deltaTime;
            float fFinalScale = 1.0f - (Mathf.Sin((m_fScaleTime % 1.0f) * Mathf.PI)) * 0.25f;
            transform.localScale = new Vector3(fFinalScale, fFinalScale, fFinalScale);
        }
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
                CGameManager.Instance.m_availableSrcCell.m_bShine = false;
                CGameManager.Instance.m_availableDstCell.m_bShine = false;
                CGameManager.Instance.m_cellList[srcCellPos.y][srcCellPos.x] = null;
                CGameManager.Instance.m_cellList[dstCellPos.y][dstCellPos.x] = null;
                CGameManager.Instance.RemoveCell(CGameManager.Instance.SrcCell);
                CGameManager.Instance.RemoveCell(this);
                // Detect if we need to rebuild the whole map
                if (CGameManager.Instance.SrcCell == CGameManager.Instance.m_availableSrcCell ||
                    CGameManager.Instance.SrcCell == CGameManager.Instance.m_availableDstCell ||
                    this == CGameManager.Instance.m_availableSrcCell ||
                    this == CGameManager.Instance.m_availableDstCell)
                {
                    if (CGameManager.Instance.m_cellTypeMap.Count > 0)
                    {
                        while (!CGameManager.Instance.FindAvailableLink(ref CGameManager.Instance.m_availableSrcCell, ref CGameManager.Instance.m_availableDstCell))
                        {
                            CGameManager.Instance.RebuildCellList();
                        }
                    }
                    else
                    {
                        int nRank = -1;
                        //change the record.
                        if (CGameManager.Instance.m_recordList.Count == 0)
                        {
                            nRank = 1;
                            Record newRecord = new Record();
                            newRecord.name = CGameManager.Instance.m_strPlayerName;
                            newRecord.time = CGameManager.Instance.m_fGameTime;
                            CGameManager.Instance.m_recordList.Insert(0, newRecord);
                            PlayerPrefs.SetString(string.Format("{0}_name", 0), newRecord.name);
                            PlayerPrefs.SetFloat(string.Format("{0}_time", 0), newRecord.time);
                            PlayerPrefs.Save();
                        }
                        else
                        {
                            for (int j = 0; j < 10; ++j)
                            {
                                if (j >= CGameManager.Instance.m_recordList.Count)
                                {
                                    nRank = j + 1;
                                    Record newRecord = new Record();
                                    newRecord.name = CGameManager.Instance.m_strPlayerName;
                                    newRecord.time = CGameManager.Instance.m_fGameTime;
                                    CGameManager.Instance.m_recordList.Add(newRecord);
                                    PlayerPrefs.SetString(string.Format("{0}_name", j), newRecord.name);
                                    PlayerPrefs.SetFloat(string.Format("{0}_time", j), newRecord.time);
                                    PlayerPrefs.Save();
                                    break;
                                }
                                else if (CGameManager.Instance.m_fGameTime < CGameManager.Instance.m_recordList[j].time)
                                {
                                    Record newRecord = new Record();
                                    newRecord.name = CGameManager.Instance.m_strPlayerName;
                                    newRecord.time = CGameManager.Instance.m_fGameTime;
                                    nRank = j + 1;
                                    CGameManager.Instance.m_recordList.Insert(j, newRecord);
                                    if (CGameManager.Instance.m_recordList.Count > 10)
                                    {
                                        CGameManager.Instance.m_recordList.RemoveRange(10, CGameManager.Instance.m_recordList.Count - 10);
                                    }
                                    for (int k = 0; k < CGameManager.Instance.m_recordList.Count; ++k)
                                    {
                                        PlayerPrefs.SetString(string.Format("{0}_name", k), CGameManager.Instance.m_recordList[k].name);
                                        PlayerPrefs.SetFloat(string.Format("{0}_time", k), CGameManager.Instance.m_recordList[k].time);
                                    }
                                    PlayerPrefs.Save();
                                    break;
                                }
                            }
                        }
                        GameObject winPanel = GameObject.Find("/Canvas").transform.Find("WinPanel").gameObject;
                        Debug.Assert(winPanel != null);
                        winPanel.GetComponent<WinPanel>().Show(nRank);
                    }
                }
            }
            CGameManager.Instance.SrcCell = this;
        }
    }
}
