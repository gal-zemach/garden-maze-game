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
	
	// used for positioning and controller
	[HideInInspector] public bool playerIsMoving;
	[HideInInspector] public Vector2 gridPosition;
	[HideInInspector] public Vector2 gridCell;
	[HideInInspector] public Vector2 tileSize;
	
	private Rigidbody2D rb2d;
	private SpriteRenderer sprite;
	private Controller controller;
	private GameManager gameManager;
	private TileMap map;
	private Animator animator;

	private const string ANIM_RUNNING = "isRunning";
	private const string ANIM_BACK = "showBack";
	private int animRunning, animBack;

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
		animator = GetComponentInChildren<Animator>();
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

		animBack = Animator.StringToHash(ANIM_BACK);
		animRunning = Animator.StringToHash(ANIM_RUNNING);
		animator.SetBool(animRunning, false);
		animator.SetBool(animBack, true);
		sprite.flipX = true;

		movementSpeed = normalMovementSpeed;
		playerIsMoving = false;
		changeableTiles = initialChangeableTiles;
//		StartMovement();
	}

	private void Update()
	{
		if (!playerIsMoving)
			return;
		
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			movementSpeed = fasterMovementSpeed;
			animator.SetBool(animRunning, true);
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			movementSpeed = normalMovementSpeed;
			animator.SetBool(animRunning, false);
		}
		
//		ChangeableTilesGUI.text = changeableTiles.ToString();
	}

	private void FixedUpdate()
	{
		if (!playerIsMoving)
		{
			UpdatePositionParameters();
			return;
		}
		
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
//			sprite.sprite = frontSprite;
			animator.SetBool(animBack, false);
			sprite.flipX = false;
		}
		else if (horizontalDirection < 0)
		{
//			sprite.sprite = backSprite;
			animator.SetBool(animBack, true);
			sprite.flipX = true;
		}

		else if (verticalDirection > 0)
		{
//			sprite.sprite = backSprite;
			animator.SetBool(animBack, true);
			sprite.flipX = false;
		}
		else if (verticalDirection < 0)
		{
//			sprite.sprite = frontSprite;
			animator.SetBool(animBack, false);
			sprite.flipX = true;
		}
	}

//	public void StartMovement()
//	{
//		UpdatePositionParameters();
//		StartCoroutine(BlinkSpriteAndStartMovement());
//	}

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

//	private IEnumerator BlinkSpriteAndStartMovement()
//	{
//		invincible = true;
//		for (int i = 0; i < 4; i++)
//		{
//			sprite.enabled = false;
//			yield return new WaitForSeconds(0.4f);
//		
//			sprite.enabled = true;
//			yield return new WaitForSeconds(0.4f);
//		}
//		playerIsMoving = true;
//		invincible = false;
//	}

	public void EnableControls()
	{
		var navAgent = GetComponent<NavigationAgent>();
		navAgent.currentDestination = gridCell;
		playerIsMoving = true;
	}

	public void DisableControls()
	{
		playerIsMoving = false;
	}
	
//	private IEnumerator InvincibilityBlink()
//	{
//		yield return new WaitForSeconds(0.7f);
//		playerIsMoving = true;
//		
//		invincible = true;
//		for (int i = 0; i < 3; i++)
//		{
//			sprite.enabled = false;
//			yield return new WaitForSeconds(0.4f);
//		
//			sprite.enabled = true;
//			yield return new WaitForSeconds(0.4f);
//		}
//
//		playerIsMoving = true;
//		invincible = false;
//	}

	public void ReduceLives()
	{
		playerIsMoving = false;
		if (!invincible)
		{
			currentLives--;
			livesVisualizer.decreaseLife();
		}
//		StartCoroutine(InvincibilityBlink());
		StartCoroutine(Blink());
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

	public void AfterHit()
	{
		StartCoroutine(Blink());
	}

	private IEnumerator Blink()
	{
		var blinkInterval = 0.35f;
		invincible = true;
		for (int i = 0; i < 4; i++)
		{
			sprite.enabled = false;
			yield return new WaitForSeconds(blinkInterval);
		
			sprite.enabled = true;
			yield return new WaitForSeconds(blinkInterval);
		}
		invincible = false;
	}
}
