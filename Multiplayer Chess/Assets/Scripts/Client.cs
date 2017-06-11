using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
	private bool socketReady;
	private TcpClient socket;
	private NetworkStream stream;
	private StreamWriter writer;
	private StreamReader reader;

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
		Debug.Log(data);
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