using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

public class UDPManager : MonoBehaviour {

	public delegate void ServerMovementEvent(Vector3 movement);
	public static event ServerMovementEvent OnServerMove;

	UdpClient client;
	uint receiveCounter = 1;
	bool isRunning = false;

	void OnEnable()
	{
		PlayerController.OnMove += SendMovementToServer;
	}
	
	void OnDisable()
	{
		PlayerController.OnMove -= SendMovementToServer;
	}

// Use this for initialization
	void Awake() {
		//client = new UdpClient("127.0.0.1", 3000);
		client = new UdpClient("172.25.48.13", 3000);

		//writing
		//byte[] sendData = Encoding.UTF8.GetBytes("Hello Server!");
		//client.Send (sendData, sendData.Length);
	}

	// Update is called once per frame
	void Update () {
		/*if (!isRunning)
			return;

		try {
			IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
			byte[] receivedData = client.Receive (ref ep);

			//when packet received
			Debug.Log (Encoding.UTF8.GetString (receivedData) + " received");

			if (receiveCounter == 10)
			{
				isRunning = false;
				client.Close();
			}
			else
			{
				receiveCounter++;
			}
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message.ToString());
		}*/

		if (!isRunning)
			return;

		try {
						
			isRunning = false;
			IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
			byte[] receivedData = client.Receive (ref ep);
				
			if (OnServerMove != null)
			{		
				Vector3 coord = DeserializeVector3(receivedData);
				OnServerMove(coord);

				Debug.Log("Cooordinates from server: " + coord);
			}

		}
		catch (Exception e)
		{
			Debug.LogError(e.Message.ToString());
		}
	}

	private void SendMovementToServer(Vector3 movement)
	{
		try {
			isRunning = true;
			byte[] sendData = SerializeVector3(movement);
			client.Send (sendData, sendData.Length);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
	}

	private byte[] SerializeVector3(Vector3 input)
	{
		byte[] bytes = new byte[12];

		Buffer.BlockCopy (BitConverter.GetBytes (input.x), 0, bytes, 0, 4);
		Buffer.BlockCopy (BitConverter.GetBytes (input.y), 0, bytes, 4, 4);
		Buffer.BlockCopy (BitConverter.GetBytes (input.z), 0, bytes, 8, 4);
	
		return bytes;
	}

	private Vector3 DeserializeVector3(byte[] input)
	{
		Vector3 result = Vector3.zero;

		result.x = BitConverter.ToSingle (input, 0);
		result.y = BitConverter.ToSingle (input, 4);
		result.z = BitConverter.ToSingle (input, 8);

		return result;
	}
}
