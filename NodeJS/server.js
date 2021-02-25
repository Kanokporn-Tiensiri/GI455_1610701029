var websocket = require('ws');
const sqlite = require('sqlite3').verbose();

var websocketServer = new websocket.Server({port:31124}, ()=>{
    console.log("JNBean Server is running");
});

var wsList = [];
var roomList = [];

/*var dataFromClient = {
    eventName:"AddMoney",
    data:"test4444#100"
}*/

var db = new sqlite.Database('./db/chatDB.db', sqlite.OPEN_CREATE | sqlite.OPEN_READWRITE, (err)=>{

    if(err) throw err;

    console.log('Connected to database.');

    websocketServer.on("connection", (ws, rq)=>{
    
        console.log('Client connected');

            ws.on("message", (data)=>{
                console.log("Send from client : "+data);

                /*var toJsonObj = { 
                    roomName:"",
                    data:""
                }*/
                var toJsonObj = {
                    eventName:"Login",
                    data:"test1111#111111#test1"
                }
                toJsonObj = JSON.parse(data); //Change string to data
                //var toJsonObj = JSON.parse(data);
                //var user = toJsonObj.Username;
                //console.log(user);
                var splitStr = toJsonObj.data.split('#');
                var userID = splitStr[0];
                var password = splitStr[1];
                var name = splitStr[2];
                //name = toJsonObj.Username;

                var sqlSelect = "SELECT * FROM UserData WHERE UserID='"+userID+"' AND Password='"+password+"'";
                var sqlInsert = "INSERT INTO UserData (UserID, Password, Name) VALUES ('"+userID+"', '"+password+"', '"+name+"')";
                //var sqlUpdate = "UPDATE UserData SET Money='200' WHERE UserID='"+userID+"'";

                if (toJsonObj.eventName == "Login") //Login
                {
                    db.all (sqlSelect, (err, rows)=>{
                        if (err)
                        {
                            console.log("[0]" + err);
                        }
                        else
                        {
                            if (rows.length > 0)
                            {
                                console.log("=====[1]=====");
                                console.log(rows);
                                console.log("=====[1]=====");

                                var callbackMsg = {
                                    eventName:"Login",
                                    data:"success#"+rows[0].Name
                                }

                                var toJsonStr = JSON.stringify(callbackMsg);
                                console.log("[2]" + toJsonStr);
                                console.log("Login success!");
                                ws.send(toJsonStr);
                            }
                            //console.log(rows);
                            //console.log(rows[0].Name);
                            else
                            {
                                var callbackMsg = {
                                    eventName:"Login",
                                    data:"fail"
                                }

                                var toJsonStr = JSON.stringify(callbackMsg);
                                console.log("[3]" + toJsonStr);
                                console.log("Login fail!");
                                ws.send(toJsonStr);
                            }
                        }
                    });
                }
                else if (toJsonObj.eventName == "Register") //Register
                {
                    db.all (sqlInsert, (err, rows)=>{
                        if (err)
                        {
                            var callbackMsg = {
                                eventName:"Register",
                                data:"fail"
                            }
                
                            var toJsonStr = JSON.stringify(callbackMsg);
                            console.log("[4]" + toJsonStr);
                            console.log("Register fail!");
                            ws.send(toJsonStr);
                        }
                        else
                        {
                            var callbackMsg = {
                                eventName:"Register",
                                data:"success"
                            }
                
                            var toJsonStr = JSON.stringify(callbackMsg);
                            console.log("[5]" + toJsonStr);
                            console.log("Register success!");
                            ws.send(toJsonStr);
                        }
                    });
                }
                //LobbyZone
                else if (toJsonObj.eventName == "CreateRoom") //CreateRoom
                {
                    var isFoundRoom = false;

                    for (var i = 0; i < roomList.length; i++)
                    {
                        if (roomList[i].roomName == toJsonObj.data)
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
                            //Username:name
                        }

                        console.log("eventName " + callbackMsg.eventName + " result " + callbackMsg.data);
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);

                        console.log("Client create room fail.");
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
                            //Username:name
                        }

                        console.log("eventName " + callbackMsg.eventName + " result " + callbackMsg.data);
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);

                        console.log("Client create room success.");
                        //ws.send("CreateRoom Success")
                    }

                    console.log("Client request CreateRoom ["+toJsonObj.data+"]");

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
                        if (roomList[i].roomName == toJsonObj.data)
                        {   
                            isFoundRoom = true;
                            roomList[i].wsList.push(ws);
                            break;
                        }
                    }

                    if (isFoundRoom == true)
                    {
                        var callbackMsg = {
                            eventName:"JoinRoom",
                            data:"success",
                            //Username:name
                        }
                        console.log("eventName " + callbackMsg.eventName + " result " + callbackMsg.data);
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
                        //callback to client join room success
                        console.log("Client join room success.");
                        console.log("Client request JoinRoom ["+toJsonObj.data+"]");
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
                            //Username:name
                        }

                        console.log("eventName " + callbackMsg.eventName + " result " + callbackMsg.data);
                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);

                        console.log("Client join room fail.");
                    }

                    //console.log("client request JoinRoom ["+toJsonObj.data+"]");
                }
                else if (toJsonObj.eventName == "LeaveRoom") //LeaveRoom
                {
                    var isLeaveSuccess = false; //Set false to default.
                    for (var i = 0; i < roomList.length; i++) //Loop in roomList
                    {
                        for (var j = 0; j < roomList[i].wsList.length; j++) //Loop in wsList in roomList
                        {
                            if (ws == roomList[i].wsList[j]) //If founded client.
                            {
                                roomList[i].wsList.splice(j, 1); //Remove at index one time. When found client.

                                if (roomList[i].wsList.length <= 0) //If no one left in room remove this room now.
                                {
                                    roomList.splice(i, 1); //Remove at index one time. When room is no one left.
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
    
                        console.log("Leave room success.");
                    }
                    else
                    {
                        var callbackMsg = {
                            eventName:"LeaveRoom",
                            data:"fail"
                        }

                        var toJsonStr = JSON.stringify(callbackMsg);
                        ws.send(toJsonStr);
    
                        console.log("Leave room fail.");
                    }
                }

                //console.log("Client request LeaveRoom [" + toJsonObj.roomName + "]")
                else if (toJsonObj.eventName == "SendMessage")
                {
                    //var msgJsonObj = JSON.parse(toJsonObj.data);
                    console.log("Send message from client : "+data);
                    Boardcast(ws, data);
                }
            });
    
        //wsList.push(ws);

        ws.on("close", ()=>{
            wsList = ArrayRemove(wsList, ws);
            console.log("Client disconnected.");

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
});

function ArrayRemove(arr, value)
{
    return arr.filter((element)=>{
        return element != value;
    })
}

function Boardcast(ws, message)
{
    var selectRoomIndex = -1;

    for (var i = 0; i < roomList.length; i++)
    {
        for (var j = 0; j < roomList[i].wsList.length; j++)
        {
            if (ws == roomList[i].wsList[j])
            {
                selectRoomIndex = i;
                console.log(selectRoomIndex);
                break;
            }
        }
    }

    for (var i = 0; i < roomList[selectRoomIndex].wsList.length; i++)
    {
        /*var callbackMsg = {
            eventName:"SendMessage",
            data:message
        }*/

        roomList[selectRoomIndex].wsList[i].send(message);
    }
}