// Server
using System.Text.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
namespace GameServer
{
    class Server 
    {
        static void Main(string[] args){
            Game game = new Game('X','O');

            // Set the IP Address and port number 
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8080;

            // Create a TCP/IP socket for the server
            TcpListener listener = new TcpListener(ipAddress,port);
            listener.Start();

            System.Console.WriteLine($"Server started on {ipAddress}:{port}");

            // Accept incoming client connections
            TcpClient client = listener.AcceptTcpClient();
            System.Console.WriteLine("Client Connected.");

            // Get a network stream object for reading and writing data
            NetworkStream stream = client.GetStream();

            // Create a new Game
            while(game.GetState())
            {
                // Receive coords from player2
                byte[] buffer = new byte[1024];
                int rowBytes = stream.Read(buffer,0,buffer.Length);
                string row = Encoding.ASCII.GetString(buffer,0,rowBytes);
                int colBytes = stream.Read(buffer,0,buffer.Length);
                string col = Encoding.ASCII.GetString(buffer,0,colBytes);
                
                // Parse coordinates to int[]
                int[] coordinates = ParseCoords(row,col);
                GameData gameData = new GameData(false,game.GetState(),game.GetBoard());

                // try to make a movement in the board
                if(game.IsMoveValid(coordinates)){
                    game.Move(coordinates,'X');
                    game.Print();
                    gameData = new GameData(true,game.GetState(),game.GetBoard());
                }

                // Serialize the GameData to JSON
                var json = JsonSerializer.Serialize(gameData);

                // Send GameData to Client
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                stream.Write(bytes,0,bytes.Length);

            }

            // Close the network stream and client connection
            stream.Close();
            client.Close();

            // Stop listening for incoming connections
            listener.Stop();
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

