using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour {
    private Text m_pTimeLable;
    private Button m_pTipBtn;
    private Button m_pRebuildBtn;
    private Button m_pRestartBtn;
	// Use this for initialization
	void Start () {
        m_pTimeLable = GameObject.Find("Timelable").GetComponent<Text>();
        m_pTipBtn = GameObject.Find("tipbtn").GetComponent<Button>();
        m_pTipBtn.onClick.AddListener(delegate () {
            if (CGameManager.Instance.m_availableDstCell != null &&
                CGameManager.Instance.m_availableSrcCell != null)
            {
                if (!CGameManager.Instance.m_availableSrcCell.m_bShine)
                {
                    CGameManager.Instance.m_availableSrcCell.m_bShine = true;
                    CGameManager.Instance.m_availableDstCell.m_bShine = true;
                    CGameManager.Instance.m_fGameTime += 5;
                }
            }
        });
        m_pRebuildBtn = GameObject.Find("rebuildbtn").GetComponent<Button>();
        m_pRebuildBtn.onClick.AddListener(delegate ()
        {
            CGameManager.Instance.RebuildCellList();
            while (!CGameManager.Instance.FindAvailableLink(ref CGameManager.Instance.m_availableSrcCell, ref CGameManager.Instance.m_availableDstCell))
            {
                CGameManager.Instance.RebuildCellList();
            }
            CGameManager.Instance.m_fGameTime += 15;
        });
        m_pRestartBtn = GameObject.Find("restartbtn").GetComponent<Button>();
        m_pRestartBtn.onClick.AddListener(delegate ()
        {
            CGameManager.Instance.Restart();
        });
    }

    // Update is called once per frame
    void Update () {
        if (CGameManager.Instance.m_cellTypeMap.Count > 0)
        {
            CGameManager.Instance.m_fGameTime += Time.deltaTime;
            m_pTimeLable.text = string.Format("{0:F2}", CGameManager.Instance.m_fGameTime);
        }
    }
}
