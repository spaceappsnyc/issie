using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpaceshipPlayerController : MonoBehaviour {

	public float speed;
	public bool flying;
	public Text countText;
	public Text winText;

	private Rigidbody rb;
	private int count;

	void Start ()
	{
		rb = GetComponent<Rigidbody>();
		count = 0;
		SetCountText ();
		winText.text = "";
	}
	
	void FixedUpdate()
	{

		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis("Vertical"); 
		float stopButton = Input.GetAxis ("Stop");
		float jumpButton = Input.GetAxis ("Jump");
		float xAccel = Input.acceleration.x;
		float zAccel = Input.acceleration.z;

		//Should convert Jump to thrust
		//Should convert vert to tilt up 
		//Should convert horiz to turn

		if (true) { //rb.transform.position.y < 0.75f || flying) {
			if (jumpButton != 0.0f) {
				Vector3 jump = new Vector3 (0.0f, jumpButton, 0.0f);
				rb.AddForce (jump);
			} else if (stopButton != 0.0f) {
				rb.velocity = Vector3.zero;
			} else {
				if (xAccel != 0.0f || zAccel != 0.0f){
					Vector3 movement = new Vector3 (xAccel, 0.0f, -(zAccel + 0.5f));
					rb.AddForce (movement * speed);
				}
				else{
					Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
					rb.AddForce (movement * speed);
				}
			}
		}


	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Pick Up")) {
			other.gameObject.SetActive (false);
			count = count + 1;
			SetCountText();
		}
		  
	}

	void SetCountText(){
		countText.text = "Count: " + count.ToString ();
		if (count >= 8)
			winText.text = "You Win!";
	}


}







