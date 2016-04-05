using UnityEngine;
using System.Collections;

public class CameraShit : MonoBehaviour {

    GUITexture BackgroundTexture;
    WebCamTexture CameraTexture;

	// Use this for initialization
	void Start () 
    {
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Initialize()
    {
        CameraTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = CameraTexture;
        //renderer.material.mainTexture = CameraTexture;
        CameraTexture.Play();
    }
}
