using System.Collections.Generic;
using UnityEngine;

public class CGameConfig : MonoBehaviour
{
    public int m_nMapWidth = 10;
    public int m_nMapHeight = 10;
    public List<Sprite> m_ImageList;
    // Use this for initialization
    void Awake () {
        CGameManager.Instance.m_pConfig = this;
        if (m_nMapHeight % 2 != 0)
        {
            m_nMapHeight -= 1;//To make the total count is double, make height as double. 
        }
        Debug.Assert(m_nMapWidth > 2, "Map width must be greater than 2!");
        Debug.Assert(m_nMapHeight > 2, "Map width must be greater than 2!");
        Debug.Assert(m_ImageList.Count > 1, "Image list count must be greater than 1!");
    }
}
