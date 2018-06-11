using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnNonMobileBuilds : MonoBehaviour 
{
    // Use this for initialization
    void Awake()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        this.gameObject.SetActive(false);
#endif
    }
	
}