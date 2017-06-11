using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public GameObject buttonHolder;
	public GameObject connectMenu;
	public GameObject hostMenu;

	public GameObject serverPrefab;
	public GameObject clientPrefab;
	public InputField hostInput;

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
	}
}
