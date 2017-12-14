using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour {

    private GameObject cellPrefab = null;
	// Use this for initialization
	void Start () {
        GenerateCells();
    }
	
	// Update is called once per frame
	void Update () {
    }

    private void GenerateCells()
    {
        float fCellSize = Mathf.Min((float)Screen.width / CGameManager.Instance.m_pConfig.m_nMapWidth,
            (float)Screen.height / CGameManager.Instance.m_pConfig.m_nMapHeight);

        cellPrefab = Resources.Load("Prefab/Cell") as GameObject;
        Debug.Assert(cellPrefab != null);
        cellPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(fCellSize, fCellSize);
        GameObject canvas = GameObject.Find("/Canvas");
        Debug.Assert(canvas != null);
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
                GameObject newCellObj = Object.Instantiate(cellPrefab) as GameObject;
                newCellObj.GetComponent<RectTransform>().position = new Vector3((j + 0.5f) * fCellSize, (i + 0.5f) * fCellSize, 0);
                newCellObj.transform.SetParent(canvas.transform);
                Cell newCell = newCellObj.GetComponent<Cell>();
                newCell.m_nType = randomTypeList[nCounter];
                newCell.m_x = j;
                newCell.m_y = i;
                newCellObj.GetComponent<Image>().sprite = CGameManager.Instance.m_pConfig.m_ImageList[newCell.m_nType];
                cellList.Add(newCell);
                ++nCounter;
            }
        }
    }
}
