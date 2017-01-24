using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text _scoreLabel;
    [SerializeField] private SettingsPopup _settingsPopup;

	// Use this for initialization
    private void Start () {
		_settingsPopup.Close();
	}
	
	// Update is called once per frame
    private void Update () {
        _scoreLabel.text = Time.realtimeSinceStartup.ToString(CultureInfo.CurrentCulture);
	}

    public void OnOpenSettings()
    {
        _settingsPopup.Open();
    }
}
