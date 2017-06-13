using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
	public class Piece
	{
		public int posX;
		public int posY;
		public Color color;
		public List<UnityEngine.Vector2> availablePositions;
		public PieceType type;
	}
}
