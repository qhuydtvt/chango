using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CustomHeroScript : MonoBehaviour
{
	public int numberOfRayHorizontal;
	public int numberOfRayVertical;

	private float raySpacingHorizontal;
	private float raySpacingVertical;

	public LayerMask collisionMask;

	private BoxCollider2D coll;
	private Bounds colliderBounds;


	private float skinWidth = 0.1f;

	private RaycastOrigins raycastOrigins;
	public CollisionInfo collisionInfo;

	private void Awake() {
		coll = GetComponent<BoxCollider2D> ();
		CalculateBounds ();
		CalculateRaySpacing ();
	}

	private void CalculateBounds() {
		colliderBounds = coll.bounds;
		colliderBounds.Expand (-skinWidth * 2);

		raycastOrigins = new RaycastOrigins ();
		raycastOrigins.topLeft = new Vector2 (colliderBounds.min.x, colliderBounds.max.y);
		raycastOrigins.topRight = new Vector2 (colliderBounds.max.x, colliderBounds.max.y);
		raycastOrigins.bottomLeft = new Vector2 (colliderBounds.min.x, colliderBounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (colliderBounds.max.x, colliderBounds.min.y);
	}

	private void CalculateRaySpacing() {
		if (numberOfRayHorizontal < 3) {
			numberOfRayHorizontal = 3;
		}

		if (numberOfRayVertical < 3) {
			numberOfRayVertical = 3;
		}

		raySpacingVertical = (colliderBounds.extents.x * 2) / (numberOfRayVertical - 1);
		raySpacingHorizontal = ((colliderBounds.extents.y * 2) / (numberOfRayHorizontal - 1));
	}

	public void Move(Vector2 velocity) {
		CalculateBounds ();
		collisionInfo.Reset ();

		if (velocity.y != 0) {
			RaycastVertical (ref velocity);
		}

		if(velocity.x != 0)
		{
			RaycastHorizontal(ref velocity);
		}

		transform.position += (Vector3)velocity;
	}

	private void RaycastHorizontal(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.x);
		float distance = Mathf.Abs(velocity.x);
		Vector2 raycastBase = direction < 0 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

		for(int i=0; i < numberOfRayHorizontal; i++)
		{
			Vector2 raycastOrigin = raycastBase + raySpacingHorizontal * i * Vector2.up;

			RaycastHit2D[] hits = 
				Physics2D.RaycastAll(raycastOrigin, new Vector2(direction, 0), distance + skinWidth, collisionMask);

			Debug.DrawRay(raycastOrigin, new Vector2(velocity.x, 0), Color.green, Time.fixedDeltaTime, false);

			foreach(RaycastHit2D hit in hits)
			{
				if (hit.collider.isTrigger)
				{
					hit.collider.SendMessage("OnTriggerEnter2D", coll);
				}
				else
				{
					if (Mathf.Abs(velocity.x) > (hit.distance - skinWidth) * direction)
					{
						velocity.x = (hit.distance - skinWidth) * direction;
					}
					if (direction > 0) {
						// Right
						collisionInfo.collideRightArr[i] = true;
					} else {
						// Left
						collisionInfo.collideLeftArr[i] = true;
					}
				}
			}
		}
	}

	private void RaycastVertical(ref Vector2 velocity) {
		float direction = Mathf.Sign(velocity.y);
		float distance = Mathf.Abs(velocity.y);
		Vector2 raycastBase = direction < 0 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;

		for (int i = 0; i < numberOfRayVertical; i++)
		{
			Vector2 raycastOrigin = raycastBase + raySpacingVertical * i * Vector2.right;

			RaycastHit2D[] hits =
				Physics2D.RaycastAll(raycastOrigin, new Vector2(0, direction), distance + skinWidth, collisionMask);
			
			Debug.DrawRay(raycastOrigin, new Vector2(0, velocity.y * 10), Color.yellow, Time.fixedDeltaTime, false);

			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.isTrigger)
				{
					hit.collider.SendMessage("OnTriggerEnter2D", coll);
				}
				else
				{
					if(Mathf.Abs(velocity.y) > (hit.distance - skinWidth) * direction)
					{
						velocity.y = (hit.distance - skinWidth) * direction;
					}
					if (direction > 0) {
						// Top
						collisionInfo.collideTopArr[i] = true;
					} else {
						//Bottom
						collisionInfo.collideBottomArr[i] = true;
					}
				}
			}
		}
	}

}

public struct RaycastOrigins {
	public Vector2 topLeft, topRight;
	public Vector2 bottomLeft, bottomRight;
}

public struct CollisionInfo
{
	public bool[] collideTopArr;
	public bool[] collideBottomArr;
	public bool[] collideRightArr;
	public bool[] collideLeftArr;

	public void Reset()
	{
		collideTopArr = new bool[] { false, false, false };
		collideBottomArr = new bool[] { false, false, false };
		collideLeftArr = new bool[] { false, false, false };
		collideRightArr = new bool[] { false, false, false };
	}

	public bool collideBottom {
		get {
			return collideBottomCount > 0;
		}
	}

	public int collideBottomCount {
		get {
			return this.collideBottomArr.Count (item => item);
		}
	}

	public bool collideTop {
		get {
			return collideTopCount > 0;
		}
	}

	public int collideTopCount {
		get {
			return collideTopArr.Count (item => item);
		}
	}

	public bool collideRight {
		get {
			return collideRightCount > 0;
		}
	}

	public int collideRightCount {
		get {
			return collideRightArr.Count (item => item);
		}
	}

	public bool collideLeft {
		get {
			return collideLeftCount > 0;
		}
	}

	public int collideLeftCount {
		get {
			return collideLeftArr.Count (item => item);
		}
	}
}