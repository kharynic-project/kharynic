# WebGL + WebAssembly + WebApp
We are rebooting the project on modern web platform. 
Entering game website is enough to play it. It will load from the server in the background. 
At some point HTML GUI will be smoothly replaced by in-game one.
You will be able to start playing at some point, while it will still be downloading rest of the assets.
Once it's done, it will notify you that now it's accessible offline in the cache of your browser.
You can bookmark it, add desktop shortcut to it or even download it to specific place on your hard drive.

# Unity shell layer
Unity projects contains only shell - empty world with empty object and single component that all resources except code assemblies from external sources. 
After main menu is loaded, hosting webpage is triggered with JavaScript to hide splash image / intro / web-based main menu and show Unity WebGL player instead. 
That web shell is also responsible for managing browser cache and for web application manifest.

# Restrictions
- System.Reflection.Emit is unavailable
- Serialization: all used types and methods have to be referenced from at least dummy code
- System.Threading is unavailable
