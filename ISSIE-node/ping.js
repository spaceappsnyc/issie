var ws = require("nodejs-websocket")
var fs = require('fs');
var child_process = require('child_process');

require('events').EventEmitter.prototype._maxListeners = 20;

// ping mode flag for debugging
var pingOn = true;
//var pingOn = false;

// Setup a server that can receive text and broadcast out.
var server = ws.createServer(function (conn) {
	console.log("New connection")
	conn.on("text", function (str) {
		console.log("Received "+str)
		conn.sendText(str.toUpperCase()+"!!!")
	})
	conn.on("close", function (code, reason) {
		console.log("Connection closed")
	})
}).listen(8001)

function broadcast(str) {
	console.log("Sending " + str);
	server.connections.forEach(function (connection) {
		connection.sendText(str)
		console.log("Sent: " + str);
	})
}

// some code to test websockets via a ping
var countOfPings = 0;
function myPing(){
	var message = countOfPings.toString();
	if (countOfPings % 2) {
		message = "right";
	}
	else {
		message = "left";
	}

	broadcast(message);
	console.log(message);
	countOfPings++;
}
if (pingOn) {
	setInterval(myPing,1000);
}

