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

	public List<ServerClient> clients;
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
		var names = "";
		foreach (var cl in clients)
		{
			names += cl.clientName + "|";
		}
		ServerClient client = new ServerClient(listener.EndAcceptTcpClient(ar));
		clients.Add(client);

		StartListening();

		Broadcast("SWHO|"+names, client);
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
		Debug.Log("Broadcast: " + data);
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

	private void Broadcast(string data, ServerClient _client)
	{
		List<ServerClient> l = new List<ServerClient>();
		l.Add(_client);
		Broadcast(data, l);
	}

	/// <summary>
	/// Server Read
	/// </summary>
	/// <param name="client"></param>
	/// <param name="data"></param>
	private void OnIncomingData (ServerClient client, string data)
	{
		Debug.Log(data);

		string[] aData = data.Split('|');

		switch (aData[0])
		{
			case "CWHO":
				client.clientName = aData[1];
				client.isHost = (aData[2] == "1") ? true : false;
				Broadcast("SCNN|" + client.clientName, clients);
				break;
			case "CMOV":
				Broadcast("SMOV"+"|"+aData[1]+"|"+aData[2] + "|" +aData[3]+ "|" + aData[4] + "|" + aData[5], clients);
				break;
			case "CLOS":
				Broadcast("SLOS|" + aData[1], clients);
				break;
		}
	}
}

public class ServerClient
{
	public string clientName;
	public TcpClient tcp;
	public bool isHost;

	public ServerClient(TcpClient tcp)
	{
		this.tcp = tcp;
	}
}