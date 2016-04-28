var ws = require("nodejs-websocket")
var fs = require('fs');
var child_process = require('child_process');

require('events').EventEmitter.prototype._maxListeners = 20;

// ping mode flag for debugging
//var pingOn = true;
var pingOn = false;

// Johnny-Five for RPi
var raspi = require('raspi-io');
var five = require('johnny-five');
var board = new five.Board({io: new raspi()});

// Setup keyboard listener for MakeyMakey
var stdin = process.openStdin();
process.stdin.setRawMode(true);

stdin.on('keypress', function (chunk, key) {

  if (chunk === "w"){msg = "up";}
else if (chunk === "a"){msg = "left";}
else if (chunk === "s"){msg = "down";}
else if (chunk === "d"){msg = "right";}
else{msg = "right";}
  broadcast(msg);
//  process.stdout.write('Get Chunk: ' + chunk + '\n');
  if (key && key.ctrl && key.name == 'c') process.exit();
});

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
	broadcast(message);
	console.log(message);
	countOfPings++;
}
if (pingOn) {
	setInterval(myPing,1000);
}

// Fire up the board and look for input
board.on('ready', function() {
  console.log('board is ready');

  // Create a new `motion` hardware instance.
  var motion = new five.Motion('P1-7'); //a PIR is wired on pin 7 (GPIO 4)

  // 'calibrated' occurs once at the beginning of a session
  motion.on('calibrated', function() {
    console.log('calibrated');
  });

  // Motion detected
  motion.on('motionstart', function() {
    console.log('motionstart');
    broadcast('motionstart');
  });

  // 'motionend' events
  motion.on('motionend', function() {
    console.log('motionend');
    broadcast('motionend');
  });
});
