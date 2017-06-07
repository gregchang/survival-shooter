using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 6f;

	Vector3 movement;
	Animator anim;
	Rigidbody playerRigidbody;
	// Layer mask tells raycast we only want to hit floor
	int floorMask;
	// Length of the ray that we cast from the camera
	float camRayLength = 100;

	// Unlike Start(), Awake() is called regardless of whether or not script is enabled
	// Good for setting up references
	void Awake ()
	{
		floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody> ();
	}

	// Called every fixed framerate frame, should be used when dealing with physics Rigidbody
	void FixedUpdate ()
	{
		// Raw Axis only has values -1, 0, and 1
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		Move (h, v);
		Turning ();
		Animating (h, v);
	}

	void Move (float h, float v)
	{
		// Normalise so that diagonal movement is not faster
		movement.Set (h, 0f, v);

		// Change to per second, deltaTime is the time between each update call
		movement = movement.normalized * speed * Time.deltaTime;

		// Apply movement to player
		// MovePosition moves a rigidbody to a position in world space
		playerRigidbody.MovePosition (transform.position + movement);
	}

	void Turning ()
	{
		// Create ray that we cast from camera into the scene
		// so that player turns to look at direction of mouse

		// Takes a point on the screen and casts a ray from that point forwards into the scene
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit floorHit;

		// Raycast returns true if it hits something
		// out means we get information out of this function and store it in floorHit

		if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
			// point where ray hits floor minus position of player
			Vector3 playerToMouse = floorHit.point - transform.position;

			// Don't want player leaning back
			playerToMouse.y = 0f;

			// Can't set player's rotation with vector, so use Quaternion
			// z axis is forward axis
			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
			playerRigidbody.MoveRotation (newRotation);
		}
	}

	void Animating (float h, float v)
	{
		bool walking = h != 0f || v != 0f;
		anim.SetBool ("IsWalking", walking);
	}
}

