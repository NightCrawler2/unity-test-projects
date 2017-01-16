using System;
using UnityEngine;

public class MouseLook : MonoBehaviour {
    public enum RotationAxes
    {
        MouseXnY = 0,
        MouseX = 1,
        MouseY = 2
    }
    public RotationAxes Axes;
    public float SensitivityHor = 6.0f;
    public float SensitivityVert = 6.0f;
    public float MaxVert = 45.0f;
    public float MinVert = -45.0f;

    private float _rotationX;
    // Use this for initialization
    private void Start () {
        var body = GetComponent<Rigidbody>();
        if (body == null) return;
        body.freezeRotation = true;
    }
	
	// Update is called once per frame
    private void Update ()
	{
	    switch (Axes)
	    {
	        case RotationAxes.MouseX:
	            transform.Rotate(0, Input.GetAxis("Mouse X") * SensitivityHor, 0);
	            break;
	        case RotationAxes.MouseY:
	        {
	            _rotationX -= Input.GetAxis("Mouse Y") * SensitivityVert;
	            _rotationX = Mathf.Clamp(_rotationX, MinVert, MaxVert);

	            var rotationY = transform.localEulerAngles.y;

	            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
	        }
	            break;
	        case RotationAxes.MouseXnY:
	        {
	            _rotationX -= Input.GetAxis("Mouse Y") * SensitivityVert;
	            _rotationX = Mathf.Clamp(_rotationX, MinVert, MaxVert);

	            var delta = Input.GetAxis("Mouse X") * SensitivityHor;
	            var rotationY = transform.localEulerAngles.y + delta;

	            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
	        }
	            break;
	        default:
	            throw new ArgumentOutOfRangeException();
	    }
	}
}
