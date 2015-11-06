## SCR C# Client

#### Compile Instructions
1. Download this repository as zip.
2. Extract the contents anywhere you like.
3. To compile the client, simply run **Developer Command Prompt for VS20##** where ## is the version of Visual Studio installed on your system. You can search for it in the start menu.
4. Navigate to the **SCR** folder located inside **SCR-Client-DotNet** folder.
5. Type the following command to compile: 

		csc /out:Client.exe Action.cs SocketHandler.cs ISensorModel.cs MessageParser.cs MessageBasedSensorModel.cs Controller.cs SimpleDriver.cs Program.cs
6. This will compile and create the executable named **Client.exe**

#### Execute Instructions
1. To execute simply run the following command and specifying parameters for the client

		Client.exe DriverName host:<ip> port:<p> id:<client-id> maxEpisodes:<me> maxSteps:<ms> verbose:<v> track:<trackname> stage:<s>
        
Where **DriverName** is the implementation of the driver controller you want to use in the Client. As we have an example Controller named **SimpleDriver** we can use it for testing. Simply type the following command to launch the Client with the test Controller:

	Client.exe SCR.SimpleDriver port:3001
 
 Complete detail of the parameters is provided in the Manual found at http://cs.adelaide.edu.au/~optlog/SCR2015/software.html
