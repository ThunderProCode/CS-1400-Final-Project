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
            game.Print();

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
            
            // gameData is the data being sent to the client
            GameData gameData = new GameData(true,game.GetState(),game.GetBoard(),game.getTurn());

            // Serialize the GameData to JSON
            var currentJson = JsonSerializer.Serialize(gameData);

            // Send the initial GameData to Client
            byte[] currentBytes = Encoding.ASCII.GetBytes(currentJson);
            stream.Write(currentBytes,0,currentBytes.Length);

            // While the game is running (No one won)
            while(game.GetState())
            {
                int[] coordinates;
                char player;
                // If it his the turn of the player 2, the client moves first
                if(game.getTurn() == 2){
                    // Change the current player
                    player = 'X';
                    // Receive coords from player2
                    byte[] buffer = new byte[1024];
                    int rowBytes = stream.Read(buffer,0,buffer.Length);
                    string clientRow = Encoding.ASCII.GetString(buffer,0,rowBytes);
                    int colBytes = stream.Read(buffer,0,buffer.Length);
                    string clientCol = Encoding.ASCII.GetString(buffer,0,colBytes);

                    // Parse coordinates to int[]
                    coordinates = ParseCoords(clientRow,clientCol);
                    if(!game.IsMoveValid(coordinates)){
                        gameData = new GameData(false,game.GetState(),game.GetBoard(),game.getTurn());
                    } else {
                        game.setTurn(1);
                        gameData = new GameData(true,game.GetState(),game.GetBoard(),game.getTurn());
                    }
                
                // If the turn is 1 the server moves first
                } else {
                    // Change the current player
                    player = 'O';

                    // Ask user to input coordinates
                    string localRow = GetInput("Type the number of the row in which you want to place: ");
                    string localCol = GetInput("Type the number of the column in which you want to place: ");

                    // Parse coordinates to int[]
                    coordinates = ParseCoords(localRow,localCol);

                    if(!(game.IsMoveValid(coordinates))){
                        System.Console.WriteLine("That was not a valid movement!");   
                    } else {
                        game.setTurn(2);
                    }
                }

                // try to make a movement in the board
                    if(game.GetState()){
                        if(game.IsMoveValid(coordinates)){
                            game.Move(coordinates,player);
                            Console.Clear();
                            game.Print();
                            if(game.IsWin()){
                                game.SetState(false);
                            }
                            gameData = new GameData(true,game.GetState(),game.GetBoard(),game.getTurn());
                        }
                    }

                    // Serialize the GameData to JSON
                    var json = JsonSerializer.Serialize(gameData);

                    // Send GameData to Client
                    byte[] bytes = Encoding.ASCII.GetBytes(json);
                    stream.Write(bytes,0,bytes.Length);

                    System.Console.WriteLine("Waiting for player 2 to make a move.....");
            }
            System.Console.WriteLine("Someone Won - Game Finished");

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

        // Ask user to input coordinates and validate them
        private static string GetInput(string message){
            System.Console.Write(message);
            string? input = Console.ReadLine();

            // Validate the user input
            bool correctInput = false;
            while(input == null || correctInput == false){
                if(input != null){
                    if(Int32.TryParse(input,out int rowNum))
                    {
                        if(rowNum < 0 || rowNum > 2)
                        {
                            System.Console.WriteLine("Please type a number between 0 and 2");
                            input = Console.ReadLine();
                        } else 
                        {
                            correctInput = true;
                        } 
                    }else {
                        System.Console.WriteLine("Please type a number between 0 and 2");
                        input = Console.ReadLine();
                    }            
                } else {
                    System.Console.WriteLine("Please type number common!: ");
                    input = Console.ReadLine();
                }
            }
            return input;
        }
    }
}

