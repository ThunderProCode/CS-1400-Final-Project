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
    string? message = Console.ReadLine();
    while(message == null){
        message = Console.ReadLine();
    }
    byte[] data = Encoding.ASCII.GetBytes(message);
    stream.Write(data, 0, data.Length);

    // Receive response from server
    byte[] buffer = new byte[1024];
    int bytesRead = stream.Read(buffer, 0, buffer.Length);
    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
    Console.WriteLine("Server: " + response);
}

stream.Close();
client.Close();