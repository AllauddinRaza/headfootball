using UnityEngine;
using System;
using System.Collections.Generic;


[RequireComponent( typeof( BoxCollider2D ), typeof( Rigidbody2D ) )]
public class CharacterController2D : MonoBehaviour
{
    private bool isKicking;

    public bool GetKicking()
    {
        return isKicking;
    }

    public void SetKickingFalse()
    {
        isKicking = false;
    }

    public void SetKickingTrue()
    {
        isKicking = true;
    }

    private struct CharacterRaycastOrigins
	{
		public Vector3 topLeft;
		public Vector3 bottomRight;
		public Vector3 bottomLeft;
	}

	public class CharacterCollisionState2D
	{
		public bool right;
		public bool left;
		public bool above;
		public bool below;
		public bool becameGroundedThisFrame;
		public bool wasGroundedLastFrame;
		public bool movingDownSlope;
		public float slopeAngle;


		public bool hasCollision()
		{
			return below || right || left || above;
		}

		public void reset()
		{
			right = left = above = below = becameGroundedThisFrame = movingDownSlope = false;
			slopeAngle = 0f;
		}


		public override string ToString()
		{
			return string.Format( "[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, wasGroundedLastFrame: {6}, becameGroundedThisFrame: {7}",
			                     right, left, above, below, movingDownSlope, slopeAngle, wasGroundedLastFrame, becameGroundedThisFrame );
		}
	}

	public event Action<RaycastHit2D> onControllerCollidedEvent;
	public event Action<Collider2D> onTriggerEnterEvent;
	public event Action<Collider2D> onTriggerStayEvent;
	public event Action<Collider2D> onTriggerExitEvent;


	public bool usePhysicsForMovement = false;

	[SerializeField]
	[Range( 0.001f, 0.3f )]
	private float _skinWidth = 0.02f;

	public float skinWidth
	{
		get { return _skinWidth; }
		set
		{
			_skinWidth = value;
			recalculateDistanceBetweenRays();
		}
	}

	public LayerMask platformMask = 0;
	public LayerMask triggerMask = 0;

	[SerializeField]
	private LayerMask oneWayPlatformMask = 0;

	[Range( 0, 90f )]
	public float slopeLimit = 30f;

	public float jumpingThreshold = 0.07f;

	public AnimationCurve slopeSpeedMultiplier = new AnimationCurve( new Keyframe( -90, 1.5f ), new Keyframe( 0, 1 ), new Keyframe( 90, 0 ) );

	[Range( 2, 20 )]
	public int totalHorizontalRays = 8;
	[Range( 2, 20 )]
	public int totalVerticalRays = 4;

	private float _slopeLimitTangent = Mathf.Tan( 75f * Mathf.Deg2Rad );

	[Range( 0.8f, 0.999f )]
	public float triggerHelperBoxColliderScale = 0.95f;

	[HideInInspector][NonSerialized]
	public new Transform transform;
	[HideInInspector][NonSerialized]
	public BoxCollider2D boxCollider;
	[HideInInspector][NonSerialized]
	public Rigidbody2D rigidBody2D;

	[HideInInspector][NonSerialized]
	public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();
	[HideInInspector][NonSerialized]
	public Vector3 velocity;
	public bool isGrounded { get { return collisionState.below; } }

    private const float kSkinWidthFloatFudgeFactor = 0.001f;

	private CharacterRaycastOrigins _raycastOrigins;

	private RaycastHit2D _raycastHit;

	private List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>( 2 );

	private float _verticalDistanceBetweenRays;
	private float _horizontalDistanceBetweenRays;
	private bool _isGoingUpSlope = false;

	void Awake()
	{
		platformMask |= oneWayPlatformMask;

		transform = GetComponent<Transform>();
		boxCollider = GetComponent<BoxCollider2D>();
		rigidBody2D = GetComponent<Rigidbody2D>();

		skinWidth = _skinWidth;

		for( var i = 0; i < 32; i++ )
		{
			if( ( triggerMask.value & 1 << i ) == 0 )
				Physics2D.IgnoreLayerCollision( gameObject.layer, i );
		}
	}


