 (function(){

 	console.log('Leveled UP: Changes detected.');

	//window.scroll(0, window.localStorage.getItem('lastKnowScrollLocation'));

     //window.localStorage.setItem('lastKnowScrollLocation', window.pageYOffset);

     if (scriptOptions.reloadCss) {
         var links = document.getElementsByTagName("link");
         for (var i = 0; i < links.length; i++) {
             var link = links[i];
             if (link.rel === "stylesheet") {
                 var queryString = '?reload=' + new Date().getTime();
                 link.href = link.href.replace(/\?.*|$/, queryString);
             }
         }
     }
     else
         window.location.reload();
})();
