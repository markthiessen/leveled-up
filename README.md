leveled-up
==========

What is this?
Two things: 

1) A simple Windows app that hosts a WebSocket server and can monitor a directory for changes.
When things are a-changin', clients are notified.
2) A chrome extension that will connect to the file watcher and reload your page when files change.

Why?
Because during web development, it's nice to have your browser auto-refresh for you when you make little changes.. 
Clicking refresh every few seconds sucks.

How to use?

There's a current release in the build folder.  

1. Start up LeveledUp.exe
   a) Choose a folder to monitor (your project folder!)
   b) Adjust the file types if you need to. Follow the example format.
   c) Click Start!  The file system watcher is now running.

2. Use the chrome extension.
   a) If you haven't yet installed the extension, drag the .crx file onto a chrome window and click 'Add'.
   b) Navigate to the site you're working on in chrome.
   c) Click the Leveled-UP+ mushroom icon beside the omnibox.

3. Work!
   Now when you make changes to files in your project, your browser should automatically reload, saving you time
   and millions of dollars!
