var activeTabIds = [];
var port;
var host_name = "com.markthiessen.leveledup";

chrome.browserAction.onClicked.addListener(function(tab) {
	if(activeTabIds.indexOf(tab.id)<0){
		activeTabIds.push(tab.id);
	}
  	//chrome.tabs.executeScript(tab.id, {file: "reload.js"});

  	// Create a simple text notification:
	// var notification = webkitNotifications.createNotification(
	//   'icon.png',
	//   'Leveled-UP+ Auto reload activated on this page!',
	//   'Make sure the Leveled-UP+ desktop app is running!'
	// );
	chrome.browserAction.setIcon({path:"active.png", tabId:tab.id});
	// notification.show();

	if(!port)
		connectToNative();
});
chrome.tabs.onUpdated.addListener(
	function(tabId, changeInfo, tab) {
		if(changeInfo.status=='complete' && activeTabIds.indexOf(tabId)>=0) 
			chrome.browserAction.setIcon({path:"active.png", tabId:tab.id});
	}
);


 connectToNative();

 function connectToNative() {
     port = chrome.runtime.connectNative(host_name);
     port.onMessage.addListener(onNativeMessage);
     port.onDisconnect.addListener(onDisconnected);
     //sendNativeMessage("Hello!");
     console.log('Listening for changes from leveled up server');
 }

 function sendNativeMessage(msg) {
     message = { "text": msg };
     console.log('Sending message to native app: ' + JSON.stringify(message));
     port.postMessage(message);
     console.log('Sent message to native app: ' + msg);
 }

 function onNativeMessage(evt) {
     console.log('Received msg');
     console.log(evt);

     var msg = evt.data;

     var isCss = (evt.data.indexOf('css') >= 0) ? 'true' : 'false';

     activeTabIds.forEach(function(tabId){
     	chrome.tabs.executeScript(tabId, {code: "var scriptOptions = {reloadCss:"+isCss+"};"}, function(){
	        chrome.tabs.executeScript(tabId, {file: "reload.js"}, function(){
	            //all injected
	        });
	    });
     });
 }

 function onDisconnected() {
     console.log(chrome.runtime.lastError);
     console.log('disconnected from native app.');
     activeTabIds.forEach(function(tabId){     	
	 	chrome.browserAction.setIcon({path:"icon.png", tabId:tabId});
     });
     activeTabIds = [];
     port = null;
 }
