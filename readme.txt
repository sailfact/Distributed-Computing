Ross Curley - 19098081

Distributed Computing Prac 1&2 Readme

Date created: 16/03/18
Date last modified: 16/03/18

Purpose: To use .NET to create a program that loads TrueMarble satellite imagery via 
	interfaceing with a third party DLL.

Files in project: 

TrueMarbleData
	ITMDataController.cs
	Program.cs
	TMDataControllerImpl.cs
	TMDLLWrapper.cs
			
TrueMarbleGUI		  
	App.xaml
	App.xaml.cs
	MainWindow.xaml
	MainWindow.xaml.cs

Functionality: 

Run the TrueMarbleData Executable to start the server once it is running.
Run The TruemarbleGUI Executable to start the GUI interface.		
In the interface the button labeled Load Tile will load the tile from the server,
The Buttons labeled as the cardinal directions will move the image respectively,
The slider will increase or decease the zoom of the image.


TODO: Cleaner GUI, more try catches for exceptions I haven't considered yet.
	

No Known bugs

