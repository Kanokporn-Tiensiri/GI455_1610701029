using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocket
{
    public class WebSocketConnectionW5 : MonoBehaviour
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
            //public string Username;

            public SocketEvent(string eventName, string data)
            {
                this.eventName = eventName;
                this.data = data;
                //this.Username = username;
            }
        }

        public GameObject rootConnection;
        public GameObject rootLogin;
        public GameObject rootRegister;
        public GameObject rootLoginFail;
        public GameObject rootLoginInputFail;
        public GameObject rootRegisterFail;
        public GameObject rootRegisNotMatch;
        public GameObject rootRegisInputFail;
        public GameObject rootLobby;
        public GameObject rootCreateRoom;
        public GameObject rootJoinRoom;
        public GameObject rootCreateFail;
        public GameObject rootJoinFail;
        public GameObject rootMessenger;

        public InputField InputMes;
        public InputField InputUser;
        public InputField InputID;
        public InputField InputPassword;
        public InputField InputIDRegis;
        public InputField InputName;
        public InputField InputPasswordRegis;
        public InputField InputRePassword;
        public InputField InputCreate;
        public InputField InputJoin;

        //public Transform BGBox;
        //public GameObject ChatBox;

        //private string msg;
        //private string mes;
        private string join;
        public string Name;

        MessageData msgJson;

        public Text RoomName;
        public Text sendText;
        public Text receiveText;

        private WebSocket ws;

        private string tempMessageString;

        public delegate void DelegateHandle(SocketEvent result);
        public DelegateHandle OnLoginRoom;
        public DelegateHandle OnRegisterRoom;
        public DelegateHandle OnCreateRoom;
        public DelegateHandle OnJoinRoom;
        public DelegateHandle OnLeaveRoom;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootLogin.SetActive(false);
            rootLoginFail.SetActive(false);
            rootRegister.SetActive(false);
            rootRegisterFail.SetActive(false);
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(false);
            rootCreateFail.SetActive(false);
            rootJoinRoom.SetActive(false);
            rootJoinFail.SetActive(false);
            rootMessenger.SetActive(false);
            rootRegisNotMatch.SetActive(false);
            rootRegisInputFail.SetActive(false);
            rootLoginInputFail.SetActive(false);

            OnCreateRoom += CreateRoomHandler;
            OnJoinRoom += JoinRoomHandler;
            OnLoginRoom += LoginHandler;
            OnRegisterRoom += RegisterHandler;
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

            rootConnection.SetActive(false);
            rootLogin.SetActive(true);
        }

        bool backLoginFail = false;
        bool backLoginInputFail = false;
        bool backRegisterFail = false;
        bool backRegisterInputFail = false;
        bool backRegisterNotMatch = false;
        bool backCreateFail = false;
        bool backJoinFail = false;

        public void BackPage()
        {
            if (backLoginFail == true)
            {
                rootLogin.SetActive(true);
                rootLoginFail.SetActive(false);
                backLoginFail = false;
            }
            if (backLoginInputFail == true)
            {
                rootLogin.SetActive(true);
                rootLoginInputFail.SetActive(false);
                backLoginInputFail = false;
            }
            if (backRegisterFail == true)
            {
                rootRegister.SetActive(true);
                rootRegisterFail.SetActive(false);
                backRegisterFail = false;
            }
            if (backRegisterInputFail == true)
            {
                rootRegister.SetActive(true);
                rootRegisInputFail.SetActive(false);
                backRegisterInputFail = false;
            }
            if (backRegisterNotMatch == true)
            {
                rootRegister.SetActive(true);
                rootRegisNotMatch.SetActive(false);
                backRegisterNotMatch = false;
            }
            if (backCreateFail == true)
            {
                rootCreateRoom.SetActive(true);
                rootCreateFail.SetActive(false);
                backCreateFail = false;
            }
            if (backJoinFail == true)
            {
                rootJoinRoom.SetActive(true);
                rootJoinFail.SetActive(false);
                backJoinFail = false;
            }
        }

        public void LoginHandler(SocketEvent receive)
        {
            string[] receiveArr = receive.data.Split('#');
            if (receive.data == "fail")
            {
                backLoginFail = true;
                rootLoginFail.SetActive(true);
            }
            else
            {
                Name = receiveArr[1];
                rootLogin.SetActive(false);
                rootLobby.SetActive(true);
            }
        }

        public void Login(string login)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                if (InputID.text == "" || InputPassword.text == "")
                {
                    backLoginInputFail = true;
                    rootLoginInputFail.SetActive(true);
                }
                else
                {
                    login = InputID.text + "#" + InputPassword.text;

                    SocketEvent socketEvent = new SocketEvent("Login", login);

                    string toJsonStr = JsonUtility.ToJson(socketEvent);

                    ws.Send(toJsonStr);
                }
            }
        }

        public void RegisterHandler(SocketEvent receive)
        {
            if (receive.data == "fail")
            {
                backRegisterFail = true;
                rootRegisterFail.SetActive(true);
            }
            else
            {
                rootRegister.SetActive(false);
                rootLogin.SetActive(true);
            }
        }

        public void Register(string register)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                if (InputIDRegis.text == "" || InputPasswordRegis.text == "" || InputName.text == "" || InputRePassword.text == "")
                {
                    backRegisterInputFail = true;
                    rootRegisInputFail.SetActive(true);
                }
                else
                {
                    if (InputRePassword.text == InputPasswordRegis.text)
                    {
                        register = InputIDRegis.text + "#" + InputPasswordRegis.text + "#" + InputName.text;

                        SocketEvent socketEvent = new SocketEvent("Register", register);

                        string toJsonStr = JsonUtility.ToJson(socketEvent);

                        ws.Send(toJsonStr);
                    }
                    else
                    {
                        backRegisterNotMatch = true;
                        rootRegisNotMatch.SetActive(true);
                    }
                }
            }
        }

        public void RegisterNext()
        {
            rootRegister.SetActive(true);
        }

        public void CreateRoom()
        {
            rootLobby.SetActive(false);
            rootCreateRoom.SetActive(true);
        }

        private void CreateRoomHandler(SocketEvent receive)
        {
            if (receive.data == "fail")
            {
                backCreateFail = true;
                rootCreateFail.SetActive(true);
            }
            else
            {
                rootMessenger.SetActive(true);
            }
        }

        private void JoinRoomHandler(SocketEvent receive)
        {
            if (receive.data == "success")
            {
                rootMessenger.SetActive(true);
            }
            else
            {
                backJoinFail = true;
                rootJoinFail.SetActive(true);
            }
        }

        public void CreateRoomNext(string roomName)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("CreateRoom", "Room : " + InputCreate.text);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }

            RoomName.text = "Room : " + InputCreate.text;
        }

        public void JoinRoom()
        {
            rootLobby.SetActive(false);
            rootJoinRoom.SetActive(true);
        }

        public void JoinRoomNext(string roomName)
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("JoinRoom", "Room : " + InputJoin.text);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }

            join = InputJoin.text;
            RoomName.text = "Room : " + InputJoin.text;
        }

        public void LeaveRoom()
        {
            if (ws.ReadyState == WebSocketState.Open)
            {
                SocketEvent socketEvent = new SocketEvent("LeaveRoom", join);

                string toJsonStr = JsonUtility.ToJson(socketEvent);

                ws.Send(toJsonStr);
            }
            receiveText.text = "";
            sendText.text = "";
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

        //public MessageData newMessageData = new MessageData();

        public void SendMessage()
        {
            if (InputMes.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData newMessageData = new MessageData();
            newMessageData.Username = Name;
            newMessageData.Message = InputMes.text;

            string toJsonStr = JsonUtility.ToJson(newMessageData);

            SocketEvent socketEvent = new SocketEvent("SendMessage", toJsonStr);

            string _toJsonStr = JsonUtility.ToJson(socketEvent);

            ws.Send(_toJsonStr); //ws.Send(InputMes.text);

            InputMes.text = "";
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void UpdateNotifyMessage()
        {
            //Debug.Log(tempMessageString);

            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                SocketEvent receiveMessageData = JsonUtility.FromJson<SocketEvent>(tempMessageString);
                

                if (receiveMessageData.eventName == "Login")
                {
                    if (OnLoginRoom != null)
                        OnLoginRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "Register")
                {
                    if (OnRegisterRoom != null)
                        OnRegisterRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "CreateRoom")
                {
                    if (OnCreateRoom != null)
                        OnCreateRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "JoinRoom")
                {
                    if (OnJoinRoom != null)
                        OnJoinRoom(receiveMessageData);
                }
                else if (receiveMessageData.eventName == "SendMessage")
                {
                    MessageData _receiveMessageData = JsonUtility.FromJson<MessageData>(receiveMessageData.data);

                    if (_receiveMessageData.Username == Name)
                    {
                        //JNbean: Hello
                        Debug.Log(_receiveMessageData.Username);
                        receiveText.text += "\n";
                        sendText.text += _receiveMessageData.Username + ": " + _receiveMessageData.Message + "\n";
                    }
                    else
                    {
                        sendText.text += "\n";
                        receiveText.text += _receiveMessageData.Username + ": " + _receiveMessageData.Message + "\n";
                    }
                    //receiveText.text += tempMessageString + "\n";
                    //tempMessageString = "";
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
            tempMessageString = messageEventArgs.Data;
            Debug.Log(messageEventArgs.Data);
            //msgJson = JsonUtility.FromJson<MessageData>(messageEventArgs.Data);
            //msg = msgJson.Message;

            ////Receive();

            //if (msgJson.Username == InputUser.text)
            //{
            //    tempMessageString = messageEventArgs.Data;
            //}
        }

        //private void Receive()
        //{
        //    MessageData _receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);
            //var newMSG = Instantiate(ChatBox, BGBox);
            //newMSG.GetComponent<Text>().text = mes + "\n"; //Chat box move up
            //newMSG.transform.SetSiblingIndex(0);

            //if (InputUser.text + " " == msgJson.Username)
            //{
            //    newMSG.GetComponent<Text>().alignment = TextAnchor.LowerRight;
            //}
            //if (_receiveMessageData.Username == InputMes.text)
            //{
            //    //JNbean: Hello
            //    sendText.text += _receiveMessageData.Username + ": " + _receiveMessageData.Message + "\n";
            //}
            //else
            //{
            //    receiveText.text += _receiveMessageData.Username + ": " + _receiveMessageData.Message + "\n";
            //}
            //tempMessageString = "";
        //}
    }
}


