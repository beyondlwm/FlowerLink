using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("closebtn").GetComponent<Button>().onClick.AddListener(delegate () {
            gameObject.SetActive(false);
            CGameManager.Instance.Restart();
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show(int nRank)
    {
        gameObject.SetActive(true);
        Debug.Assert(CGameManager.Instance.m_cellTypeMap.Count == 0);
        GameObject.Find("resultlable").GetComponent<Text>().text = string.Format("{0} 耗时 {1:F2} 秒", CGameManager.Instance.m_strPlayerName, CGameManager.Instance.m_fGameTime);
        GameObject.Find("recordlable").GetComponent<Text>().text = nRank == -1 ? 
            "很遗憾您没有取得名次，请再接再厉！" :
            string.Format("创造了新的纪录！第 {0} 名！", nRank);
    }
}
