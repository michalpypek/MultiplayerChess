using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Color { White, Black};
public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King};

public class ChessBoard : MonoBehaviour
{
	//the matrix that actually holds values
	public GridCell[][] board;

	//matrix used to spawn sprites at correct positions only
	public GameObject[][] gridSprites;

	//GameObjects displayed in the scene
	public GameObject[][] piecesObjects;

	[Header("Prefabs")]
	public GameObject cellPrefab;
	public GameObject clickableCellPrefab;

	[Header("White Pieces Prefabs")]
	public GameObject whitePawnPrefab;
	public GameObject whiteKnightPrefab;
	public GameObject whiteRookPrefab;
	public GameObject whiteBishopPrefab;
	public GameObject whiteQueenPrefab;
	public GameObject whiteKingPrefab;

	[Header("Black Pieces Prefabs")]
	public GameObject blackPawnPrefab;
	public GameObject blackKnightPrefab;
	public GameObject blackRookPrefab;
	public GameObject blackBishopPrefab;
	public GameObject blackQueenPrefab;
	public GameObject blackKingPrefab;

	public int boardSize = 8; 

	public bool isWhite;

	List<GameObject> clickableCells = new List<GameObject>();
	Piece selectedPiece;
	Client client;

	void Start()
	{
		Init();
	}

	/// <summary>
	/// Create the chessboard
	/// </summary>
	public void Init()
	{
		//client = FindObjectOfType<Client>();
		//isWhite = client.isHost;

		for (int i = 0; i < 32; i++)
		{
			var go = Instantiate(clickableCellPrefab, transform);
			go.SetActive(false);
			clickableCells.Add(go);
		}

		board = new GridCell[boardSize][];
		for (int i = 0; i < boardSize; i++)
		{
			board[i] = new GridCell[boardSize];
		}

		for (int y = 0; y < boardSize; y++)
		{
			for (int x = 0; x < boardSize; x++)
			{
				board[x][y] = new GridCell();
			}
		}


		piecesObjects = new GameObject[boardSize][];
		for (int i = 0; i < boardSize; i++)
		{
			piecesObjects[i] = new GameObject[boardSize];
		}

		bool black = true;
		for (int y = 0; y < boardSize; y++)
		{
			for (int x = 0; x < boardSize; x++)
			{
				board[x][y].color = (black ? Color.Black : Color.White);
				black = !black;
				if (x == boardSize - 1)
				{
					black = !black;
				}
			}
		}

		SetPiecesInMatrix();

		GenerateVisuals();

		CalculateMovesForAllPieces();
	}

	public void CalculateMovesForAllPieces()
	{


		foreach (var row in board)
		{
			foreach (var cell in row)
			{
				if(!cell.isEmpty)
				{
					cell.piece.availablePositions = GetPossibleMovesForPiece(cell.piece);
				}
			}
		}
	}

	public void SelectPiece(int x, int y)
	{
		Debug.Log(x + " " + y);
		selectedPiece = board[x][y].piece;
		Debug.Log("Selected: " + selectedPiece.color + " " + selectedPiece.type +" at " + selectedPiece.posX +" " +selectedPiece.posY );

		foreach (var item in selectedPiece.availablePositions)
		{
			Debug.Log(item);
		}
		HighlightAvailableMovePositions();
	}

	public void MovePiece (Piece p, Vector2 targetPos, GameObject pieceObj)
	{
		board[p.posX][p.posY].isEmpty = true;
		piecesObjects[p.posX][p.posY] = null;

		Debug.Log("Before: " + p.posX + "  : " + p.posY);
		p.posX = (int)targetPos.x;
		p.posY = (int)targetPos.y;

		if (!board[(int)targetPos.x][(int)targetPos.y].isEmpty)
		{
			if (piecesObjects[(int)targetPos.x][(int)targetPos.y] != null)
			{
				Destroy(piecesObjects[(int)targetPos.x][(int)targetPos.y]);
			}
		}


		board[(int)targetPos.x][(int)targetPos.y].piece = p;
		board[(int)targetPos.x][(int)targetPos.y].isEmpty = false;
		piecesObjects[(int)targetPos.x][(int)targetPos.y] = pieceObj;
		pieceObj.transform.position = targetPos;
		Debug.Log("After: " + p.posX + "  : " + p.posY);

		foreach (var item in clickableCells)
		{
			item.SetActive(false);
		}

		CalculateMovesForAllPieces();
	}

