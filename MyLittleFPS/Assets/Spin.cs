using UnityEngine;

public class Spin : MonoBehaviour {
    public float Speed = 3.0f;

	// Use this for initialization
    private void Start () {
	
	}
	
	// Update is called once per frame
    private void Update () {
        transform.Rotate(30, Speed, 0);
	}
}
