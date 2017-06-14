using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public ChessBoard chessBoard;

	public GameObject buttonHolder;
	public GameObject connectMenu;
	public GameObject hostMenu;
	public GameObject mainMenu;
	public GameObject gameMenu;
	public GameObject winScreen;
	public GameObject loseScreen;
	public Text turnText;
	public Text checkText;

	public GameObject serverPrefab;
	public GameObject clientPrefab;
	public InputField hostInput;
	public InputField nameInput;

	void Start()
	{
		if(instance == null)
		{
			instance = this;
		}

		else
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		buttonHolder.SetActive(true);
		connectMenu.SetActive(false);
		hostMenu.SetActive(false);
	}

	void Update()
	{
		turnText.text = "Current turn: player " + (chessBoard.whiteTurn ? "white" : "black");
		checkText.gameObject.SetActive(chessBoard.isChecked);
	}

	public void JoinButton()
	{
		buttonHolder.SetActive(false);
		connectMenu.SetActive(true);
		hostMenu.SetActive(false);
	}

	public void HostButton()
	{
		try
		{
			var server = Instantiate(serverPrefab).GetComponent<Server>();
			server.Init();

			var c = Instantiate(clientPrefab).GetComponent<Client>();
			c.clientName = nameInput.text;
			c.isHost = true;
			if (c.clientName == "")
			{
				c.clientName = "Host";
			}
			c.ConnectToServer("127.0.0.1", 6321);
		}
		catch (System.Exception e)
		{

			Debug.Log(e.Message);
		}

		buttonHolder.SetActive(false);
		connectMenu.SetActive(false);
		hostMenu.SetActive(true);
	}

	public void ConnectButton()
	{
		string host = hostInput.text;
		if(host == "")
		{
			host = "127.0.0.1";
		}

		try
		{
			var c = Instantiate(clientPrefab).GetComponent<Client>();
			c.clientName = nameInput.text;
			if(c.clientName == "")
			{
				c.clientName = "Client";
			}
			c.ConnectToServer(host, 6321);
			connectMenu.SetActive(false);
		}
		catch (System.Exception e)
		{

			Debug.Log(e.Message);
		}

	}

	public void BackButton()
	{
		buttonHolder.SetActive(true);
		connectMenu.SetActive(false);
		hostMenu.SetActive(false);

		var server = GameObject.FindObjectOfType<Server>();
		if(server != null)
		{
			Destroy(server.gameObject);
		}

		var cl = GameObject.FindObjectOfType<Client>();

		if(cl != null)
		{
			Destroy(cl.gameObject);
		}
	}

	public void StartGame()
	{
		mainMenu.SetActive(false);
		gameMenu.SetActive(true);
		chessBoard.Init();
	}

	public void WinScreen()
	{
		winScreen.SetActive(true);
	}

	public void LoseScreen()
	{
		loseScreen.SetActive(true);
	}
}
