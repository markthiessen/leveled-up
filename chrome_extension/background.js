var activeTabIds = [];
chrome.browserAction.onClicked.addListener(function(tab) {
	if(activeTabIds.indexOf(tab.id)<0){
		activeTabIds.push(tab.id);
  		chrome.tabs.executeScript(tab.id, {file: "reload.js"});

  		// Create a simple text notification:
		var notification = webkitNotifications.createNotification(
		  'icon.png',
		  'Leveled-UP+ Auto reload activated on this page!',
		  'Make sure the Leveled-UP+ desktop app is running!'
		);
		notification.show();
    }
});
chrome.tabs.onUpdated.addListener(
	function(tabId, changeInfo, tab) {
		if(changeInfo.status=='complete' && activeTabIds.indexOf(tabId)>=0) 
			chrome.tabs.executeScript(tabId, {file: "reload.js"});
});