	public void MovePiece (Vector2 targetPos)
	{
		MovePiece(selectedPiece, targetPos, piecesObjects[selectedPiece.posX][selectedPiece.posY]);
	}

	void HighlightAvailableMovePositions()
	{
		foreach (var item in clickableCells)
		{
			item.SetActive(false);
		}


		foreach (var pos in selectedPiece.availablePositions)
		{
			var cl = GetClickableCell();
			cl.transform.position = pos;
			cl.SetActive(true);
		}
	}

	private void SetPiecesInMatrix()
	{
		//Pawns
		for (int x = 0; x < boardSize; x++)
		{
			board[x][1].piece.type = PieceType.Pawn;
			board[x][1].piece.color = Color.White;
			board[x][1].isEmpty = false;
		}

		for (int x = 0; x < boardSize; x++)
		{
			board[x][boardSize - 2].piece.type = PieceType.Pawn;
			board[x][boardSize - 2].piece.color = Color.Black;
			board[x][boardSize - 2].isEmpty = false;
		}

		//White Pieces
		board[0][0].piece.type = PieceType.Rook;
		board[0][0].piece.color = Color.White;
		board[0][0].isEmpty = false;
		board[boardSize - 1][0].piece.type =PieceType.Rook;
		board[boardSize - 1][0].piece.color = Color.White;
		board[boardSize - 1][0].isEmpty = false;

		board[1][0].piece.type =PieceType.Knight;
		board[1][0].piece.color = Color.White;
		board[1][0].isEmpty = false;
		board[boardSize - 2][0].piece.type =PieceType.Knight;
		board[boardSize - 2][0].piece.color = Color.White;
		board[boardSize - 2][0].isEmpty = false;

		board[boardSize - 3][0].piece.type =PieceType.Bishop;
		board[boardSize - 3][0].piece.color = Color.White;
		board[boardSize - 3][0].isEmpty = false;
		board[2][0].piece.type =PieceType.Bishop;
		board[2][0].piece.color = Color.White;
		board[2][0].isEmpty = false;

		board[boardSize - 4][0].piece.type =PieceType.King;
		board[boardSize - 4][0].piece.color = Color.White;
		board[boardSize - 4][0].isEmpty = false;
		board[boardSize - 5][0].piece.type =PieceType.Queen;
		board[boardSize - 5][0].piece.color = Color.White;
		board[boardSize - 5][0].isEmpty = false;

		//Black Pieces
		board[0][boardSize - 1].piece.type =PieceType.Rook;
		board[0][boardSize - 1].piece.color = Color.Black;
		board[0][boardSize - 1].isEmpty = false;
		board[boardSize - 1][boardSize - 1].piece.type =PieceType.Rook;
		board[boardSize - 1][boardSize - 1].piece.color = Color.Black;
		board[boardSize - 1][boardSize - 1].isEmpty = false;

		board[1][boardSize - 1].piece.type =PieceType.Knight;
		board[1][boardSize - 1].piece.color = Color.Black;
		board[1][boardSize - 1].isEmpty = false;
		board[boardSize - 2][boardSize - 1].piece.type =PieceType.Knight;
		board[boardSize - 2][boardSize - 1].piece.color = Color.Black;
		board[boardSize - 2][boardSize - 1].isEmpty = false;


		board[boardSize - 3][boardSize - 1].piece.type =PieceType.Bishop;
		board[boardSize - 3][boardSize - 1].piece.color = Color.Black;
		board[boardSize - 3][boardSize - 1].isEmpty = false;
		board[2][boardSize - 1].piece.type =PieceType.Bishop;
		board[2][boardSize - 1].piece.color = Color.Black;
		board[2][boardSize - 1].isEmpty = false;


		board[boardSize - 4][boardSize - 1].piece.type =PieceType.King;
		board[boardSize - 4][boardSize - 1].piece.color = Color.Black;
		board[boardSize - 4][boardSize - 1].isEmpty = false;


		board[boardSize - 5][boardSize - 1].piece.type =PieceType.Queen;
		board[boardSize - 5][boardSize - 1].piece.color = Color.Black;
		board[boardSize - 5][boardSize - 1].isEmpty = false;

		for (int x = 0; x < boardSize; x++)
		{
			for (int y = 0; y < boardSize; y++)
			{
				board[x][y].piece.posX = x;
				board[x][y].piece.posY = y;
			}
		}
	}

