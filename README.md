leveled-up
==========

What is this?
A simple Windows app that hosts a WebSocket server and can monitor a directory for changes.
When things are a-changin', clients are notified.

Why?
Because during web development, it's nice to have your browser auto-refresh for you.

How to use?
Start up the LeveledUp.exe, set the directory you want to monitor (your project folder!), modify the file type filter if you like, and hit 'Start'.

And then in your HTML file somewhere.. you could just embed a tiny little script in the page you are working on to connect to the socket and auto-reload your page:

<script type="text/javascript">
    (function(){
    	var ws = new WebSocket("ws://localhost:9797"); 
    	ws.onopen = function () {
		    console.log('Connected to leveledUp change notification server....')
		};				 
		ws.onmessage = function (evt) { window.location.reload(); };
    })();			
</script>

You probably don't want that to make it's way into your release builds. For now, it's just an experiment. I'd like to find a nice way to embed this automatically in some scenarios..

In an ASP.NET MVC project using razor, for example, you can wrap that script in this:

@if (HttpContext.Current.IsDebuggingEnabled)
    {
       ...
    }

It's a start..
