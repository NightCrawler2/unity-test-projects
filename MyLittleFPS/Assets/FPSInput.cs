using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Control Script/FPS Input")]
public class FPSInput : MonoBehaviour {

    private CharacterController _charController;

    public float Speed = 6.0f;
    public float Gravity = -9.8f;

	// Use this for initialization
    private void Start () {
        _charController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
    private void Update () {
        var deltaX = Input.GetAxis("Horizontal") * Speed;
        var deltaZ = Input.GetAxis("Vertical") * Speed;

        var movement = new Vector3(deltaX, 0, deltaZ);

        movement = Vector3.ClampMagnitude(movement, Speed);
        movement.y = Gravity;

        movement *= Time.deltaTime;
        movement = transform.TransformDirection(movement);

        _charController.Move(movement);
	}
}
