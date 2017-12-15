using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {

    private Button m_pStartBtn;
    // Use this for initialization
    void Start() {
        GameObject pStartBtn = transform.Find("startbtn").gameObject;
        Debug.Assert(pStartBtn != null);
        m_pStartBtn = pStartBtn.GetComponent<Button>();
        m_pStartBtn.onClick.AddListener(delegate () {
            gameObject.SetActive(false);
            CGameManager.Instance.GenerateCells();
        });
    }
	
	// Update is called once per frame
	void Update () {
	}
}
