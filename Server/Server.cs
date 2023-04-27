// Server
using System.Text.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
namespace GameServer
{
    class Server 
    {
        // Set the IP Address and port number 
        static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        static int port = 8080;
        // Create a TCP/IP socket for the server
        static TcpListener listener;
        static ManualResetEvent waitHandle = new ManualResetEvent(false);
        static int connectedClients = 0;
        static TcpClient client1;
        static TcpClient client2;
        static Game game;
        static GameData Client1Data;
        static GameData Client2Data;
        static int Player1Score = 0;
        static int Player2Score = 0;
        static void Main(string[] args){

            // Start a new game
            game = new Game('X','O');

            bool PlayAgain = false;

            // Create a TCP/IP socket for the server
            listener = new TcpListener(ipAddress,port);
            listener.Start();

            do
            {
                if(PlayAgain)
                {
                    game.Reset();
                } else
                {
                    game.Reset();
                    Player1Score = 0;
                    Player2Score = 0;
                }

                System.Console.WriteLine($"PRIMERO Player 1: {Player1Score} - Player 2: {Player2Score}");

                try
                {
                    if(!PlayAgain)
                    {
                        System.Console.WriteLine("Waiting for players to connect...");
                        while(connectedClients < 2)
                        {
                            waitHandle.Reset();
                            listener.BeginAcceptTcpClient(new AsyncCallback(HandleClientConnection), listener);
                            waitHandle.WaitOne();
                        }
                        System.Console.WriteLine("Two players connected");
                    }

                    // Intialize initial game data
                    Client1Data = new GameData(true,game.GetState(),game.IsFull(),game.GetBoard(),false,Player1Score,Player2Score,true,true);
                    Client2Data = new GameData(true,game.GetState(),game.IsFull(),game.GetBoard(),false,Player2Score,Player1Score,true,true);

                    while(true)
                    {
                        GameData PlayerDisconnectedData = new GameData(false,false,true,game.GetBoard(),false,0,0,false,false);

                        // Check if either player has disconnected
                        if(!client1.Connected)
                        {
                            System.Console.WriteLine("Player 1 has disconnected, Game has ended");
                            connectedClients = 0;
                            SendGameData(client2,PlayerDisconnectedData);
                            client2.Close();
                            PlayAgain = false;
                            break;
                        } else if(!client2.Connected)
                        {
                            System.Console.WriteLine("Player 2 has disconnected, Game has ended");
                            connectedClients = 0;
                            SendGameData(client1,PlayerDisconnectedData);
                            client1.Close();
                            PlayAgain = false;
                            break;
                        }

                        RunGame();

                        // If the game has ended, break out of the loop
                        if(!game.GetState())
                        {
                            break;
                        }
                    }

                    // Send Final Data to Clients
                    Client1Data.SetGameState(false);
                    Client2Data.SetGameState(false);
                    SendGameData(client1,Client1Data);
                    SendGameData(client2,Client2Data);
                    System.Console.WriteLine("Data Sent");

                    PrintWinner();
                    
                    string response1 = ReceiveString(client1); 
                    System.Console.WriteLine("SEXO:"+response1);
                    string response2 = ReceiveString(client2);
                    System.Console.WriteLine("SEXO:"+response2);
                    

                    if(response1.ToUpper() == "Y" && response2.ToUpper() == "Y")
                    {
                        PlayAgain = true;
                        System.Console.WriteLine("Players are playing again! ");
                        SendMessage(client1,"PLAYINGAGAIN");
                        SendMessage(client2,"PLAYINGAGAIN");
                    }
                    else
                    {
                        PlayAgain = false;
                        SendMessage(client1,"NOTPLAYINGAGAIN");
                        SendMessage(client2,"NOTPLAYINGAGAIN");
                        System.Console.WriteLine("Not playing again");
                    }

                    // Reset the number of connected clients
                    connectedClients = 0;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }

            } while (true);
        }

        // Prints the game winner and adds up to their score
        static void PrintWinner()
        {
            string response1 = ReceiveString(client1);
            System.Console.WriteLine("PrintWinner1"+response1);
            string response2 = ReceiveString(client2);
            System.Console.WriteLine("PrintWinner2"+response2);

            if(response1 == "WINNER")
            {
                Player1Score++;
            } else if(response2 == "WINNER")
            {
                Player2Score++;
            }
            System.Console.WriteLine($"Player 1: {Player1Score} - Player 2: {Player2Score}");
        }

