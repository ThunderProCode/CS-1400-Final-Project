// Client
using System.Net.Sockets;
using System.Net;
using System.Text;

// Set the IP address and port number for the server
IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
int port = 8080;

// Create a TCP/IP socket for the client
TcpClient client = new TcpClient();
client.Connect(ipAddress,port);
System.Console.WriteLine("Connected to server.");

// Get a network stream object for reading and writing data
NetworkStream stream = client.GetStream();

while(true)
{
     // Send message to server
    Console.Write("Client: ");
    string row = GetInput("Type the number of the row to which you want to move: ");
    string col = GetInput("Type the number of the column to which you want to move: ");
    byte[] rowData = Encoding.ASCII.GetBytes(row);
    byte[] colData = Encoding.ASCII.GetBytes(col);
    stream.Write(rowData, 0, rowData.Length);
    stream.Write(colData,0,colData.Length);

    // Receive response from server
    byte[] buffer = new byte[1024];
    int bytesRead = stream.Read(buffer, 0, buffer.Length);
    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
    Console.WriteLine("Server: " + response);
}

stream.Close();
client.Close();

string GetInput(string message){
    System.Console.Write(message);
    string? input = Console.ReadLine();
    while(input == null || input.Length == 0){
        input = Console.ReadLine();
        if(Int32.TryParse(input,out int rowNum))
        {
            if(rowNum < 0 || rowNum > 2)
            {
                System.Console.WriteLine("Please type a number between 0 and 2");
                input = null;
            } 
        }else {
            System.Console.WriteLine("Please type a number");
            input = null;
        }            
    }
    return input;
}

// prints the board
void PrintBoard(char[,] board)
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
            System.Console.Write($"{board[row,col]}  |");
        }
        System.Console.WriteLine();
        if(row != 2)
        {
            System.Console.WriteLine("  ├───┼───┼───┤");
        }
    }
    Console.WriteLine("  └───┴───┴───┘");
}