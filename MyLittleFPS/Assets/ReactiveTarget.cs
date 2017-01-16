using System.Collections;
using UnityEngine;

public class ReactiveTarget : MonoBehaviour {

	// Use this for initialization
    private void Start () {
	
	}
	
	// Update is called once per frame
    private void Update () {
	
	}

	public void ReactToHit(){
		var wai = transform.GetComponent<WanderingAI> ();
		if (wai != null) {
			wai.SetAlive (false);
		}
		StartCoroutine (Die ());
	}

	private IEnumerator Die(){
		transform.Rotate (-75, 0, 0);

		yield return new WaitForSeconds (1.5f);

		Destroy (gameObject);
	}

}
