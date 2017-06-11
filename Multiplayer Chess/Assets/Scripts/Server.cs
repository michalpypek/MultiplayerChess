using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server : MonoBehaviour
{
	public int port = 6321;

	private List<ServerClient> clients;
	private List<ServerClient> disconnectList;

	private TcpListener server;
	private bool serverStarted;

	public void Init()
	{
		DontDestroyOnLoad(gameObject);
		clients = new List<ServerClient>();
		disconnectList = new List<ServerClient>();

		try
		{
			server = new TcpListener(IPAddress.Any,port);
			server.Start();

			StartListening();
			serverStarted = true;
		}
		catch (Exception e)
		{
			Debug.Log("Socket error: " + e.Message);			
		}
	}

	private void Update()
	{
		if(!serverStarted)
		{
			return;
		}

		foreach(ServerClient client in clients)
		{
			// Client still connected?
			if(!IsConnected(client.tcp))
			{
				client.tcp.Close();
				disconnectList.Add(client);
				continue;
			}

			else
			{
				NetworkStream stream = client.tcp.GetStream();
				if(stream.DataAvailable)
				{
					StreamReader reader = new StreamReader(stream, true);
					string data = reader.ReadLine();

					if(data != null)
					{
						OnIncomingData(client, data);
					}
				}
			}
		}

		for (int i = 0; i < disconnectList.Count - 1; i++)
		{
			//Tell the player somebody has disconnected

			clients.Remove(disconnectList[i]);
			disconnectList.RemoveAt(i);
		}

	}

	private void StartListening()
	{
		server.BeginAcceptTcpClient(AcceptTcpClient, server);
	}

	private void AcceptTcpClient(IAsyncResult ar)
	{
		TcpListener listener = (TcpListener)ar.AsyncState;

		ServerClient client = new ServerClient(listener.EndAcceptTcpClient(ar));
		clients.Add(client);

		StartListening();

		Debug.Log("Somebody has connected");
	}

	private bool IsConnected(TcpClient client)
	{
		try
		{
			if(client != null && client.Client != null && client.Client.Connected)
			{
				if(client.Client.Poll(0,SelectMode.SelectRead))
				{
					return !(client.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
				}
				return true;
			}

			else
			{
				return false;
			}
		}

		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Server Broadcast (send)
	/// </summary>
	/// <param name="data"></param>
	/// <param name="_clients"></param>
	private void Broadcast(string data, List<ServerClient> _clients)
	{
		foreach(var client in _clients)
		{
			try
			{
				StreamWriter writer = new StreamWriter(client.tcp.GetStream());
				writer.WriteLine(data);
				writer.Flush();
			}
			catch
			{

				Debug.Log("Error");
			}
		}
	}

	/// <summary>
	/// Server Read
	/// </summary>
	/// <param name="client"></param>
	/// <param name="data"></param>
	private void OnIncomingData (ServerClient client, string data)
	{
		Debug.Log(client.clientName + " : " + data);
	}
}

public class ServerClient
{
	public string clientName;
	public TcpClient tcp;

	public ServerClient(TcpClient tcp)
	{
		this.tcp = tcp;
	}
}