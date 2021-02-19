using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using UnityEngine.UI;

namespace ChatWebSocketWithJson
{
    public class WebSocketConnectionW3 : MonoBehaviour
    {
        public class MessageData
        {
            public string Username;
            public string Message;
        }

        public GameObject rootConnection;
        public GameObject rootMessenger;

        public InputField InputUsername;
        public InputField inputText;
        public Text sendText;
        public Text receiveText;
        
        private WebSocket ws;

        private string tempMessageString;

        public void Start()
        {
            rootConnection.SetActive(true);
            rootMessenger.SetActive(false);
        }

        public void Connect()
        {
            string url = $"ws://127.0.0.1:31124/";

            ws = new WebSocket(url);

            ws.OnMessage += OnMessage;

            ws.Connect();

            rootConnection.SetActive(false);
            rootMessenger.SetActive(true);
        }

        public void Disconnect()
        {
            if (ws != null)
                ws.Close();
        }
        
        public void SendMessage()
        {
            if (inputText.text == "" || ws.ReadyState != WebSocketState.Open)
                return;

            MessageData newMessageData = new MessageData();
            newMessageData.Username = InputUsername.text;
            newMessageData.Message = inputText.text;

            string toJsonStr = JsonUtility.ToJson(newMessageData);

            ws.Send(toJsonStr);
            //ws.Send(inputText.text);
            inputText.text = "";
        }

        private void OnDestroy()
        {
            if (ws != null)
                ws.Close();
        }

        private void Update()
        {
            //if (tempMessageString != "")
            if (string.IsNullOrEmpty(tempMessageString) == false)
            {
                MessageData receiveMessageData = JsonUtility.FromJson<MessageData>(tempMessageString);

                if(receiveMessageData.Username == inputText.text)
                {
                    //JNbean: Hello
                    sendText.text += receiveMessageData.Username + ": " + receiveMessageData.Message + "\n";
                }
                else
                {
                    receiveText.text += receiveMessageData.Username + ": " + receiveMessageData.Message + "\n";
                }

                //receiveText.text += tempMessageString + "\n";
                tempMessageString = "";
            }
        }

        private void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            tempMessageString = messageEventArgs.Data;
        }
    }
}


