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

        static void Main(string[] args){

            // Start a new game
            game = new Game('X','O');

            bool PlayAgain = true;

            // Create a TCP/IP socket for the server
            listener = new TcpListener(ipAddress,port);
            listener.Start();

            do
            {
                game.Reset();

                // Intialize initial game data
                Client1Data = new GameData(true,game.GetState(),game.IsFull(),game.GetBoard(),false,game.GetPlayer1Score(),game.GetPlayer2Score());
                Client2Data = new GameData(true,game.GetState(),game.IsFull(),game.GetBoard(),false,game.GetPlayer2Score(),game.GetPlayer1Score());

                try
                {
                    System.Console.WriteLine("Waiting for players to connect...");

                    while(connectedClients < 2)
                    {
                        waitHandle.Reset();
                        listener.BeginAcceptTcpClient(new AsyncCallback(HandleClientConnection), listener);
                        waitHandle.WaitOne();
                    }

                    System.Console.WriteLine("Two players connected");

                    while(game.GetState()) RunGame();

                    PrintWinner();

                    // Send Final Data to Clients
                    Client1Data.SetGameState(false);
                    Client2Data.SetGameState(false);
                    SendData(client1,Client1Data);
                    SendData(client2,Client2Data);
                    System.Console.WriteLine("Data Sent");
                    
                    string response1 = ReceiveString(client1);
                    string response2 = ReceiveString(client2);

                    if(response1.ToUpper() == "Y" && response2.ToUpper() == "Y")
                    {
                        System.Console.WriteLine("Players are playing again! ");
                    }
                    else
                    {
                        PlayAgain = false;
                        System.Console.WriteLine("Not playing again");
                    }

                    // Reset the number of connected clients
                    connectedClients = 0;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }

            } while (PlayAgain);

            // Stop listening for incoming connections
            listener.Stop();

            // Close clients connections
            client1.Close();
            client2.Close();

        }

        // Prints the game winner and adds up to their score
        static void PrintWinner()
        {
            if(game.getTurn() == 2){
                game.SetPlayer1Score(game.GetPlayer1Score() + 1);
                System.Console.WriteLine($"{client1.Client.RemoteEndPoint.ToString()} won!");
            } else 
            {
                game.SetPlayer2Score(game.GetPlayer2Score() + 1);
                System.Console.WriteLine($"{client1.Client.RemoteEndPoint.ToString()} won!");
            }
            System.Console.WriteLine($"Player 1: {game.GetPlayer1Score()} - Player 2: {game.GetPlayer2Score()}");
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
                    SendData(client1,Client1Data);
                    SendData(client2,Client2Data);

                    // Change the current player                        
                    player = 'X';
                    // Receive coordinates from player
                    coordinates = ReceiveData(client1,Client1Data);
                break;
                // Its player 2 turn
                case 2:
                    System.Console.WriteLine($"Platyer {client2.Client.RemoteEndPoint.ToString()} turn");

                    // Set Player 2 Turn to true
                    Client2Data.SetYourTurn(true);

                    // Set Player 1 Turn to False
                    Client1Data.SetYourTurn(false);

                    // Send Data to Clients
                    SendData(client2,Client2Data);
                    SendData(client1,Client1Data);

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
            else
            {
                client2 = client;
            }
            waitHandle.Set();
        }

        // Send Data to Clients
        static void SendData(TcpClient client, GameData ClientData)
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
                    ClientData = new GameData(false,game.GetState(),game.IsFull(),game.GetBoard(),true, game.GetPlayer1Score(),game.GetPlayer2Score());
                    SendData(client,ClientData);
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

 