	public void OnTriggerEnter2D( Collider2D col )
	{
		if( onTriggerEnterEvent != null )
			onTriggerEnterEvent( col );
	}

	public void OnTriggerStay2D( Collider2D col )
	{
		if( onTriggerStayEvent != null )
			onTriggerStayEvent( col );
	}

	public void OnTriggerExit2D( Collider2D col )
	{
		if( onTriggerExitEvent != null )
			onTriggerExitEvent( col );
	}

	[System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
	private void DrawRay( Vector3 start, Vector3 dir, Color color )
	{
		Debug.DrawRay( start, dir, color );
	}

	public void move( Vector3 deltaMovement )
	{
		collisionState.wasGroundedLastFrame = collisionState.below;

		collisionState.reset();
		_raycastHitsThisFrame.Clear();
		_isGoingUpSlope = false;

		var desiredPosition = transform.position + deltaMovement;
		primeRaycastOrigins( desiredPosition, deltaMovement );

		if( deltaMovement.y < 0 && collisionState.wasGroundedLastFrame )
			handleVerticalSlope( ref deltaMovement );

		if( deltaMovement.x != 0 )
			moveHorizontally( ref deltaMovement );

		if( deltaMovement.y != 0 )
			moveVertically( ref deltaMovement );

		if( usePhysicsForMovement )
		{
			GetComponent<Rigidbody2D>().MovePosition( transform.position + deltaMovement );
			velocity = GetComponent<Rigidbody2D>().velocity;
		}
		else
		{
			transform.Translate( deltaMovement, Space.World );

			if( Time.deltaTime > 0 )
				velocity = deltaMovement / Time.deltaTime;
		}

		if( !collisionState.wasGroundedLastFrame && collisionState.below )
			collisionState.becameGroundedThisFrame = true;

		if( _isGoingUpSlope )
			velocity.y = 0;

		if( onControllerCollidedEvent != null )
		{
			for( var i = 0; i < _raycastHitsThisFrame.Count; i++ )
				onControllerCollidedEvent( _raycastHitsThisFrame[i] );
		}
	}

	public void warpToGrounded()
	{
		do
		{
			move( new Vector3( 0, -1f, 0 ) );
		} while( !isGrounded );
	}

	public void recalculateDistanceBetweenRays()
	{
		var colliderUseableHeight = boxCollider.size.y * Mathf.Abs( transform.localScale.y ) - ( 2f * _skinWidth );
		_verticalDistanceBetweenRays = colliderUseableHeight / ( totalHorizontalRays - 1 );

		var colliderUseableWidth = boxCollider.size.x * Mathf.Abs( transform.localScale.x ) - ( 2f * _skinWidth );
		_horizontalDistanceBetweenRays = colliderUseableWidth / ( totalVerticalRays - 1 );
	}

	private void primeRaycastOrigins( Vector3 futurePosition, Vector3 deltaMovement )
	{
		var modifiedBounds = boxCollider.bounds;
		modifiedBounds.Expand( -2f * _skinWidth );

		_raycastOrigins.topLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
		_raycastOrigins.bottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
		_raycastOrigins.bottomLeft = modifiedBounds.min;
	}

	private void moveHorizontally( ref Vector3 deltaMovement )
	{
		var isGoingRight = deltaMovement.x > 0;
		var rayDistance = Mathf.Abs( deltaMovement.x ) + _skinWidth;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

		for( var i = 0; i < totalHorizontalRays; i++ )
		{
			var ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays );

			DrawRay( ray, rayDirection * rayDistance, Color.red );

			if( i == 0 && collisionState.wasGroundedLastFrame )
				_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask );
			else
				_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask & ~oneWayPlatformMask );

