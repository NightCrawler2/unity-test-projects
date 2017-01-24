using UnityEngine;

public class SceneController : MonoBehaviour {

	[SerializeField] private GameObject _enemyPrefab;
	private GameObject _enemy;

	// Use this for initialization
    private void Start () {
	
	}
	
	// Update is called once per frame
    private void Update () {
	    if (_enemy != null) return;
	    _enemy = Instantiate(_enemyPrefab);
	    _enemy.transform.position = new Vector3 (0, 1, 0);
	    float angle = Random.Range (0, 360);
	    _enemy.transform.Rotate (0, angle, 0);
	}
}
