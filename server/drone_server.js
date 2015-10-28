// Includes
var osc = require('osc-min'),
  udp = require('dgram'),
  arDrone = require('ar-drone'),
  keypress = require('keypress'),
  http = require('http')
  tools = require('./tools');

// standard ip for the drone
var ipDrone = '192.168.1.1';

var receivePort = 12001;
var droneIndex = 0;


// standard ip for sending info about NavData
var ipTarget = '192.168.3.173';
var outport = 12345;


var battery = 1,
  altitude = 0,
  front_back = 0,
  left_right = 0,
  is_in_air = false,
  is_in_air_test = false;

process.argv.forEach(function(val, index) {
  if (index > 1) {
    console.log(index + ': ' + val);
  }
  if (index === 2) {
    ipDrone = val;
  }
  if (index === 3) {
    droneIndex = val;
  }
});

// Initializing the client
var client = arDrone.createClient({
  'ip': ipDrone
});

var udpSend = udp.createSocket('udp4');

client.disableEmergency();
client.ftrim();
client.config('general:navdata_demo', 'TRUE');
client.config('control:altitude_max', 8000);
client.config('control:control_vz_max', 2000);


// make `process.stdin` begin emitting 'keypress' events
keypress(process.stdin);

var mayday = 0;
// listen for the 'keypress' event
// keypress events in the terminal that will overwrite the osx commands
process.stdin.on('keypress', function(ch, key) {
  if (!key) {
    return;
  }
  if (key.name === 'space' && (!is_in_air_test)) {
    console.log('take off');
    client.stop();
    client.takeoff();
    is_in_air_test = true;

  } else if (key.name === 'space' && (is_in_air_test || is_in_air)) {
    client.stop();
    console.log('land');
    client.land();
    is_in_air_test = false;
    is_in_air = false;
  }
  if (is_in_air) {
    if (key.name === 'f') {
      console.log("flip");
      client.animate('flipBehind', 1000);
    }
    else if (key.name == "d") {
      stdin.pause();
      client.down(0.25);
      client.after(10, function() {
        client.stop();
        stdin.resume();
      });
    }
    else if (key.name == "u") {
      stdin.pause();
      client.up(0.25);
      client.after(10, function() {
        client.stop();
        stdin.resume();
      });
    }
    else if (key.name == "up") {
      stdin.pause();
      client.front(0.25);
      client.after(10, function() {
        client.stop();
        stdin.resume();
      });
    } else if (key.name == "down") {
      stdin.pause();
      client.back(0.25);
      client.after(10, function() {
        client.stop();
        stdin.resume();
      });
    } else if (key.name == "left") {
      stdin.pause();
      client.counterClockwise(0.35);
      client.after(10, function() {
        client.stop();
        stdin.resume();
      });
    } else if (key.name == "right") {
      stdin.pause();
      client.clockwise(0.35);
      client.after(10, function() {
        client.stop();
        stdin.resume();
      });
    }
  }


  if (key.ctrl && key.name === 'c') {
    //process.stdin.pause();
    console.log('Mayday!!!!!!!');
    if (++mayday >= 3) {
      process.exit();
    }

    client.stop();
    client.land();
    is_in_air_test = false;
    is_in_air = false;
  }
});

process.stdin.setRawMode(true);
process.stdin.resume();