	/// <summary>
	/// Spawn the sprites
	/// </summary>
	private void GenerateVisuals()
	{
		gridSprites = new GameObject[boardSize][];
		for (int i = 0; i < boardSize; i++)
		{
			gridSprites[i] = new GameObject[boardSize];
		}

		StartCoroutine(SpawnCellsAndPieces());
	}

	/// <summary>
	/// Spawns sprites for the grid and the pieces
	/// </summary>
	/// <returns></returns>
	IEnumerator SpawnCellsAndPieces()
	{
		bool black = true;
		for (int y = 0; y < boardSize; y++)
		{
			for (int x = 0; x < boardSize; x++)
			{
				var cell = Instantiate(cellPrefab, new Vector2(x, y), Quaternion.identity);
				gridSprites[x][y] = cell;
				if (black)
				{
					cell.GetComponent<SpriteRenderer>().color = UnityEngine.Color.gray;
				}
				black = !black;
				if(x == boardSize -1)
					{
						black = !black;
					}
				yield return new WaitForSeconds(0.005f);
			}
		}

		for (int y = 0; y < boardSize; y++)
		{
			for (int x = 0; x < boardSize; x++)
			{
				if (!board[x][y].isEmpty)
				{
					var go = (GameObject)Instantiate(SelectPrefabToSpawn(board[x][y].piece.color, board[x][y].piece.type), new Vector2(x, y), Quaternion.identity);
					piecesObjects[x][y] = go;
					yield return new WaitForSeconds(0.005f);
				}
			}
		}
	}

	private GameObject SelectPrefabToSpawn(Color col, PieceType type)
	{
		switch (col)
		{
			case Color.White:
				return SelectWhitePiece(type);
			case Color.Black:
				return SelectBlackPiece(type);
		}
		return null;
	}

	private GameObject SelectWhitePiece(PieceType type)
	{
		switch (type)
		{
			case PieceType.Pawn:
				return whitePawnPrefab;
			case PieceType.Rook:
				return whiteRookPrefab;
			case PieceType.Knight:
				return whiteKnightPrefab;
			case PieceType.Bishop:
				return whiteBishopPrefab;
			case PieceType.Queen:
				return whiteQueenPrefab;
			case PieceType.King:
				return whiteKingPrefab;
		}
		return null;
	}

	private GameObject SelectBlackPiece(PieceType type)
	{
		switch (type)
		{
			case PieceType.Pawn:
				return blackPawnPrefab;
			case PieceType.Rook:
				return blackRookPrefab;
			case PieceType.Knight:
				return blackKnightPrefab;
			case PieceType.Bishop:
				return blackBishopPrefab;
			case PieceType.Queen:
				return blackQueenPrefab;
			case PieceType.King:
				return blackKingPrefab;
		}
		return null;
	}

	List<Vector2> GetPossibleMovesForPiece(Piece piece)
	{
		switch (piece.type)
		{
			case PieceType.Pawn:
				return GetPossibleMovesForPawn(piece.posX, piece.posY, piece.color);
			case PieceType.Rook:
				return GetPossibleMovesForRook(piece.posX, piece.posY, piece.color);
			case PieceType.Knight:
				return GetPossibleMovesForKnight(piece.posX, piece.posY, piece.color);
			case PieceType.Bishop:
				return GetPossibleMovesForBishop(piece.posX, piece.posY, piece.color);
			case PieceType.Queen:
				return GetPossibleMovesForQueen(piece.posX, piece.posY, piece.color);
			case PieceType.King:
				return GetPossibleMovesForKing(piece.posX, piece.posY, piece.color);
		}

		return null;
	}

