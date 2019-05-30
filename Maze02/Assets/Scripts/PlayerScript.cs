using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{	
	public Sprite frontSprite, backSprite;
	
	public int MaxLives = 3;
	public int initialChangeableTiles = 3;
	public LivesVisualizer livesVisualizer;
	public bool invincible;
	public int normalMovementSpeed = 100;
	public int fasterMovementSpeed = 200;
	public Text ChangeableTilesGUI;
	
	// used for positioning and controller
	[HideInInspector]
	public bool movementStarted;
	[HideInInspector]
	public Vector2 gridPosition;
	[HideInInspector]
	public Vector2 gridCell;
	[HideInInspector]
	public Vector2 tileSize;
	
	private Rigidbody2D rb2d;
	private SpriteRenderer sprite;
	private Controller controller;
	private GameManager gameManager;
	private TileMap map;

	private List<Item.ItemType> items;
	private Vector2 forward, right;
	private int movementSpeed = 100;
	private int currentLives = 5;
	[HideInInspector] public int changeableTiles;

	void Awake ()
	{
		currentLives = MaxLives;
		livesVisualizer = GameObject.Find("Lives").GetComponent<LivesVisualizer>();
		livesVisualizer.setLives(MaxLives);
		
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
		
		items = new List<Item.ItemType>();
		
		sprite.sprite = backSprite;
		movementSpeed = normalMovementSpeed;
		movementStarted = false;
		changeableTiles = initialChangeableTiles;
		StartMovement();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			movementSpeed = fasterMovementSpeed;
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			movementSpeed = normalMovementSpeed;
		}
		
		ChangeableTilesGUI.text = changeableTiles.ToString();
	}

	private void FixedUpdate()
	{
		if (!movementStarted)
			return;
		
		float horizontalDirection = controller.HorizontalAxis();
		float verticalDirection = controller.VerticalAxis();

		UpdatePositionParameters();
		MarkCurrentTileAsVisited();
		UpdatePlayerSprite(horizontalDirection, verticalDirection);
		
		MovePlayer(horizontalDirection, verticalDirection);
		UpdateZPosition();
	}

	private void UpdatePositionParameters()
	{
		var worldPosition = transform.position;
		gridPosition = IsoVectors.WorldToIso(worldPosition, tileSize);
		gridCell = new Vector2(Mathf.Round(gridPosition.x), Mathf.Round(gridPosition.y)); // changed from Floor to Round because of AIController not recognizing it got to a destination cell
	}

	private void MovePlayer(float horizontalDirection, float verticalDirection)
	{
		Vector2 currentPos = rb2d.position;
		Vector2 verticalVector = (verticalDirection * IsoVectors.UP).normalized;
		Vector2 horizontalVector = (horizontalDirection * IsoVectors.RIGHT).normalized;
		var inputVector = horizontalVector + verticalVector;
		
		rb2d.MovePosition(currentPos + inputVector.normalized * movementSpeed * Time.fixedDeltaTime);
	}

	private void UpdateZPosition()
	{
		var worldPosition = transform.position;
		worldPosition = transform.position;
		worldPosition.z = gridPosition.x + gridPosition.y;
		transform.position = worldPosition;
	}

	private void UpdatePlayerSprite(float horizontalDirection, float verticalDirection)
	{
		if (horizontalDirection > 0)
		{
			sprite.sprite = frontSprite;
			sprite.flipX = true;
		}
		else if (horizontalDirection < 0)
		{
			sprite.sprite = backSprite;
			sprite.flipX = false;
		}

		else if (verticalDirection > 0)
		{
			sprite.sprite = backSprite;
			sprite.flipX = true;
		}
		else if (verticalDirection < 0)
		{
			sprite.sprite = frontSprite;
			sprite.flipX = false;
		}
	}

	public void StartMovement()
	{
		UpdatePositionParameters();
		StartCoroutine(BlinkSpriteAndStartMovement());
	}

	private void MarkCurrentTileAsVisited()
	{
		var tile = map.tiles[map.TileIndex((int)gridCell.x, (int)gridCell.y)].GetComponent<Tile>();
		var moveableWall = tile as MoveableWall;
		if (moveableWall != null)
		{
			var visitedNewTile = moveableWall.MarkAsVisited();
			if (visitedNewTile)
				AddChangeableTile();
		}
	}

	private void AddChangeableTile()
	{
		changeableTiles++;
	}

	public void SubtractChangeableTile()
	{
		changeableTiles--;
	}

	private IEnumerator BlinkSpriteAndStartMovement()
	{
		invincible = true;
		for (int i = 0; i < 4; i++)
		{
			sprite.enabled = false;
			yield return new WaitForSeconds(0.4f);
		
			sprite.enabled = true;
			yield return new WaitForSeconds(0.4f);
		}
		movementStarted = true;
		invincible = false;
	}
	
	private IEnumerator InvincibilityBlink()
	{
		yield return new WaitForSeconds(0.7f);
		movementStarted = true;
		
		invincible = true;
		for (int i = 0; i < 3; i++)
		{
			sprite.enabled = false;
			yield return new WaitForSeconds(0.4f);
		
			sprite.enabled = true;
			yield return new WaitForSeconds(0.4f);
		}

		movementStarted = true;
		invincible = false;
	}

	public void ReduceLives()
	{
		movementStarted = false;
		if (!invincible)
		{
			currentLives--;
			livesVisualizer.decreaseLife();
		}
		StartCoroutine(InvincibilityBlink());
	}
	
	public bool IsDead()
	{
		return currentLives == 0;
	}
	
	public void AddItem(Item.ItemType itemType)
	{
		if (items.Contains(itemType))
			return;
		
		items.Add(itemType);
	}

	public bool HasItem(Item.ItemType itemType)
	{
		return items.Contains(itemType);
	}
}
