using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Vector2 direction;

	public bool followMouse;
	float mouseSensitivity = 0.05f;
	public bool followTarget;
	public GameObject target;
	
	[Space(20)]
	public float cameraStep = 2;
	public int cameraZoomChangeStep = 5;
	public int initialCameraZoom = 200;
	public int minCameraZoom = 100;
	public int maxCameraZoom = 400;

	public Transform levelBottomLeft;
	public Transform levelTopRight;

	private Camera cam;
	private Vector2 levelBottomLeftPosition;
	private Vector2 levelTopRightPosition;
	private Vector2 cameraExtents;
	private int cameraZoom;
	private float cameraZ = -10;
	
	void Start ()
	{
		levelBottomLeftPosition = levelBottomLeft.position;
		levelTopRightPosition = levelTopRight.position;
		
		cam = gameObject.GetComponent<Camera>();
		cameraExtents.y = cam.orthographicSize;
		cameraExtents.x = cameraExtents.y * cam.aspect;

		cameraZoom = initialCameraZoom;

		if (target != null && followTarget)
		{
			UpdatePositionToTarget();
		}
			
			
	}
	
	void Update ()
	{
		// zoom
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			cameraZoom += cameraZoomChangeStep;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			cameraZoom -= cameraZoomChangeStep;
		}

		cameraZoom = (int) Mathf.Clamp(cameraZoom, minCameraZoom, maxCameraZoom);
		cam.orthographicSize = cameraZoom;
		
		cameraExtents.y = cam.orthographicSize;
		cameraExtents.x = cameraExtents.y * cam.aspect;


		if (target != null && followTarget)
		{
			UpdatePositionToTarget();
		}
		else if (followMouse)
		{
			Vector3 vp = cam.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
			vp.x -= 0.5f;
			vp.y -= 0.5f;
			vp.x *= mouseSensitivity;
			vp.y *= mouseSensitivity;
			vp.x += 0.5f;
			vp.y += 0.5f;
			Vector3 sp = cam.ViewportToScreenPoint(vp);
			
			Vector3 v = cam.ScreenToWorldPoint(sp);
			transform.position = v;
			
//			var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
//			transform.position = mousePos;
		}
		else
		{
			// directional movement
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

	private void UpdatePositionToTarget()
	{
		var pos = target.transform.position;
		pos.z = cameraZ;
		transform.position = pos;
	}
}
