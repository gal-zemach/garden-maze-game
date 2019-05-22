using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : Controller
{
	private Vector2 direction;
	
	void Update () {
		direction.x = Input.GetAxis("Horizontal");
		direction.y = Input.GetAxis("Vertical");
	}

	public override float HorizontalAxis()
	{
		return direction.x;
	}
	
	public override float VerticalAxis()
	{
		return direction.y;
	}
}