	List<Vector2> GetPossibleMovesForPawn(int x, int y, Color col)
	{
		var moves = new List<Vector2>();
		if (col == Color.White)
		{
			if (y + 1 < boardSize && board[x][y + 1].isEmpty)
			{
				moves.Add(new Vector2(x, y + 1));
			}

			if (x + 1 < boardSize && y + 1 < boardSize && !board[x + 1][y + 1].isEmpty)
			{
				if (board[x + 1][y + 1].piece.color != col)
				{
					moves.Add(new Vector2(x + 1, y + 1));
				}
			}

			if (x - 1 >= 0 && y + 1 < boardSize && !board[x - 1][y + 1].isEmpty)
			{
				if (board[x - 1][y - 1].piece.color != col)
				{
					moves.Add(new Vector2(x - 1, y + 1));
				}
			}
		}

		else
		{
			if (y - 1 >= 0 && board[x][y - 1].isEmpty)
			{
				moves.Add(new Vector2(x, y - 1));
			}

			if (x + 1 < boardSize && y - 1 >= 0 && !board[x + 1][y - 1].isEmpty)
			{
				if (board[x + 1][y + 1].piece.color != col)
				{
					moves.Add(new Vector2(x + 1, y - 1));
				}
			}

			if (x - 1 >= 0 && y - 1 >= 0 && !board[x - 1][y - 1].isEmpty)
			{
				if (board[x - 1][y - 1].piece.color != col)
				{
					moves.Add(new Vector2(x - 1, y - 1));
				}
			}
		}
		return moves;
	}

	List<Vector2> GetPossibleMovesForRook(int x, int y, Color col)
	{
		var moves = new List<Vector2>();

		for (int _x = x+1; _x < boardSize; _x++)
		{
			if(board[_x][y].isEmpty)
			{
				moves.Add(new Vector2(_x, y));
			}
			else
			{
				if(board[_x][y].piece.color != col)
				{
					moves.Add(new Vector2(_x, y));
					_x = boardSize + 1;
				}
				else
				{
					_x = boardSize + 1;
				}
			}
		}

		for (int _x = x - 1; _x >= 0; _x--)
		{
			if (board[_x][y].isEmpty)
			{
				moves.Add(new Vector2(_x, y));
			}
			else
			{
				if (board[_x][y].piece.color != col)
				{
					moves.Add(new Vector2(_x, y));
					_x = -2;
				}
				else
				{
					_x = -2;
				}
			}
		}

		for (int _y = y + 1; _y < boardSize; _y++)
		{
			if (board[x][_y].isEmpty)
			{
				moves.Add(new Vector2(x, _y));
			}
			else
			{
				if (board[x][_y].piece.color != col)
				{
					moves.Add(new Vector2(x, _y));
					_y = boardSize + 1;
				}
				else
				{
					_y = boardSize + 1;
				}
			}
		}

		for (int _y = y - 1; _y >= 0; _y--)
		{
			if (board[x][_y].isEmpty)
			{
				moves.Add(new Vector2(x, _y));
			}
			else
			{
				if (board[x][_y].piece.color != col)
				{
					moves.Add(new Vector2(x, _y));
					_y = -2;
				}
				else
				{
					_y = -2;
				}
			}
		}

		return moves;

	}

	List<Vector2> GetPossibleMovesForKnight(int x, int y, Color col)
	{
		var moves = new List<Vector2>();
		if (y+2 < boardSize)
		{
			if(x+1 < boardSize)
			{
				if(board[x+1][y+2].isEmpty)
				{
					moves.Add(new Vector2(x + 1, y + 2));
				}

				if(board[x+1][y+2].piece.color != col)
				{
					moves.Add(new Vector2(x + 1, y + 2));
				}
			}

			if(x-1 >= 0)
			{
				if (board[x - 1][y + 2].isEmpty)
				{
					moves.Add(new Vector2(x - 1, y + 2));
				}

				if (board[x - 1][y + 2].piece.color != col)
				{
					moves.Add(new Vector2(x - 1, y + 2));
				}


			}
		}

		if (y - 2 >= 0)
		{
			if (x + 1 < boardSize)
			{
				if (board[x + 1][y - 2].isEmpty)
				{
					moves.Add(new Vector2(x + 1, y - 2));
				}

				if (board[x + 1][y - 2].piece.color != col)
				{
					moves.Add(new Vector2(x + 1, y - 2));
				}
			}

			if (x - 1 >= 0)
			{
				if (board[x - 1][y - 2].isEmpty)
				{
					moves.Add(new Vector2(x - 1, y - 2));
				}

				if (board[x - 1][y - 2].piece.color != col)
				{
					moves.Add(new Vector2(x - 1, y - 2));
				}
			}
		}

		if (x+2< boardSize)
		{
			if(y+1<boardSize)
			{
				if (board[x + 2][y + 1].isEmpty)
				{
					moves.Add(new Vector2(x + 2, y + 1));
				}

				if (board[x + 2][y + 1].piece.color != col)
				{
					moves.Add(new Vector2(x + 2, y + 1));
				}
			}

			if(y-1 >=0)
			{
				if (board[x + 2][y - 1].isEmpty)
				{
					moves.Add(new Vector2(x + 2, y - 1));
				}

				if (board[x + 2][y - 1].piece.color != col)
				{
					moves.Add(new Vector2(x + 2, y - 1));
				}

			}
		}

		if (x - 2 >= 0)
		{
			if (y + 1 < boardSize)
			{
				if (board[x - 2][y + 1].isEmpty)
				{
					moves.Add(new Vector2(x - 2, y + 1));
				}

				if (board[x - 2][y + 1].piece.color != col)
				{
					moves.Add(new Vector2(x - 2, y + 1));
				}
			}
			if (y - 1 >= 0)
			{
				if (board[x - 2][y - 1].isEmpty)
				{
					moves.Add(new Vector2(x - 2, y - 1));
				}

				if (board[x - 2][y - 1].piece.color != col)
				{
					moves.Add(new Vector2(x - 2, y - 1));
				}
			}
		}
		return moves;

	}

