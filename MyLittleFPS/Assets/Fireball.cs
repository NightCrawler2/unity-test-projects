using UnityEngine;

public class Fireball : MonoBehaviour {

	public float Speed = 10.0f;
	public int Damage = 1;

	// Use this for initialization
    private void Start () {
	
	}
	
	// Update is called once per frame
    private void Update () {
		transform.Translate (0, 0, Speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other){
		var player = other.GetComponent<PlayerCharacter> ();
		if (player != null) {
			player.Hurt(1);
		}
		Destroy (gameObject);
	}
}
