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

            // Intialize initial game data
            Client1Data = new GameData(true,game.GetState(),game.IsFull(),game.GetBoard(),false);
            Client2Data = new GameData(true,game.GetState(),game.IsFull(),game.GetBoard(),false);

            try
            {
                // Create a TCP/IP socket for the server
                listener = new TcpListener(ipAddress,port);
                listener.Start();

                System.Console.WriteLine("Waiting for players to connect...");

                while(connectedClients < 2)
                {
                    waitHandle.Reset();
                    listener.BeginAcceptTcpClient(new AsyncCallback(HandleClientConnection), listener);
                    waitHandle.WaitOne();
                }

                System.Console.WriteLine("Two players connected");

                while(game.GetState())
                {
                    int[] coordinates = new int[]{};
                    char player = ' ';

                    switch(game.getTurn())
                    {
                        // Its players 1 turn
                        case 1:
                            System.Console.WriteLine($"Platyer {client1.Client.RemoteEndPoint.ToString()} turn");
                            Client1Data.SetYourTurn(true);
                            Client2Data.SetYourTurn(false);
                            SendData(client1,Client1Data);
                            SendData(client2,Client2Data);
                            // Change the current player                        
                            player = 'X';
                            // Receive coordinates from player
                            coordinates = ReceiveData(client1,Client1Data);
                        break;
                        // Its player 2 turn
                        case 2:
                            System.Console.WriteLine($"Platyer {client1.Client.RemoteEndPoint.ToString()} turn");
                            Client2Data.SetYourTurn(true);
                            Client1Data.SetYourTurn(false);
                            SendData(client2,Client2Data);
                            SendData(client1,Client1Data);
                            // Change the current player                        
                            player = 'O';
                            // Receive coordinates from player
                            coordinates = ReceiveData(client2,Client2Data);
                        break;
                    }

                    if(game.IsFull()) game.SetState(false);
                    if(game.GetState()){
                        game.Move(coordinates,player);
                        if(game.IsWin()) game.SetState(false);
                    }
                }

                System.Console.WriteLine("Someone Won");

                // Send Final Data to Clients
                Client1Data.SetGameState(false);
                Client2Data.SetGameState(false);
                SendData(client1,Client1Data);
                SendData(client2,Client2Data);

                // Stop listening for incoming connections
                listener.Stop();

                // Close clients connections
                client1.Close();
                client2.Close();

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            listener.Stop();
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
            // System.Console.WriteLine($"Sending data to client{ client.Client.RemoteEndPoint.ToString() }: {ClientData.GetYouTurn()}");
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
                    ClientData = new GameData(false,game.GetState(),game.IsFull(),game.GetBoard(),true);
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

 