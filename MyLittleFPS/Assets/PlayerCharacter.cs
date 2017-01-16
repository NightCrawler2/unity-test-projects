using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    private int _health;

	// Use this for initialization
    private void Start ()
    {
        _health = 5;
    }
	
	// Update is called once per frame
    private void Update () {
	
	}

    public void Hurt(int damage)
    {
        _health -= damage;
        Debug.Log("Health: " + _health);
    }
}