	List<Vector2> GetPossibleMovesForBishop(int x, int y, Color col)
	{
		var moves = new List<Vector2>();

		for (int _x = x + 1, _y = y + 1; _x < boardSize && _y < boardSize; _x++)
		{			
				if (board[_x][_y].isEmpty)
				{
					moves.Add(new Vector2(_x, _y));
				}
				else
				{
					if (board[_x][_y].piece.color != col)
					{
						moves.Add(new Vector2(_x, _y));
						_y = boardSize + 1;
						_x = boardSize + 1;
					}
					else
					{
						_y = boardSize + 1;
						_x = boardSize + 1;
					}
				}
			_y++;			
		}

		for (int _x = x + 1, _y = y - 1; _x < boardSize && _y >= 0; _x++)
		{

				if (board[_x][_y].isEmpty)
				{
					moves.Add(new Vector2(_x, _y));
				}
				else
				{
					if (board[_x][_y].piece.color != col)
					{
						moves.Add(new Vector2(_x, _y));
						_y = -2;
						_x = boardSize + 1;
					}
					else
					{
						_y = -2;
						_x = boardSize + 1;
					}
				}
			_y--;
		}

		for (int _x = x - 1, _y = y - 1; _x >= 0 && _y >= 0; _x--)
		{
			if (board[_x][_y].isEmpty)
				{
					moves.Add(new Vector2(_x, _y));
				}
				else
				{
					if (board[_x][_y].piece.color != col)
					{
						moves.Add(new Vector2(_x, _y));
						_y = -2;
						_x = -1;
					}
					else
					{
						_y = -2;
						_x = -1;
					}
				}
			_y--;
		}

		for (int _x = x - 1, _y = y + 1; _x >= 0 && _y < boardSize; _x--)
		{

				if (board[_x][_y].isEmpty)
				{
					moves.Add(new Vector2(_x, _y));
				}
				else
				{
					if (board[_x][_y].piece.color != col)
					{
						moves.Add(new Vector2(_x, _y));
						_y = boardSize + 1;
						_x = -1;
					}
					else
					{
						_y = boardSize + 1;
						_x = -1;
					}
				}
			_y++;
		}

		return moves;
	}

	List<Vector2> GetPossibleMovesForQueen(int x, int y, Color col)
	{
		var moves = new List<Vector2>();
		moves = moves.Union(GetPossibleMovesForBishop(x, y, col)).ToList();
		moves = moves.Union(GetPossibleMovesForRook(x, y, col)).ToList();

		return moves;
	}

	List<Vector2> GetPossibleMovesForKing(int x, int y, Color col)
	{
		var moves = new List<Vector2>();
		return moves;

	}

	private GameObject GetClickableCell()
	{
		foreach (var item in clickableCells)
		{
			if(!item.activeSelf)
			{
				return item;
			}
		}
		return null;
	}
}


public class GridCell
{
	public Color color;
	public Piece piece = new Piece();
	public bool isEmpty = true;
}
