using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Vector2 direction;
	
	public float cameraStep = 2;

	public Transform levelBottomLeft;
	public Transform levelTopRight;

	private Vector2 levelBottomLeftPosition;
	private Vector2 levelTopRightPosition;
	private Vector2 cameraExtents;
	
	void Start ()
	{
		levelBottomLeftPosition = levelBottomLeft.position;
		levelTopRightPosition = levelTopRight.position;
		
		var cam = gameObject.GetComponent<Camera>();
		cameraExtents.y = cam.orthographicSize;
		cameraExtents.x = cameraExtents.y * cam.aspect;
	}
	
	void Update ()
	{
		direction.x = Input.GetAxis("Horizontal");
		direction.y = Input.GetAxis("Vertical");
	
		var cameraPos = transform.position;
		cameraPos.x += direction.x * cameraStep;
		cameraPos.y += direction.y * cameraStep;

		cameraPos.x = Mathf.Clamp(cameraPos.x, levelBottomLeftPosition.x + cameraExtents.x, levelTopRightPosition.x - cameraExtents.x);
		cameraPos.y = Mathf.Clamp(cameraPos.y, levelBottomLeftPosition.y + cameraExtents.y, levelTopRightPosition.y - cameraExtents.y);
	
		transform.position = cameraPos;
	}
}
