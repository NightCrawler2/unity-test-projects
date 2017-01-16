using UnityEngine;

public class WanderingAI : MonoBehaviour {

	public float Speed = 3.0f;
	public float ObstacleRange = 5.0f;

	private bool _alive;

	[SerializeField] private GameObject _fireballPrefab;
	private GameObject _fireball;

	// Use this for initialization
    private void Start () {
		_alive = true;
	}
	
	// Update is called once per frame
    private void Update () {
        if (!_alive) return;
        transform.Translate (0, 0, Speed * Time.deltaTime);
        var ray = new Ray (transform.position, transform.forward);

        RaycastHit hit;

        if (!Physics.SphereCast(ray, 0.75f, out hit)) return;
        var hitObject = hit.transform.gameObject;
        if (hitObject.GetComponent<PlayerCharacter> ()) {
            if (_fireball != null) return;
            _fireball = Instantiate (_fireballPrefab);
            _fireball.transform.position = transform.TransformPoint (Vector3.forward * 1.5f);
            _fireball.transform.rotation = transform.rotation;
        } else if (hit.distance < ObstacleRange) {
            float angle = Random.Range (-110, 110);
            transform.Rotate (0, angle, 0);
        }
    }

	public void SetAlive(bool alive) {
		_alive = alive;
	}
}
