using System.Text.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace GameServer
{
    class Tests 
    {
        [TestMethod]
        public void Test_HandleClientDisconnection()
        {
            TcpClient client = new TcpClient();
            GameData gameData = new GameData(false, false, true, null, false, 0, 0, false, false);

            // Call HandleClientDisconnection method
            Server.HandleClientDisconnection(client);

            // Check if client is disconnected and GameData is sent to the client
            Assert.IsFalse(client.Connected);
            // Check if GameData is correct
            Assert.AreEqual(gameData.GetGameState(), false);
            Assert.AreEqual(gameData.IsFull(), true);
            Assert.AreEqual(gameData.GetBoard(), null);
            Assert.AreEqual(gameData.GetPlayer1Score(), 0);
            Assert.AreEqual(gameData.GetPlayer2Score(), 0);
            Assert.AreEqual(gameData.IsYourTurn(), false);
            Assert.AreEqual(gameData.IsPlayer1(), false);
        }

        // Test for ReceiveString method
        [TestMethod]
        public void Test_ReceiveString()
        {
            // Create a new TcpClient object
            TcpClient client = new TcpClient();

            // Connect to the local host
            client.Connect(IPAddress.Parse("127.0.0.1"), 8080);

            // Send a message to the server
            byte[] buffer = Encoding.ASCII.GetBytes("Hello");
            client.GetStream().Write(buffer, 0, buffer.Length);

            // Receive the message from the server
            string message = Server.ReceiveString(client);

            // Assert that the message received is the same as the message sent
            Assert.AreEqual("Hello", message);

            // Close the client
            client.Close();
        }

    }
}