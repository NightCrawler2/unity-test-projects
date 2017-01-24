using UnityEngine;

public class SettingsPopup : MonoBehaviour {

	// Use this for initialization
    private void Start () {
		
	}
	
	// Update is called once per frame
    private void Update () {
		
	}

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnSubmitName(string name)
    {
        Debug.Log(name);
    }

    public void OnSpeedValue(float speed)
    {
        Debug.Log("Speed: " + speed);
    }
}
