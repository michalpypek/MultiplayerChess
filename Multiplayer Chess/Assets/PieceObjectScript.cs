using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceObjectScript : MonoBehaviour
{
	public void OnMouseDown()
	{
		GameManager.instance.chessBoard.SelectPiece((int)transform.position.x, (int)transform.position.y);
	}
}
