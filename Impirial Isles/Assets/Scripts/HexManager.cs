using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexManager : MonoBehaviour {

	static float hexSize = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	static Vector2Int WorldToHexCoords(Vector3 worldPos){
		//TODO
		float invHexWidth = (2.0f * Mathf.Sqrt (3)) / 3.0f;

		// Convert world position to scale where hex width and height are 1
		worldPos /= hexSize;
		worldPos.x = worldPos.x * invHexWidth;
		Vector2Int hexCoord = Vector2Int.zero;
		// HACK figure out more elegant way to do this

		// Get possible x and y coordinates
		int x1 = Mathf.FloorToInt(worldPos.x);
		int x2 = Mathf.CeilToInt(worldPos.x);
		int y = Mathf.FloorToInt (worldPos.z);

		// each 1x1 square can be 3 hexes: whichever x is even can have 2 possibilities
		Vector2Int hex1, hex2;
		if (x1 % 2 != 0) {
			int temp = x1;
			x1 = x2;
			x2 = temp;
		}
		hex1 = Vector2Int (x1, y - x1 / 2);
		hex2 = Vector2Int (x2, y - x2 / 2);

		// If top half pick that one for even column hex
		if((worldPos.z - Mathf.Floor (worldPos.z)) > 0.5f){
			hex1.y = hex1.y + 1;
		}

		// If exactly at middle of column, pick odd or even based on remainder
		if (x1 == x2) {
			if (x1 % 2 == 0) {
				return hex1;
			} else {
				return hex2;
			}
		} else {
			// Otherwise check which side of line point is
			Vector3 centrePoint = Vector3 (Mathf.Floor (worldPos.x) + 0.5f, 0, Mathf.Floor (worldPos.z) + 0.5f);
			Vector3 hex2Pos = Vector3 (hex2.x, 0, hex2.y);
			Vector3 otherHexPos = Vector3 (hex1.x, 0, hex1.y);

			Vector3 normal = Vector3.Normalize (hex2Pos - otherHexPos);
			float offset = Vector3.Dot (centrePoint, normal);
			float d = Vector3.Dot (worldPos, normal) - offset;
			if (d > 0) {
				return hex2;
			} else {
				return hex1;
			}
		}
	}

	static Vector3 HexToWorldPos(Vector2Int hexCoord){

		float hexWidth = 3.0f / (2.0f * Mathf.Sqrt (3));

		Vector3 worldPos = Vector3.zero;
		worldPos.x = (float)hexCoord.x * hexWidth * hexSize;
		worldPos.z = ((float)hexCoord.y + 0.5f * (float)hexCoord.x) * hexSize;
		return worldPos;
	}
}
