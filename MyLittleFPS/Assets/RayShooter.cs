using UnityEngine;
using System.Collections;

public class RayShooter : MonoBehaviour {
    private Camera _camera;

	// Use this for initialization
    private void Start () {
        _camera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
    private void Update () {
	    if (!Input.GetMouseButtonDown(0)) return;
	    var point = new Vector3(_camera.pixelWidth / 2f, _camera.pixelHeight / 2f, 0);

	    var ray = _camera.ScreenPointToRay(point);
	    RaycastHit hit;
	    if (!Physics.Raycast(ray, out hit)) return;
	    //StartCoroutine(SphereIndicator(hit.point));
	    var hitObject = hit.transform.gameObject;
	    var reactTarg = hitObject.GetComponent<ReactiveTarget> ();
	    if (reactTarg != null) {
	        //Debug.Log ("Target hit");
	        reactTarg.ReactToHit();
	    } else {
	        StartCoroutine(SphereIndicator(hit.point));
	    }
	}

    private void OnGUI()
    {
        const int size = 12;
        float posX = _camera.pixelWidth / 2 - size / 4;
        float posY = _camera.pixelHeight / 2 - size / 2;
        GUI.Label(new Rect(posX, posY, size, size), "*");
    }

    private static IEnumerator SphereIndicator(Vector3 position)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;

        yield return new WaitForSeconds(1);

        Destroy(sphere);
    }
}
