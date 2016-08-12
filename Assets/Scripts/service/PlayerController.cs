using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public delegate void PlayerMovementEvent(Vector3 movement);
	public static event PlayerMovementEvent OnMove;

	public float speed = 10f;

	Rigidbody rigidBody;

	private void Move(Vector3 movement) 
	{
		movement = movement.normalized * speed * Time.deltaTime;
		rigidBody.MovePosition (transform.position + movement);
	}

	void OnEnable()
	{
		UDPManager.OnServerMove += Move;
	}

	void OnDisable()
	{
		UDPManager.OnServerMove -= Move;
	}

	void Awake()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		float v = Input.GetAxisRaw ("Vertical");
		float h = Input.GetAxisRaw ("Horizontal");
		Vector3 movement = Vector3.zero;

		movement.Set (h, 0f, v);

		if (movement == Vector3.zero) 
			return;

		if (OnMove != null) 
		{
			OnMove(movement);
		}
	}
}
