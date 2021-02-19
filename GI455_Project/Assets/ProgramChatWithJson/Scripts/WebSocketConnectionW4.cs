using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocket
{
    public class WebSocketConnectionW4 : MonoBehaviour
    {
        public class MessageData
        {
            public string Username;
            public string Message;
        }

        public struct SocketEvent
        {
            public string eventName;
            public string data;
            public string Username;

            public SocketEvent(string eventName, string data, string username)
            {
                this.eventName = eventName;
                this.data = data;
                this.Username = username;
            }
        }

        public GameObject rootConnection;
        public GameObject rootLobby;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject rootMessenger;
        public GameObject rootCreateFail;
        public GameObject rootJoinFail;

        public InputField InputMes;
        public InputField InputUser;
        public InputField InputCreate;
        public InputField InputJoin;
        public InputField IP;
        public InputField Port;

        public Transform BGBox;
        public GameObject ChatBox;

        private string msg;
        private string mes;
        private string join;

        MessageData msgJson;

        public Text RoomName;

        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootCreateFail.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootJoinFail.SetActive(false);
            rootMessenger.SetActive(false);

            OnCreateRoom += CreateRoomHandler;
            OnJoinRoom += JoinRoomHandler;
            //OnLeaveRoom += Test;
        }

        private void Update()
        {
            UpdateNotifyMessage();

        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:31124/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            if (IP.text + ":" + Port.text == "127.0.0.1:31124")
            {
                rootConnection.SetActive(false);
                rootLobby.SetActive(true);
            }
            else
            {
                rootConnection.SetActive(true);
                rootLobby.SetActive(false);
            }
        }

        public void CreateRoom(string roomName)
        {
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(true);
        }

        private void CreateRoomHandler(SocketEvent receive)
        {
            Debug.Log(receive.data);

            if (receive.data == "fail")
            {
                rootCreateRoom.SetActive(false);
                rootCreateFail.SetActive(true);
                rootJoinFail.SetActive(false);
                rootMessenger.SetActive(false);
            }
            else
            {
                rootCreateRoom.SetActive(false);
                rootMessenger.SetActive(true);
                rootCreateFail.SetActive(false);
                rootJoinFail.SetActive(false);
            }
        }
        private void JoinRoomHandler(SocketEvent receive)
        {
            Debug.Log(receive.data);

            if (receive.data == "success")
            {
                rootJoinRoom.SetActive(false);
                rootMessenger.SetActive(true);
                rootCreateFail.SetActive(false);
                rootJoinFail.SetActive(false);
            }
            else
            {
                rootJoinRoom.SetActive(false);
                rootJoinFail.SetActive(true);
                rootCreateFail.SetActive(false);
                rootMessenger.SetActive(false);
            }
        }
        public void CreateRoomNext()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {

                SocketEvent socketEvent = new SocketEvent("CreateRoom", "Room : " + InputCreate.text, InputUser.text);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }

            //rootCreateRoom.SetActive(false);
            //rootMessenger.SetActive(true);

            RoomName.text = "Room : " + InputCreate.text;
        }

        public void JoinRoom(string roomName)
        {
            rootLobby.SetActive(false);
            rootJoinRoom.SetActive(true);
        }

        public void JoinRoomNext()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("JoinRoom", "Room : " + InputJoin.text, InputUser.text);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }

            //rootJoinRoom.SetActive(false);
            //rootMessenger.SetActive(true);

            join = InputJoin.text;
            //JoinRoom("Room : " + InputJoin.text);
            RoomName.text = "Room : " + InputJoin.text;
        }

        public void LeaveRoom()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("LeaveRoom", join, InputUser.text);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }

            rootMessenger.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootLobby.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }

        public void SendMessage()
        {
            if (InputMes.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData newMessageData = new MessageData();
            newMessageData.Username = InputUser.text;
            newMessageData.Message = InputMes.text;

            string toJsonStr = JsonUtility.ToJson(newMessageData);

            ws.Send(toJsonStr); //ws.Send(inputText.text);

            InputMes.text = "";
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void UpdateNotifyMessage()
        {
            Debug.Log(tempMessageString);

            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);

                if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "LeaveRoom")
                {
                    if (OnLeaveRoom != null)
                        OnLeaveRoom(receiveMessageData);
                }

                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            Debug.Log(messageEventArgs.Data);
            msgJson = JsonUtility.FromJson<MessageData>(messageEventArgs.Data);
            msg = msgJson.Message;

            //Receive();

            if (msgJson.Username == InputUser.text)
            {
                tempMessageString = messageEventArgs.Data;
            }
        }

        private void Receive()
        {
            var newMSG = Instantiate(ChatBox, BGBox);
            newMSG.GetComponent<Text>().text = mes + "\n"; //Chat box move up
            newMSG.transform.SetSiblingIndex(0);

            if (InputUser.text + " " == msgJson.Username)
            {
                newMSG.GetComponent<Text>().alignment = TextAnchor.LowerRight;
            }
        }
    }
}


