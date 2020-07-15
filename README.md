# Tweaks for Visual Studio

[![Build status](https://ci.appveyor.com/api/projects/status/4pha1svkn0aqg3u4?svg=true)](https://ci.appveyor.com/project/madskristensen/tweakster)

A collection of minor fixes and tweaks for Visual Studio to reduce the paper cuts and make you a happier developer

<!--Download this extension from the [Marketplace](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.KnownMonikersExplorer)
or get the [CI build](https://www.vsixgallery.com/extension/0c8bd9fa-77d5-4563-ab57-9e01608c3d04/).-->

----------------------------------------------

## Features
The list of features are coming from the [Visual Studio Developer Community](https://developercommunity.visualstudio.com/topics/extension+candidate.html?page=1&pageSize=15&sort=votes&openOnly=false&closedOnly=false&topics=extension%20candidate) where users are posting feature suggestions and problem report tickets. 
It's from those tickets inspiration for this extension came.

* [Run Code Cleanup on format](#codecleanup)
* [Re-open closed tab](#reopen)
* [Auto save](#autosave)
* [Don't copy empty lines](#dontcopyemptylines)
* [Don't start debug on F10/F11](#nodebugonf10)
* [Restart command](#restart)
* [Delete *obj* and *bin* on Clean](#deleteoutputartifacts)
* [Format on line move up/down](#formatonlinemove)
* [Settings](#settings)

If you know of any suggestion tickets that would be well suited for this extension to implement, please [open an issue](https://github.com/madskristensen/Tweakster/issues) on it. Find inspiration by [browsing the Developer Community](https://developercommunity.visualstudio.com/spaces/8/visual-studio-ide.html?sort=votes&type=idea&openOnly=true) for more suggestions.

<h3 id="codecleanup">Run Code Cleanup on format (C# only)</h3>

Inspired by the suggestion [Cleanup code during formatting](https://developercommunity.visualstudio.com/idea/420291/cleanup-code-during-formatting.html)

Instead of running the *Code Cleanup* command manually, it now runs automatically when *Format Document* or **Ctrl+K,Ctrl+D** is invoked. 

<h3 id="reopen">Re-Open Closed File</h3>

Inspired by the suggestion [Reopen closed tab](https://developercommunity.visualstudio.com/content/idea/402931/reopen-closed-tab.html)

When you close a file by accident, you can now easily open it back up again. Go to **File -> Re-Open [file name]**

![Re-Open Closed File](art/re-open-closed-file.png)

<h3 id="autosave">Auto save</h3>

Inspired by the suggestion [Option to Auto Save the editor pages](https://developercommunity.visualstudio.com/idea/371187/option-to-auto-save-the-editor-pages.html).

Automatic saving of documents happen when the document loses focus. That could happen when you open a different document or click around in another tool window such as Solution Explorer. It will also save any changes to its containing project.

Projects are also automatically saved when files are added, removed or renamed. 

<h3 id="dontcopyemptylines">Don't copy empty lines</h3>

Inspired by the suggestion [Please stop clearing the clipboard when you hit ctrl+c and nothing is selected](https://developercommunity.visualstudio.com/idea/693790/please-stop-clearing-the-clipboard-when-you-hit-ct.html).

When the caret is placed on an empty line and you hit *Copy* or **Ctrl+C** then the empty lines isn't copied to the clipboard like it normally would.

<h3 id="nodebugonf10">Don't start debug on F10/F11</h3>

Inspired by the suggestion [Please provide a way to disable F10/F11 until debug mode is entered](https://developercommunity.visualstudio.com/idea/960671/please-provide-a-way-to-disable-f10f11-until-debug.html).

**F10** (*Step Over*) and **F11** (*Step Into*) are two commands people often hit by accident. That starts a new debugging session and that can be annoying if you didn't mean for that to happen. They should only take effect during a debug session. 

<h3 id="restart">Restart command</h3>

Inspired by the suggestion [Add a Restart Visual Studio option to 'File' menu](https://developercommunity.visualstudio.com/idea/537213/add-a-restart-visual-studio-option-to-file-menu.html).

Make it easy to restart Visual Studio. If it's running elevated, it will restart back into elevated mode. The shortcut key is **Shift+Alt+F4**.

![Restart](art/restart.png)

<h3 id="deleteoutputartifacts">Delete obj and bin on Clean</h3>

Inspired by the suggestion [Aggresive Clean Output Option](https://developercommunity.visualstudio.com/idea/698053/aggresive-clean-output-option.html).

Both *bin* and *obj* are now fully deleted when the *Clean* command is executed. It does so right before the regular *Clean* command runs. The *Clean* command will add the *bin* and *obj* folders back, so be aware of that when testing this feature.

<h3 id="formatonlinemove">Format on live move up/down</h3>

Inspired by the suggestion [Move lines up/down command should correct indentation](https://developercommunity.visualstudio.com/idea/1068219/move-lines-updown-command-should-correct-indentati.html).

When using **Alt+Up/Down** arrow keys to move lines up and down in the editor, those lines will now indent correctly as they move. This is supported for C#, Visual Basic, and HTML.

<h3 id="settings">Settings</h3>

You can enable or disable the various tweaks to your liking.

![Settings](art/settings.png)

## License
[Apache 2.0](LICENSE)