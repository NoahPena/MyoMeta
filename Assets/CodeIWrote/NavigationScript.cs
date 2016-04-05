using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ISuckAtCoding;

public class NavigationScript : MonoBehaviour 
{

    Text navigationText;
	// Use this for initialization
	void Start () 
    {
        navigationText = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        navigationText.text = GlobalVariables.navigationData;
	}
}
