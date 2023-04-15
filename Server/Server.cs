// Server
using System.Net.Sockets;
using System.Net;
using System.Text;
namespace GameServer
{
    class Server 
    {
        static void Main(string[] args){
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

            while(true)
            {
                // Receive messaeg from client
                byte[] buffer = new byte[1024];
                int bytes = stream.Read(buffer,0,buffer.Length);
                string message = Encoding.ASCII.GetString(buffer,0,bytes);
                System.Console.WriteLine("Client: "+message);

                // Send response back to client
                Console.Write("Server: ");
                string? response = Console.ReadLine();
                while(response == null){
                    response = Console.ReadLine();
                }
                byte[] data = Encoding.ASCII.GetBytes(response);
                stream.Write(data, 0, data.Length);
            }

            // Close the network stream and client connection
            stream.Close();
            client.Close();

            // Stop listening for incoming connections
            listener.Stop();
        }
    }
}