        // Executes the game logic
        static void RunGame()
        {
            int[] coordinates = new int[]{};
            char player = ' ';

            switch(game.getTurn())
            {
                // Its players 1 turn
                case 1:
                    System.Console.WriteLine($"Player {client1.Client.RemoteEndPoint.ToString()} turn");

                    // Set Player 1 Turn to True
                    Client1Data.SetYourTurn(true);

                    // Set Player 2 Turn to False
                    Client2Data.SetYourTurn(false);

                    // Send Data to Players
                    SendGameData(client1,Client1Data);
                    SendGameData(client2,Client2Data);

                    // Change the current player                        
                    player = 'X';
                    // Receive coordinates from player
                    coordinates = ReceiveData(client1,Client1Data);
                break;
                // Its player 2 turn
                case 2:
                    System.Console.WriteLine($"Player {client2.Client.RemoteEndPoint.ToString()} turn");

                    // Set Player 2 Turn to true
                    Client2Data.SetYourTurn(true);

                    // Set Player 1 Turn to False
                    Client1Data.SetYourTurn(false);

                    // Send Data to Clients
                    SendGameData(client2,Client2Data);
                    SendGameData(client1,Client1Data);

                    // Change the current player                        
                    player = 'O';
                    // Receive coordinates from player
                    coordinates = ReceiveData(client2,Client2Data);
                break;
            }
            // If the board is full, means its a tie
            if(game.IsFull()) game.SetState(false);

            if(game.GetState()){
                game.Move(coordinates,player);
                if(game.IsWin()) game.SetState(false);
            }
        }

        // Receive string data from clients
        static string ReceiveString(TcpClient client)
        {
            // Get a network stream object for reading and writing data
            NetworkStream stream = client.GetStream();

            // Read data from client
            byte[] buffer = new byte[1024];
            int bytes = stream.Read(buffer, 0, buffer.Length);
            string data = Encoding.ASCII.GetString(buffer, 0, bytes);
            System.Console.WriteLine(data);
            return data.Trim();
        }

        // Handle Incoming Connections to server
        static void HandleClientConnection(IAsyncResult result)
        {
            
            TcpListener listener = (TcpListener)result.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(result);
            Interlocked.Increment(ref connectedClients);

            if (connectedClients == 1)
            {
                client1 = client;
            }
            else if(connectedClients == 2)
            {
                client2 = client;
            } else 
            {
                // We already have two players, so close the connection
                client.Close();
            }
            waitHandle.Set(); 

            // If one of the clients disconnected, wait for another player to connect before resuming the game
            if (connectedClients < 2)
            {
                System.Console.WriteLine("Waiting for another player to connect...");
                return;
            }

        }

        // Send String to clients
        static void SendMessage(TcpClient client,string message)
        {
            // Get a network stream object for reading and writing data
            NetworkStream stream = client.GetStream();

            // Send game data to client
            byte[] currentBytes = Encoding.ASCII.GetBytes(message);
            stream.Write(currentBytes,0,currentBytes.Length);
            stream.Flush();
        }

        // Send GameData to Clients
        static void SendGameData(TcpClient client, GameData ClientData)
        {
            // Get a network stream object for reading and writing data
            NetworkStream stream = client.GetStream();

            // Serialize the GameData to JSON
            var currentJson = JsonSerializer.Serialize(ClientData);

            // Send game data to client
            byte[] currentBytes = Encoding.ASCII.GetBytes(currentJson);
            stream.Write(currentBytes,0,currentBytes.Length);
            stream.Flush();
        }
        
        // Receive coordinates from clients
        static int[] ReceiveData(TcpClient client, GameData ClientData)
        {
            int[] coordinates;
            // Get a network stream object for reading and writing data
            NetworkStream stream = client.GetStream();

            do
            {
                // Read coordinates from client
                byte[] buffer = new byte[1024];
                int rowBytes = stream.Read(buffer,0,buffer.Length);
                string clientRow = Encoding.ASCII.GetString(buffer,0,rowBytes);
                int colBytes = stream.Read(buffer,0,buffer.Length);
                string clientCol = Encoding.ASCII.GetString(buffer,0,colBytes);
                
                // Parse coordinates to int[]
                coordinates = ParseCoords(clientRow,clientCol);
                if(!game.IsMoveValid(coordinates))
                {
                    SendMessage(client,"NOTVALIDMOVEMENT");
                    System.Console.WriteLine("SE MANDO SEXO");
                }
                
            } while (!game.IsMoveValid(coordinates));

            return coordinates;
        }

        // Parse and returns coordinates in string format to int[]
        private static int[] ParseCoords(string row,string col)
        {
            int[] coordinates = new int[2];
            row = row.Trim();
            col = col.Trim();
            coordinates[0] = Convert.ToInt32(row);
            coordinates[1] = Convert.ToInt32(col);
            return coordinates;
        }
    }
}

 