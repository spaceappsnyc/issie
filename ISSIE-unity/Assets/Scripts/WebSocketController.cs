﻿using UnityEngine;
using System.Collections;

// Demo class
// inherits of WebSocketUnityDelegate to receive WebSockets events
public class WebSocketController : MonoBehaviour, WebSocketUnityDelegate {
	
	// boolean to manage display
	private bool sendingMessage = false;
	private bool receivedMessage = false;
	private string _message = "";
	public int force = 0;
	public string address = "issie.local:8001";
	public bool _debug = false;

	// Web Socket for Unity
	//    Desktop
	//    WebPlayer
	//    Android
	//    ios (+ ios simulator)
	//	  WebGL
	private WebSocketUnity webSocket;

	// Use this for initialization
	void Start () {
		if (!_debug){ 
			//autoconnect
			connectWebsocket(webSocket);
		}

	}

	public enum DIRECTION { UP, DOWN, LEFT, RIGHT, NONE, };
 	public class PowerSignal
 	{
	 	private int m_force;
 		private DIRECTION m_direction;
 		public PowerSignal(int force, DIRECTION direction)
 		{
 			m_force = force;
	 		m_direction = direction;
 		}
 		public int getForce() { return m_force; }
 		public DIRECTION getDirection() { return m_direction; }
 	}
  		  
 	public PowerSignal getHumanPower() {
 		PowerSignal signal = new PowerSignal(0, DIRECTION.NONE);
  		  
  		if (_message != "") {
			switch( _message)
	 		{
 				case "right": signal = new PowerSignal(1, DIRECTION.RIGHT); break;
 				case "left": signal = new PowerSignal(-1, DIRECTION.LEFT); break;
	 			case "up": signal = new PowerSignal(1, DIRECTION.UP); break;
 				case "down": signal = new PowerSignal(-1, DIRECTION.DOWN); break;
 				default: break;
 			}
 			_message = "";
  		}
	 	return signal;
  	}

	void OnGUI(){
		
		if (_debug) {
			// Make a background box
			GUI.Box (new Rect (10, 10, 400, 400), "Actions");

			// Check if the websocket has an opened connection
			if (webSocket == null || !webSocket.IsOpened ()) {
				// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
				if (GUI.Button (new Rect (30, 40, 320, 80), "Connect")) {

					// first step : we need to create the WebSocket
					//      You can use a websocketURL (ws://XXXXXXX or wss:XXXXXXXX)
					//		You need to give a MonoBehaviour object who will receive events
					//		This object must extend WebSocketUnityDelegate interface
					//      For example : TestWebSocketController inherits of WebSocketUnityDelegate
					//		You can create your own websocket server
					//		for example : https://github.com/willryan/em-websocket
					// Warning : some libraries (for example on ios) don't support reconnection
					//         so you need to create a new websocketunity before each connect (destroy your object when receive a disconnect or an error)
					//webSocket = new WebSocketUnity("ws://echo.websocket.org", this); // <= public server

					connectWebsocket (webSocket);

				}
			} else {
				// Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
				if (GUI.Button (new Rect (30, 40, 320, 80), "Disconnect")) {

					// Third Step : we need to close the connection when we finish
					webSocket.Close ();
				}

				if (GUI.Button (new Rect (30, 130, 320, 80), "Send message")) {

					// Fourth Step : we can send message
					//    In this sample, we are waiting a "echo" message from the server
					//    when we will receive this message, we will be able to change the display
					webSocket.Send ("Hello World");
					sendingMessage = true;
				}
				if (GUI.Button (new Rect (30, 220, 320, 80), "Send data")) {

					// Fifth Step : we can send Data
					//    In this sample, we are waiting a "echo" message from the server
					//    when we will receive this message, we will be able to change the display
					// You need a server which manages bytes message

					int test1 = 42;
					int test2 = 33;
					byte[] data = new byte[8];

					byte[] testB1 = System.BitConverter.GetBytes (test1);
					byte[] testB2 = System.BitConverter.GetBytes (test2);

					testB1.CopyTo (data, 0);
					testB2.CopyTo (data, 4);

					webSocket.Send (data);
					sendingMessage = true;
				}

				if (sendingMessage) {
					GUI.Label (new Rect (30, 350, 400, 60), "Waiting answer from server...");
				} else if (receivedMessage) {
					GUI.Label (new Rect (30, 350, 400, 60), "Server sent an echo answer !");
					DisplayMessage (_message);
				}

			}
		}

	}

	private void connectWebsocket(WebSocketUnity websocket)
	{
		webSocket = new WebSocketUnity ("ws://" + address, this); // <= local server
				// Second Step : we open the connection
		webSocket.Open ();
		Debug.Log ("Connected to " + address);
	}

	private void DisplayMessage (string message)
	{
		GUI.Label(new Rect(100, 400, 400, 60), message);
	}

	#region WebSocketUnityDelegate implementation

	// These callbacks come from WebSocketUnityDelegate
	// You will need them to manage websocket events

	// This event happens when the websocket is opened
	public void OnWebSocketUnityOpen (string sender)
	{
		Debug.Log("WebSocket connected, "+sender);
		sendingMessage = false;
		receivedMessage = false;
	}

	// This event happens when the websocket is closed
	public void OnWebSocketUnityClose (string reason)
	{
		Debug.Log("WebSocket Close : "+reason);
		sendingMessage = false;
		receivedMessage = false;
	}

	// This event happens when the websocket received a message
	public void OnWebSocketUnityReceiveMessage (string message)
	{
		Debug.Log("Received : " + message);
	
		//if(!sendingMessage)
		//	return;

		sendingMessage = false;
		receivedMessage = true;
		_message = message;
	}

	// This event happens when the websocket received data (on mobile : ios and android)
	// you need to decode it and call after the same callback than PC
	public void OnWebSocketUnityReceiveDataOnMobile(string base64EncodedData)
	{
		// it's a limitation when we communicate between plugin and C# scripts, we need to use string
		byte[] decodedData = webSocket.decodeBase64String(base64EncodedData);
		OnWebSocketUnityReceiveData(decodedData);
	}

	// This event happens when the websocket did receive data
	public void OnWebSocketUnityReceiveData(byte[] data)
	{	
		int testInt1 = System.BitConverter.ToInt32(data,0);
		int testInt2 = System.BitConverter.ToInt32(data,4);;

		Debug.Log("Received data from server : " + testInt1 + ", " + testInt2);

		if(!sendingMessage)
			return;

		sendingMessage = false;
		receivedMessage = true;
	}

	// This event happens when you get an error@
	public void OnWebSocketUnityError (string error)
	{
		Debug.LogError("WebSocket Error : "+ error);
		sendingMessage = false;
		receivedMessage = false;
	}

	#endregion

}
