// Client
using System.Net.Sockets;
using System.Text.Json;
using System.Net;
using System.Text;

namespace GameClient
{
    class Client
    {
        static bool PlayAgain = true;
        static bool PlayerDisconnected = false;
        static void Main(string[] args)
        {
            int opt = 0;
            while(opt != 2){
                opt = MainMenu();
                switch(opt)
                {
                    // Play the game
                    case 1:
                        RunGame();
                    break;
                    // Exit the game
                    case 2:
                        System.Console.WriteLine("Thanks for playing, see you later!");
                    break;

                    default:
                        System.Console.WriteLine("Invalid Option");
                    break;
                }
            }
        }

        // Connect to server and runs game logic
        private static void RunGame()
        {
            // Set the IP address and port number for the server
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 8080;

            do
            { 
                try
                {
                    // Create a TCP/IP socket for the client
                    TcpClient client = new TcpClient();
                    client.Connect(ipAddress,port);
                    System.Console.WriteLine("Connected to server.");
                    System.Console.WriteLine("Waiting for Player 2....");
                    HandleGame(client);
                }
                catch (SocketException)
                {
                    System.Console.WriteLine("Make sure the server is running before you run the client");
                    break;
                }
                catch (IOException)
                {
                    System.Console.WriteLine("Player 2 disconnected, He was afraid of you!");
                    break;
                }
            } while (PlayAgain);
        }

        // Handle game logic
        private static void HandleGame(TcpClient client)
        {
            // Get a network stream object for reading and writing data
            NetworkStream stream = client.GetStream();

            bool Draw = false;
            bool IsYourTurn = false;

            while(true)
            {
                // Receive response from server
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if(data != null && data.Length > 0)
                {
                    GameData gameData = JsonSerializer.Deserialize<GameData>(data);

                    if(gameData != null)
                    {                     
                        if(gameData.PlayersConnected)
                        {
                            PlayerDisconnected = false;
                            PrintBoard(gameData.GetGameBoard());
                            PrintScores(gameData.GetMyScore(),gameData.GetPlayer2Score());

                            if(gameData.GetIsFull() == true)
                            {
                                Draw = true;
                                break;    
                            }

                            if(gameData.GetGameState() == false) break;

                            // Check if it's the current player's turn
                            if(gameData.GetYourTurn())
                            {
                                IsYourTurn = true;
                                // Request user for Coordinates and send them to server
                                SendData(stream);

                                // Repeat until user inputs a valid movement
                                if(!gameData.GetValidPreviousMovement()){
                                    System.Console.WriteLine("That was not a valid movement!");
                                    continue;
                                } 
                            } 
                            // If it's not the current player's turn
                            else 
                            {
                                IsYourTurn = false;
                                System.Console.WriteLine("Waiting for other player to make a move....");
                            }
                        } else 
                        {
                            PlayerDisconnected = true;
                            PlayAgain = false;
                            break;
                        }       
                    }
                }
            }

            if(!PlayerDisconnected)
            {
                PrintFinalMessage(Draw, IsYourTurn);
                AskPlayAgain(client);
            } else 
            {
                System.Console.WriteLine("Player 2 disconnected, He was afraid of you!");
            }
            stream.Close();
        }

        private static void PrintScores(int MyScore, int Score2)
        {
            System.Console.WriteLine($"Your Score: {MyScore} - Player 2: {Score2}");
            System.Console.WriteLine();
        }

        // Print the main menu and ask user for input
        private static int MainMenu()
        {
            System.Console.WriteLine("====MENU====\n1)Play\n2)Exit\nType an option:");
            string input = Console.ReadLine();
            bool correctInput = Int32.TryParse(input,out int option);
            while(!correctInput){
                System.Console.WriteLine("Please type a correct option: ");
                input = Console.ReadLine();
            }
            return option;
        }

        // Ask user if wants to play again and send to server
        private static void AskPlayAgain(TcpClient server)
        {
            System.Console.WriteLine("Do you want to play again? (Y - yes, N - no): ");
            string response = Console.ReadLine();
            response = response.ToUpper();

            while(response.Length > 1 || (response != "Y" && response != "N"))
            {
                System.Console.WriteLine("Please type Y or N: ");
                response = Console.ReadLine();
                response = response.ToUpper();
            }

            if(response == "Y")
            {
                PlayAgain = true;
            } else 
            {
                PlayAgain = false;
            }

            NetworkStream stream = server.GetStream();
            byte[] responseData = Encoding.ASCII.GetBytes(response);
            stream.Write(responseData, 0, responseData.Length);

        }

        // Print message at the end of a game
        private static void PrintFinalMessage(bool Draw, bool IsYourTurn)
        {
            if(Draw)
            {
                System.Console.WriteLine("Its a tie, no one Won");
            } else 
            {
                if(IsYourTurn)
                {
                    System.Console.WriteLine("You Won!");
                } else
                {
                    System.Console.WriteLine("Player 2 Won");
                }
            }
        }

        // Ask user to input coordinates and send them to server
        private static void SendData(NetworkStream stream)
        {
            // Ask user to input the coordinates to place their character
            string row = GetInput("Type the number of the row in which you want to place: ");
            string col = GetInput("Type the number of the column in which you want to place: ");
            System.Console.WriteLine($"\nYou are placing in row:{row} column:{col}");

            // Send message to server
            byte[] rowData = Encoding.ASCII.GetBytes(row);
            byte[] colData = Encoding.ASCII.GetBytes(col);
            stream.Write(rowData, 0, rowData.Length);
            stream.Write(colData,0,colData.Length);
            stream.Flush();
        }
        
        // Ask user to input coords and validate them
        private static string GetInput(string message){
            System.Console.Write(message);
            string input = Console.ReadLine();

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

        // Prints the game instructions
        private static void PrintInstructions()
        {
            System.Console.WriteLine("Instructions:\n1- Type the number of the row in which you want to place\n2- Type the number of the column in which you want to place\n3- If your movement was not valid, you will have to type the inputs again until you make a valid movement.\nGoodluck!\n");
        }

        // prints the board
        private static void PrintBoard(char[][] board)
        {
            Console.Clear();
            PrintInstructions();
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
            System.Console.WriteLine();
        }
    }
}

