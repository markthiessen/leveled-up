 if(_connectedToLeveledUpServer==null)
 	var _connectedToLeveledUpServer=false;
 if(!_connectedToLeveledUpServer){
	 (function(){
	 	window.scroll(0, window.localStorage.getItem('lastKnowScrollLocation'));
		var ws = new WebSocket("ws://localhost:9797"); 
		ws.onopen = function () { 
		    console.log('Connected to leveledUp change notification server....');
		    _connectedToLeveledUpServer=true;
		};				 
		ws.onmessage = function (evt) { 
			window.localStorage.setItem('lastKnowScrollLocation', window.pageYOffset);
			window.location.reload(); 
		};
	})();
}
