// Client
using System.Net.Sockets;
using System.Text.Json;
using System.Net;
using System.Text;

namespace GameClient
{
    class Client
    {
        static void Main(string[] args)
        {
            // Set the IP address and port number for the server
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8080;

            try
            {
                // Create a TCP/IP socket for the client
                TcpClient client = new TcpClient();
                client.Connect(ipAddress,port);
                System.Console.WriteLine("Connected to server.");

                // Get a network stream object for reading and writing data
                NetworkStream stream = client.GetStream();
                
                bool GameState = true;
                while(GameState)
                {
                    // Receive response from server
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if(data != null && data.Length > 0){
                        GameData? gameData = JsonSerializer.Deserialize<GameData>(data);
                        if(gameData != null) {
                            Console.Clear();
                            PrintBoard(gameData.GetGameBoard());
                            if(gameData.GetTurn() == 2){
                                // Ask user to input the coordinates to place their character
                                string row = GetInput("Type the number of the row in which you want to place: ");
                                string col = GetInput("Type the number of the column in which you want to place: ");
                                System.Console.WriteLine($"\nYou are placing in row:{row} column:{col}");

                                // Send message to server
                                byte[] rowData = Encoding.ASCII.GetBytes(row);
                                byte[] colData = Encoding.ASCII.GetBytes(col);
                                stream.Write(rowData, 0, rowData.Length);
                                stream.Write(colData,0,colData.Length);

                                if(gameData.GetGameState()){
                                    if(!gameData.GetValidPreviousMovement()){
                                        System.Console.WriteLine("That was not a valid movement!");
                                        continue;
                                    }     
                                } else {
                                    GameState = gameData.GetGameState();
                                }
                            }
                            System.Console.WriteLine("Waiting for player 1 to make a move....");
                            GameState = gameData.GetGameState();
                        }
                    }

                }
                System.Console.WriteLine("Someone Won - Game Finished");
                stream.Close();
                client.Close();
            }
            catch (SocketException)
            {
                System.Console.WriteLine("Make sure the server is running before you run the client");
            }
            catch (IOException)
            {
                System.Console.WriteLine("Player 1 disconnected, He was afraid of you!");
            }
        }
        
        // Ask user to input coords and validate them
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

        // prints the board
        private static void PrintBoard(char[][] board)
        {
            System.Console.WriteLine();
            System.Console.WriteLine(" --TIC-TAC-TOE--");
            System.Console.WriteLine();
            System.Console.WriteLine("   0    1   2");
            Console.WriteLine("  ┌───┬───┬───┐");
            for (int row = 0; row < 3; row++)
            {
                Console.Write($"{row} │");
                for (int col = 0; col < 3; col++)
                {
                    System.Console.Write($"{board[row][col]}  |");
                }
                System.Console.WriteLine();
                if(row != 2)
                {
                    System.Console.WriteLine("  ├───┼───┼───┤");
                }
            }
            Console.WriteLine("  └───┴───┴───┘");
        }
    }
}

