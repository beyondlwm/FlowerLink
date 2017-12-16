using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {

    private Button m_pStartBtn;
    private GameObject m_pScorePanel;
    private InputField m_strNameInputFiled;
    // Use this for initialization
    void Start() {
        GameObject pStartBtn = transform.Find("startbtn").gameObject;
        Debug.Assert(pStartBtn != null);
        m_pStartBtn = pStartBtn.GetComponent<Button>();
        m_pStartBtn.onClick.AddListener(delegate () {
            gameObject.SetActive(false);
            CGameManager.Instance.GenerateCells();
            m_pScorePanel = transform.parent.Find("ScorePanel").gameObject;
            float fScorePanelWidth = Screen.width - CGameManager.Instance.m_fCellSize * CGameManager.Instance.m_pConfig.m_nMapWidth;
            m_pScorePanel.GetComponent<RectTransform>().sizeDelta = new Vector2(fScorePanelWidth, Screen.height);
            m_pScorePanel.GetComponent<RectTransform>().position = new Vector3(CGameManager.Instance.m_fCellSize * CGameManager.Instance.m_pConfig.m_nMapWidth, 0, 0);
            m_pScorePanel.SetActive(true);
        });
        m_strNameInputFiled = transform.Find("InputField").gameObject.GetComponent<InputField>();
        m_strNameInputFiled.onValueChanged.AddListener(delegate (string textValue)
        {
            m_pStartBtn.gameObject.SetActive(textValue.Length != 0);
            CGameManager.Instance.m_strPlayerName = textValue;
        });
        float fLastTime = 0;
        CGameManager.Instance.m_recordList.Clear();
        GameObject recordSubPanel = GameObject.Find("/Canvas/MainPanel/RecordPanel/Panel");
        Debug.Assert(recordSubPanel != null); 
        for (int i = 0; i < 10; ++i)
        {
            string key = string.Format("{0}_name", i);
            string strText = "  空缺中";
            if (PlayerPrefs.HasKey(key))
            {
                Record record = new Record();
                record.name = PlayerPrefs.GetString(key);
                record.time = PlayerPrefs.GetFloat(string.Format("{0}_time", i));
                Debug.Assert(record.time >= fLastTime);
                fLastTime = record.time;
                CGameManager.Instance.m_recordList.Add(record);
                strText = string.Format("  第{2}名  时间：{0:F2}  玩家：{1}", record.time, record.name, i+1);
            }
            string childName = string.Format("No{0}", i + 1);
            Text textComponent = recordSubPanel.transform.Find(childName).gameObject.GetComponent<Text>();
            textComponent.text = strText;
            if (PlayerPrefs.HasKey(key))
            {
                textComponent.color = i < 3 ? new Color(1, 0, 0) : new Color(0, 0, 1);
            }            
        }        
    }
	
	// Update is called once per frame
	void Update () {
	}
}