// When getting message from Unity
var udp = udp.createSocket('udp4', function(msg) {
  var error, buf;
  try {
    var oscmessage = osc.fromBuffer(msg);



    if (oscmessage.address === '/musicStarted') {
      buf = osc.toBuffer({
        address: '/musicStarted',
        args: [{
          type: 'float',
          value: oscmessage.args[0].value
        }]
      });
      udpSend.send(buf, 0, buf.length, outport, ipTarget);
      return;
    }
    if (oscmessage.address === '/musicState') {
      buf = osc.toBuffer({
        address: '/musicState',
        args: [{
          type: 'float',
          value: oscmessage.args[0].value
        }]
      });
      udpSend.send(buf, 0, buf.length, outport, ipTarget);
      return;
    }

    if (oscmessage.address === '/startdrone') {
      client.stop();
      client.takeoff();
      is_in_air = true;
    }

    if (oscmessage.address === '/left') {
      client.left(oscmessage.args[0].value);
    }
    if (oscmessage.address === '/right') {
      client.right(oscmessage.args[0].value);
    }
    if (oscmessage.address === '/stop') {
      client.stop();
    }
    if (oscmessage.address === '/front') {
      client.front(oscmessage.args[0].value);
    }
    if (oscmessage.address === '/back') {
      client.back(oscmessage.args[0].value);
    }
    if (oscmessage.address === '/up') {
      client.up(oscmessage.args[0].value);
    }
    if (oscmessage.address === '/down') {
      client.down(oscmessage.args[0].value);
    }
    if (oscmessage.address === '/clockwise') {
      client.clockwise(oscmessage.args[0].value);
    }
    if (oscmessage.address === '/counterClockwise') {
      client.counterClockwise(oscmessage.args[0].value);
    }
    if (oscmessage.address === '/land') {
      client.stop();
      console.log('land');
      client.land();
      client.disableEmergency();
      is_in_air = false;
    }
    if (oscmessage.address === '/flip') {
      client.animate('flipLeft', 1000);
    }
    if (oscmessage.address === '/flipBehind') {
      client.animate('flipBehind', 1000);
    }
    if (oscmessage.address === '/frontFlip') {
      client.animate('flipAhead', 1000);
    }
    if (oscmessage.address === '/wave') {
      client.animate('wave', 500);
    }
    if (oscmessage.address === '/blinkRed') {
      client.animateLeds('blinkRed', 5, 2);
    }
    if (oscmessage.address === '/red') {
      client.animateLeds('red', 5, 2);
    }
    if (oscmessage.address === '/green') {
      client.animateLeds('green', 5, 2);
    }



    //return console.log(oscmessage.address);
  } catch (_error) {
    error = _error;
    return console.log('invalid OSC packet ' + _error);
  }
});

udp.bind(receivePort + parseInt(droneIndex));

client.on('navdata', function(navdata) {
  if (!navdata || !navdata.demo || !navdata.demo.velocity) {
    return;
  }

  // possitive forward, negative backwards
  front_back = navdata.demo.velocity.x;

  // possitive right, negative left
  left_right = navdata.demo.velocity.x;

  altitude = tools.clamp(Math.ceil(navdata.demo.altitudeMeters * 2.0) - 1, 1,
    8);

  battery = navdata.demo.batteryPercentage;

});


var send = function send() {

  //console.log('battery: ' + battery );
  var buf;
  buf = osc.toBuffer({
    address: '/d'+droneIndex+'_' + 'takeoff',
    args: [{
      type: 'float',
      value: is_in_air ? 1 : 0
    }]
  });

  udpSend.send(buf, 0, buf.length, outport, ipTarget);

  buf = osc.toBuffer({
    address: '/d'+droneIndex+'_' + 'takedown',
    args: [{
      type: 'float',
      value: is_in_air ? 0 : 1
    }]
  });

  udpSend.send(buf, 0, buf.length, outport, ipTarget);

  if (left_right < 0) {
    buf = osc.toBuffer({
      address: '/d'+droneIndex+'_' + 'velocityLeft',
      args: [{
        type: 'float',
        value: tools.clamp01((left_right * -1) / 1600)
      }]
    });
    udpSend.send(buf, 0, buf.length, outport, ipTarget);
  }
  if (left_right > 0) {
    buf = osc.toBuffer({
      address: '/d'+droneIndex+'_' + 'velocityRight',
      args: [{
        type: 'float',
        value: tools.clamp01(left_right / 1600)
      }]
    });
    udpSend.send(buf, 0, buf.length, outport, ipTarget);
  }
  buf = osc.toBuffer({
    address: '/d'+droneIndex+'_' + 'velocity',
    args: [{
      type: 'float',
      value: tools.clamp01((Math.abs(left_right) + Math.abs(front_back)) /
        2000)
    }]
  });
  udpSend.send(buf, 0, buf.length, outport, ipTarget);

  buf = osc.toBuffer({
    address: '/d'+droneIndex+'_' + 'altitude',
    args: [{
      type: 'float',
      value: altitude
    }]
  });
  udpSend.send(buf, 0, buf.length, outport, ipTarget);
  buf = osc.toBuffer({
    address: '/d'+droneIndex+'_' + 'battery',
    args: [{
      type: 'float',
      value: battery
    }]
  });
  console.log('Battery: ' + battery);
  udpSend.send(buf, 0, buf.length, outport, ipTarget);
  //client.up(0.25);
};

setInterval(send, 500);

console.log('init');


