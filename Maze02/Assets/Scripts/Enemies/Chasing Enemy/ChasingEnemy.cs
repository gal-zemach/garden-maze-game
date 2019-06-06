using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : MonoBehaviour
{
	public int normalMovementSpeed;
	
	// used for positioning and controller
	[HideInInspector] public bool playerIsMoving;
	[HideInInspector] public Vector2 gridPosition;
	[HideInInspector] public Vector2 gridCell;
	[HideInInspector] public Vector2 tileSize;
	
	private Rigidbody2D rb2d;
	private SpriteRenderer sprite;
	private Controller controller;
	private TileMap map;
//	private Animator animator;

	private int movementSpeed = 100;

	void Awake ()
	{
		rb2d = GetComponent<Rigidbody2D>();
		sprite = GetComponentInChildren<SpriteRenderer>();
//		animator = GetComponentInChildren<Animator>();
		controller = GetComponent<Controller>();
		if (ReferenceEquals(controller, null))
		{
			Debug.LogError("enemy is missing a Controller component");
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
		
//		animBack = Animator.StringToHash(ANIM_BACK);
//		animRunning = Animator.StringToHash(ANIM_RUNNING);
//		animator.SetBool(animRunning, false);
//		animator.SetBool(animBack, true);
//		sprite.flipX = true;

		movementSpeed = normalMovementSpeed;
		playerIsMoving = false;
		EnableControls();
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
//		UpdatePlayerSprite(horizontalDirection, verticalDirection);
		
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
		
		rb2d.velocity = inputVector.normalized * movementSpeed; // speeds: 1100, 8000 with delta time, 30, 160 without
	}

	private void UpdateZPosition()
	{
		var worldPosition = transform.position;
		worldPosition = transform.position;
		worldPosition.z = gridPosition.x + gridPosition.y;
		transform.position = worldPosition;
	}

//	private void UpdatePlayerSprite(float horizontalDirection, float verticalDirection)
//	{
//		if (horizontalDirection > 0)
//		{
////			sprite.sprite = frontSprite;
//			animator.SetBool(animBack, false);
//			sprite.flipX = false;
//		}
//		else if (horizontalDirection < 0)
//		{
////			sprite.sprite = backSprite;
//			animator.SetBool(animBack, true);
//			sprite.flipX = true;
//		}
//
//		else if (verticalDirection > 0)
//		{
////			sprite.sprite = backSprite;
//			animator.SetBool(animBack, true);
//			sprite.flipX = false;
//		}
//		else if (verticalDirection < 0)
//		{
////			sprite.sprite = frontSprite;
//			animator.SetBool(animBack, false);
//			sprite.flipX = true;
//		}
//	}

//	public void StartMovement()
//	{
//		UpdatePositionParameters();
//		StartCoroutine(BlinkSpriteAndStartMovement());
//	}

	public void EnableControls()
	{
		var navAgent = GetComponent<EnemyNavigationAgent>();
		navAgent.currentDestination = gridCell;
		playerIsMoving = true;
	}

	public void DisableControls()
	{
		playerIsMoving = false;
	}
}
