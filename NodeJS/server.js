var websocket = require('ws');

var websocketServer = new websocket.Server({port:31124}, ()=>{
    console.log("JNBean Server is running");
});

var wsList = [];
var roomList = [];

/*
{
    roomName: ""
    wsList: []
}
*/

websocketServer.on("connection", (ws, rq)=>{
    
     //LobbyZone
        console.log('client connected');

        ws.on("message", (data)=>{
            console.log("send from client : "+data);

            var toJsonObj = { 
                roomName:"",
                data:""
            }

            var toJsonObj = JSON.parse(data); //Change string to data
            var user = toJsonObj.Username;
            console.log(user);

            if (toJsonObj.eventName == "CreateRoom") //CreateRoom
            {
                var isFoundRoom = false;

                for (var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }

                if (isFoundRoom == true)
                {
                    //callback to client create room fail
                    var callbackMsg = {
                        eventName:"CreateRoom",
                        data:"fail",
                        Username:user
                    }
                    console.log("eventName " + callbackMsg.eventName + " result " + callbackMsg.data + " Username " + callbackMsg.Username);
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================

                    console.log("client create room fail.");
                    //ws.send("CreateRoom Fail")
                }
                else
                {
                    //callback to client create room success

                    var newRoom ={
                        roomName: toJsonObj.data,
                        wsList: []
                    }

                    newRoom.wsList.push(ws);
                    roomList.push(newRoom);

                    var callbackMsg = {
                        eventName:"CreateRoom",
                        data:"success",
                        Username:user
                    }
                    console.log("eventName " + callbackMsg.eventName + " result " + callbackMsg.data + " Username " + callbackMsg.Username);
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=====================================================
                    console.log("client create room success.");
                    //ws.send("CreateRoom Success")
                }

                console.log("client request CreateRoom ["+toJsonObj.data+"]");

                /*for (var i = 0; i < roomList.length; i++)
                {
                    console.log(roomList[i]);
                }*/
            }
            else if (toJsonObj.eventName == "JoinRoom") //JoinRoom
            {
                var isFoundRoom = false;

                for (var i = 0; i < roomList.length; i++)
                {
                    if(roomList[i].roomName == toJsonObj.data)
                    {
                        isFoundRoom = true;
                        break;
                    }
                }

                if (isFoundRoom == true)
                {
                    var callbackMsg = {
                        eventName:"JoinRoom",
                        data:"success",
                        Username:user
                    }
                    console.log("eventName " + callbackMsg.eventName + " result " + callbackMsg.data + " Username " + callbackMsg.Username);
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //callback to client join room success
                    console.log("client join room success.");
                    console.log("client request JoinRoom ["+toJsonObj.data+"]");
                }
                else
                {
                    //callback to client join room fail
                    /*var newRoom ={
                        roomName: toJsonObj.data,
                        wsList: []
                    }

                    newRoom.wsList.push(ws);
                    roomList.push(newRoom);*/

                    var callbackMsg = {
                        eventName:"JoinRoom",
                        data:"fail",
                        Username:user
                    }
                    console.log("eventName " + callbackMsg.eventName + " result " + callbackMsg.data + " Username " + callbackMsg.Username);
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================
                    console.log("client join room fail.");
                }

                //console.log("client request JoinRoom ["+toJsonObj.data+"]");
            }
            else if (toJsonObj.eventName == "LeaveRoom") //LeaveRoom
            {
                var isLeaveSuccess = false;//Set false to default.
                for(var i = 0; i < roomList.length; i++)//Loop in roomList
                {
                    for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
                    {
                        if(ws == roomList[i].wsList[j])//If founded client.
                        {
                            roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                            if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                            {
                                roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                            }

                            isLeaveSuccess = true;
                            break;
                        }
                    }
                }

                if (isLeaveSuccess)
                {
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"success"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================
    
                    console.log("leave room success");
                }
                else
                {
                    var callbackMsg = {
                        eventName:"LeaveRoom",
                        data:"fail"
                    }
                    var toJsonStr = JSON.stringify(callbackMsg);
                    ws.send(toJsonStr);
                    //=======================================================
    
                    console.log("leave room fail");
                }
            }

            console.log("client request LeaveRoom [" + toJsonObj.roomName + "]")
        });

    /*wsList.push(ws);

    ws.on("message", (data)=>{
        console.log("send from client : "+data);
        Boardcast(data);
    });*/

    ws.on("close", ()=>{
        //wsList = ArrayRemove(wsList, ws);
        console.log("client disconnected.");

        //============ Find client in room for remove client out of room ================
        for(var i = 0; i < roomList.length; i++)//Loop in roomList
        {
            for(var j = 0; j < roomList[i].wsList.length; j++)//Loop in wsList in roomList
            {
                if(ws == roomList[i].wsList[j])//If founded client.
                {
                    roomList[i].wsList.splice(j, 1);//Remove at index one time. When found client.

                    if(roomList[i].wsList.length <= 0)//If no one left in room remove this room now.
                    {
                        roomList.splice(i, 1);//Remove at index one time. When room is no one left.
                    }
                    break;
                }
            }
        }
    });
});

function ArrayRemove(arr, value)
{
    return arr.filter((element)=>{
        return element != value;
    })
}

function Boardcast(data)
{
    for(var i = 0; i < wsList.length; i++)
    {
        wsList[i].send(data);
    }
}