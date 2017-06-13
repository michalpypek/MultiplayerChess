using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableCellScript : MonoBehaviour
{
	public void OnMouseDown()
	{
		GameManager.instance.chessBoard.MovePiece(transform.position);
	}
}
