using System.Collections.Immutable;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using LibData;

// SendTo();
class Program {
    static void Main(string[] args) {
        ClientUDP.start();
    }
}

public class Setting {
    public int ServerPortNumber { get; set; }
    public string? ServerIPAddress { get; set; }
    public int ClientPortNumber { get; set; }
    public string? ClientIPAddress { get; set; }
}

class ClientUDP {

    //TODO: [Deserialize Setting.json]
    static string configFile = @"../Setting.json";
    static string configContent = File.ReadAllText(configFile);
    static Setting? setting = JsonSerializer.Deserialize<Setting>(configContent);


    public static void start() {


        string serverIp = "145.137.58.204"; // Replace with your friend's IP
        int serverPort = 9000; // Must match server port

        // Create a UDP socket
        Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Define server endpoint
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

        string message = "MIAUW!";
        byte[] sendBytes = Encoding.UTF8.GetBytes(message);

        // Send message to server
        clientSocket.SendTo(sendBytes, serverEndPoint);
        Console.WriteLine($"Sent: {message} to {serverIp}:{serverPort}");

        clientSocket.Close();
    }

    //TODO: [Create endpoints and socket]


    //TODO: [Create and send HELLO]

    //TODO: [Receive and print Welcome from server]

    // TODO: [Create and send DNSLookup Message]


    //TODO: [Receive and print DNSLookupReply from server]


    //TODO: [Send Acknowledgment to Server]

    // TODO: [Send next DNSLookup to server]
    // repeat the process until all DNSLoopkups (correct and incorrect onces) are sent to server and the replies with DNSLookupReply

    //TODO: [Receive and print End from server]





}
