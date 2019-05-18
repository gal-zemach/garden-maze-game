using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{	
	private Rigidbody2D rb2d;
	private SpriteRenderer sprite;
	private Controller controller;
	private TileMap map;
	private GameManager gameManager;

	public Vector2 gridPosition;
	public Vector2 gridCell;
	[HideInInspector]
	public Vector2 tileSize;

	private Vector2 forward, right;
	private bool movementStarted;

	// movement
	public int movementSpeed = 100;	
	
	void Awake ()
	{
		rb2d = GetComponent<Rigidbody2D>();
		sprite = GetComponentInChildren<SpriteRenderer>();
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		controller = GetComponent<Controller>();
		if (ReferenceEquals(controller, null))
		{
			Debug.LogError("player is missing a Controller component");
		}
		rb2d.gravityScale = 0;

		map = GameObject.Find("Tile Map").GetComponent<TileMap>();
		if (map == null)
		{
			Debug.LogError("cannot find Tile Map");
			return;
		}

		tileSize = new Vector2(map.tileSize.x, map.tileSize.y / 2);
		
		var worldPosition = transform.position;
		gridPosition = IsoVectors.WorldToIso(worldPosition, tileSize);
		gridCell = new Vector2(Mathf.Round(gridPosition.x), Mathf.Round(gridPosition.y));

		movementStarted = false;
	}
	
	private void FixedUpdate()
	{
		if (!movementStarted)
			return;
		
		float horizontalDirection = controller.HorizontalAxis();
		float verticalDirection = controller.VerticalAxis();

		var worldPosition = transform.position;
		gridPosition = IsoVectors.WorldToIso(worldPosition, tileSize);
		gridCell = new Vector2(Mathf.Round(gridPosition.x), Mathf.Round(gridPosition.y)); // changed from Floor to Round because of AIController not recognizing it got to a destination cell
		map.tiles[map.TileIndex((int)gridCell.x, (int)gridCell.y)].MarkAsVisited();
		
		Vector2 currentPos = rb2d.position;
//		var inputVector = new Vector2(horizontalDirection, verticalDirection);

		var verticalVector = (verticalDirection * IsoVectors.UP).normalized;
		var horizontalVector = (horizontalDirection * IsoVectors.RIGHT).normalized;
		var inputVector = horizontalVector + verticalVector;
		
		inputVector = Vector2.ClampMagnitude(inputVector, 1);
		Vector2 movement = inputVector * movementSpeed;
		Vector3 newPos = currentPos + movement * Time.fixedDeltaTime;
		
		rb2d.MovePosition(newPos);
		
		// updating z position
		worldPosition = transform.position;
		worldPosition.z = gridPosition.x + gridPosition.y;
		transform.position = worldPosition;
	}

	public void StartMovement()
	{
		StartCoroutine(BlinkSpriteAndStartMovement());
	}

	private IEnumerator BlinkSpriteAndStartMovement()
	{
		for (int i = 0; i < 3; i++)
		{
			sprite.enabled = false;
			yield return new WaitForSeconds(0.4f);
		
			sprite.enabled = true;
			yield return new WaitForSeconds(0.4f);
		}
		movementStarted = true;
	}

	public void FellToHole()
	{
		Debug.Log("PlayerScript: player fell to Hole");
		movementStarted = false;
		StartCoroutine(WaitAndEndGame());
	}

	private IEnumerator WaitAndEndGame()
	{
		yield return new WaitForSeconds(2f);
		gameManager.PlayerDied();
	}
}
