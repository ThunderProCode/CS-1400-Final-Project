## Online Tic-Tac-Toe (In Progress)

### This is a console online tic-tac-toe game made with .net. 
- The project is a C# console-based Tic Tac Toe game with a client-server architecture. The game allows two players to play against each other, with one player acting as the server and the other as the client.

- The server-side code includes the main functionality of the game, such as accepting connections, starting the game, receiving and processing moves from the client, and sending game data back to the client. The server side uses the TcpListener class to listen for incoming client connections and the NetworkStream class to read and write data.

- The client-side code includes a separate console application that connects to the server using the TcpClient class. Once the connection is established, the client receives the initial game data from the server, displays the game board, and prompts the user to enter their move. The client then sends the move to the server and waits for the updated game data to be sent back.

- The game is played by entering row and column numbers to indicate where the player wants to place their mark. The game board is displayed as a 3x3 grid with each square being labeled with its corresponding row and column number. The first player to get three in a row (horizontally, vertically, or diagonally) wins the game.

### How to execute the game
 1. Clone this repo
 2. Open the Server Folder and Run it.
 3. Open two new Console Instances, open and run the Client Folder in each Console Instance. Each Console is a Player. Once the two players connect, the game will start.
 4. Once the clients are connected to the server, the game will start automatically. The server will wait for the first move from the client, and the client will wait for the other clients response before making its next move.
 5. The game ends when either one of the clients wins, or when the game is a draw.

 Note: The default IP address used in the server code is "127.0.0.1", which is the loopback address. If you want to connect to the server from a different machine, you need to replace this IP address with the IP address of the server machine. Also, make sure that the firewall is not blocking the incoming connections to the server.