			if( _raycastHit )
			{
				if( i == 0 && handleHorizontalSlope( ref deltaMovement, Vector2.Angle( _raycastHit.normal, Vector2.up ) ) )
				{
					_raycastHitsThisFrame.Add( _raycastHit );
					break;
				}

				deltaMovement.x = _raycastHit.point.x - ray.x;
				rayDistance = Mathf.Abs( deltaMovement.x );

				if( isGoingRight )
				{
					deltaMovement.x -= _skinWidth;
					collisionState.right = true;
				}
				else
				{
					deltaMovement.x += _skinWidth;
					collisionState.left = true;
				}

				_raycastHitsThisFrame.Add( _raycastHit );

				if( rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor )
					break;
			}
		}
	}

	private bool handleHorizontalSlope( ref Vector3 deltaMovement, float angle )
	{
		if( Mathf.RoundToInt( angle ) == 90 )
			return false;

		if( angle < slopeLimit )
		{
			if( deltaMovement.y < jumpingThreshold )
			{
				var slopeModifier = slopeSpeedMultiplier.Evaluate( angle );
				deltaMovement.x *= slopeModifier;

				deltaMovement.y = Mathf.Abs( Mathf.Tan( angle * Mathf.Deg2Rad ) * deltaMovement.x );
				_isGoingUpSlope = true;

				collisionState.below = true;
			}
		}
		else
		{
			deltaMovement.x = 0;
		}

		return true;
	}


	private void moveVertically( ref Vector3 deltaMovement )
	{
		var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs( deltaMovement.y ) + _skinWidth;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

		initialRayOrigin.x += deltaMovement.x;

		var mask = platformMask;
		if( isGoingUp && !collisionState.wasGroundedLastFrame )
			mask &= ~oneWayPlatformMask;

		for( var i = 0; i < totalVerticalRays; i++ )
		{
			var ray = new Vector2( initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y );

			DrawRay( ray, rayDirection * rayDistance, Color.red );
			_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, mask );
			if( _raycastHit )
			{
				deltaMovement.y = _raycastHit.point.y - ray.y;
				rayDistance = Mathf.Abs( deltaMovement.y );

				if( isGoingUp )
				{
					deltaMovement.y -= _skinWidth;
					collisionState.above = true;
				}
				else
				{
					deltaMovement.y += _skinWidth;
					collisionState.below = true;
				}

				_raycastHitsThisFrame.Add( _raycastHit );

				if( !isGoingUp && deltaMovement.y > 0.00001f )
					_isGoingUpSlope = true;

				if( rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor )
					return;
			}
		}
	}

	private void handleVerticalSlope( ref Vector3 deltaMovement )
	{
		var centerOfCollider = ( _raycastOrigins.bottomLeft.x + _raycastOrigins.bottomRight.x ) * 0.5f;
		var rayDirection = -Vector2.up;

		var slopeCheckRayDistance = _slopeLimitTangent * ( _raycastOrigins.bottomRight.x - centerOfCollider );

		var slopeRay = new Vector2( centerOfCollider, _raycastOrigins.bottomLeft.y );
		DrawRay( slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow );
		_raycastHit = Physics2D.Raycast( slopeRay, rayDirection, slopeCheckRayDistance, platformMask );
		if( _raycastHit )
		{
			var angle = Vector2.Angle( _raycastHit.normal, Vector2.up );
			if( angle == 0 )
				return;

			var isMovingDownSlope = Mathf.Sign( _raycastHit.normal.x ) == Mathf.Sign( deltaMovement.x );
			if( isMovingDownSlope )
			{
				var slopeModifier = slopeSpeedMultiplier.Evaluate( -angle );
				deltaMovement.y = _raycastHit.point.y - slopeRay.y - skinWidth;
				deltaMovement.x *= slopeModifier;
				collisionState.movingDownSlope = true;
				collisionState.slopeAngle = angle;
			}
		}
	}
}