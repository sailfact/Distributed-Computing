# Distributed-Computing
Ross Curley - 19098081

Distributed Computing Prac 3,4,5 Readme

Date created: 13/04/18
Date last modified: 13/04/18

Purpose: To use .NET to create a program that loads TrueMarble satellite imagery via
	interfaceing with a third party DLL.

Files in project:

TrueMarbleData
	ITMDataController.cs
	Program.cs
	TMDataControllerImpl.cs
	TMDLLWrapper.cs

TrueMarbleBiz
	BrowseHistory.cs
	ITMBizController.cs
	Program.cs
	TMBizController.cs

TrueMarbleGUI		  
	App.xaml
	App.xaml.cs
	DisplayWindow.xaml
	DisplayWindow.xaml.cs
	MainWindow.xaml
	MainWindow.xaml.cs

Functionality:

Run the TrueMarbleData Executable to start the data server once it is running,
Run the TrueMarbleBiz Executable to start the business server once that is running,
Run The TruemarbleGUI Executable to start the GUI interface.		
In the interface the button labeled Load Tile will load the tile from the server,
The Buttons labeled as the cardinal directions will move the image respectively,
The slider will increase or decease the zoom of the image. The back and forward buttons will load the tiles from history, the file menu item can save and load the History list from the users home folder, the history menu item will load the history window which displays all the history coordinates. Biz tier will verify tiles asynchronously then notify the user when done.

TODO: Cleaner GUI, implement the ability the select the entry from the history window and load the tile, implement a progress bar so the user can see the tiles being verified.

known problems
I could no get the TrueMarbleData to read the DLL from System 32 so it is located in the datas bin/debug folder.
