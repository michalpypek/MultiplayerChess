using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
	public string clientName;
	public bool isHost;

	private bool socketReady;
	private TcpClient socket;
	private NetworkStream stream;
	private StreamWriter writer;
	private StreamReader reader;
	private List<GameClient> players = new List<GameClient>();

	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	private void Update()
	{
		if(socketReady)
		{
			if (stream.DataAvailable)
			{
				string data = reader.ReadLine();
				if(data!= null)
				{
					OnIncomingData(data);
				}
			}
		}
	}

	private void OnApplicationQuit()
	{
		CloseSocket();
	}

	private void OnDisable()
	{
		CloseSocket();
	}

	public bool ConnectToServer(string host, int port)
	{
		if (socketReady)
		{
			return false;
		}

		try
		{
			socket = new TcpClient(host, port);
			stream = socket.GetStream();
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);

			socketReady = true;
		}

		catch (Exception e)
		{
			Debug.Log("socket error " + e.Message);
		}

		return socketReady;
	}

	/// <summary>
	/// Send data to the server
	/// </summary>
	/// <param name="data"></param>
	public void Send(string data)
	{
		if (!socketReady)
		{
			return;
		}

		writer.WriteLine(data);
		writer.Flush();
	}

	/// <summary>
	/// Receive data from server
	/// </summary>
	/// <param name="data"></param>
	private void OnIncomingData(string data)
	{
		string[] aData = data.Split('|');

		switch (aData[0])
		{
			case "SWHO":
				for(int i = 1; i < aData.Length -1; i++)
				{
					UserConnected(aData[i], false);
				}
				Send("CWHO|" + clientName + "|" + (isHost? 1: 0));
				break;
			case "SCNN":
				UserConnected(aData[1], false);
				break;
			case "SMOV":
				if(clientName != aData[1])
				{
					GameManager.instance.chessBoard.MovePieceFromServer(int.Parse(aData[2]), int.Parse(aData[3]), int.Parse(aData[4]), int.Parse(aData[5]));
				}
				break;
			case "SLOS":
				if(clientName != aData[1])
				{
					GameManager.instance.WinScreen();
				}
				else
				{
					GameManager.instance.LoseScreen();
				}
				break;
		}
		Debug.Log(data);
	}

	private void UserConnected(string name, bool isHost)
	{
		GameClient c = new GameClient();
		c.name = name;

		players.Add(c);

		if (players.Count == 2)
		{
			GameManager.instance.StartGame();
		}
	}

	private void CloseSocket()
	{
		if(!socketReady)
		{
			return;
		}

		writer.Close();
		reader.Close();
		socket.Close();
		socketReady = false;
	}
}

public class GameClient
{
	public string name;
	public bool isHost;

}