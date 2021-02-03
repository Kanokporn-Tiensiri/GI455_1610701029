using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace ProgramChat
{
    public class WebsocketConnection : MonoBehaviour
    {
        private WebSocket websocket;

        public InputField InputMes;
        public InputField InputUser;
        //public Text Mes;
        //public Text User;

        public Transform BGBox;
        public GameObject ChatBox;
        //private Object messageEventArgs;
        private string[] msg;
        private string mes;
        //string onMSG;

        void Start()
        {
            websocket = new WebSocket("ws://127.0.0.1:31124/");

            websocket.OnMessage += OnMessage;

            websocket.Connect();

            //websocket.Send("I'm coming here.");
        }

        //void Update()
        //{
            //if (Input.GetKeyDown(KeyCode.Return)) //Press Enter to send
            //{
            //    websocket.Send("Rendom number : " + Random.Range(0, 999999));
            //    if (websocket.ReadyState == WebSocketState.Open)
            //    {
            //        newMSG = InputMes.text; //Show text output
            //        websocket.Send(InputUser.text + " : " + InputMes.text);
            //    }
            //}
        //}

        public void GetInputOnClick() //Click mouse on button to send
        {
            if (websocket.ReadyState == WebSocketState.Open)
            {
                //Mes.text = InputMes.text;
                websocket.Send(InputUser.text + " : " + InputMes.text);
            }
        }

        private void OnDestroy()
        {
            if(websocket != null)
            {
                websocket.Close();
            }
        }

        public void OnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            mes = messageEventArgs.Data;
            msg = messageEventArgs.Data.Split(':');
            Receive();
            Debug.Log("Receive msg : " + messageEventArgs.Data);
        }

        private void Receive()
        {
            var newMSG = Instantiate(ChatBox, BGBox);
            newMSG.GetComponent<Text>().text = mes + "\n"; //Chat box move up
            newMSG.transform.SetSiblingIndex(0);

            if (InputUser.text + " " == msg[0])
            {
                newMSG.GetComponent<Text>().alignment = TextAnchor.LowerRight;
            }
        }
    }
}

