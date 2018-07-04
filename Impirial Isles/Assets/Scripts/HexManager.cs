using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexManager : MonoBehaviour {

	static float hexSize = 1;

	//hack
	public GameObject selection;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//HACK testing

        // Draw grid

		Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(cameraRay, out hit)) {
			Vector2Int hexCoord = WorldToHexCoords (hit.point);
			Vector3 pos = HexToWorldPos (hexCoord);
			selection.transform.position = pos;
			selection.GetComponentInChildren<Text> ().text = hexCoord.ToString ();
			//if (Input.GetKey(KeyCode.LeftControl)) {
			//	Debug.Log (Input.mousePosition);
			//}
		}
	}


	static Vector2Int WorldToHexCoords(Vector3 worldPos){
		//TODO when X is odd, line on Y axis treated as hexes on either side
		float invHexWidth = Mathf.Sqrt (3) * 0.5f;
        float oneThird = 1.0f / 3.0f;

		// Convert world position to scale where distance between hex rows and columns is 1
		worldPos /= hexSize;
		worldPos.x = worldPos.x * invHexWidth * 4.0f * oneThird;
		Vector2Int hexCoord = Vector2Int.zero;
		// HACK figure out more elegant way to do this

		// Get possible x and y coordinates
		int x1 = Mathf.FloorToInt(worldPos.x);
		int x2 = Mathf.CeilToInt(worldPos.x);
		int y = Mathf.FloorToInt (worldPos.z);

        float cornerX1 = (float)x1 + 2.0f * oneThird;
        float cornerX2 = (float)x2 - 2.0f * oneThird;

        // each 1x1 square can be 3 hexes: whichever x is even can have 2 possibilities
        Vector2Int hex1, hex2;
		if (x1 % 2 != 0) {
			int temp = x1;
			x1 = x2;
			x2 = temp;
            // If even is ceiling, centrepoint is at -1/3
            float temp2 = cornerX1;
            cornerX1 = cornerX2;
            cornerX2 = temp2;
		}
		hex1 = new Vector2Int (x1, y - x1 / 2);
		hex2 = new Vector2Int (x2, y - x2 / 2);
		//HACK if x negative, hex2 needs ceiling not floor
		if (x2 < 0) {
			hex2.y += 1;
		}

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
			
			Vector3 hex2Pos = new Vector3 (hex2.x, 0, hex2.y + 0.5f * (float)hex2.x);

            Vector3 corner1Pos = new Vector3(cornerX1, 0, hex1.y + 0.5f * (float)hex1.x) ;
            Vector3 corner2Pos = new Vector3(cornerX2, 0, hex2.y + 0.5f * (float)hex2.x);

            Vector3 edge = corner2Pos - corner1Pos;
            Vector3 edgeNormal = Vector3.Normalize(new Vector3(edge.z, 0, -edge.y));
			float offset = Vector3.Dot (corner1Pos, edgeNormal);

            Debug.DrawLine(corner1Pos, corner2Pos,Color.blue);

            // If hex2 centre and point on same side of line, chose hex2
			float pointDistance = Vector3.Dot (worldPos, edgeNormal) - offset;
            float hex2Distance = Vector3.Dot(hex2Pos, edgeNormal) - offset;
			if (pointDistance * hex2Distance > 0) {
				return hex2;
			} else {
				return hex1;
			}
		}
	}

	static Vector3 HexToWorldPos(Vector2Int hexCoord){

		float hexWidth = 2.0f / Mathf.Sqrt (3);

		Vector3 worldPos = Vector3.zero;
		worldPos.x = (float)hexCoord.x * 0.75f * hexWidth * hexSize;
		worldPos.z = ((float)hexCoord.y + 0.5f * (float)hexCoord.x) * hexSize;
		return worldPos;
	}

    private void OnDrawGizmos()
    {
        float hexWidth = hexSize * 2.0f / ( Mathf.Sqrt(3));
        for (int i = -10; i < 10; ++i) {
            float x1 = ((float)i * 0.75f + 0.25f) * hexWidth;
            float x2 = x1 + 0.25f * hexWidth;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(new Vector3(x1, 0, -10), new Vector3(x1, 0, 10));
            Gizmos.DrawLine(new Vector3(x2, 0, -10), new Vector3(x2, 0, 10));

            float z1 = (float)i * 0.5f * hexSize;
            Gizmos.DrawLine(new Vector3(-10, 0, z1), new Vector3(10, 0, z1));
        }
    }
}
