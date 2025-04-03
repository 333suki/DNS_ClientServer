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

static class ClientUDP {
    //TODO: [Deserialize Setting.json]
    static Setting? setting = JsonSerializer.Deserialize<Setting>(File.ReadAllText(Path.Combine("Setting.json")));
    private static Socket clientSocket;

    public static void start() {
        //TODO: [Create endpoints and socket]
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse(setting.ClientIPAddress), setting.ClientPortNumber);
        clientSocket.Bind(clientEndPoint);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(setting.ServerIPAddress), setting.ServerPortNumber);
        
        //TODO: [Create and send HELLO]
        SendMessageToServer(new Message { MsgId = 1, MsgType = MessageType.Hello, Content = "Hello from client!!!" }, serverEndPoint);

        //TODO: [Receive and print Welcome from server]
        ReceiveMessageFromServer(serverEndPoint);

        // TODO: [Create and send DNSLookup Message]
        (string type, string name)[] records = { ("A", "www.customdomain.com"), ("MX", "example.com"), ("CNAME", "www.funny_cat.com"), ("TXT", "https://visdeurbel.nl/") };
        for (int i = 0; i < records.Length; i++)
        {
            SendMessageToServer(new Message { MsgId = 33+i, MsgType = MessageType.DNSLookup, Content = new DNSRecord { Type = records[i].type, Name = records[i].name, Value = null, TTL = null, Priority = null } }, serverEndPoint);
            Message reply = ReceiveMessageFromServer(serverEndPoint);

            if (reply is not null && (reply.MsgType == MessageType.DNSLookupReply || reply.MsgType == MessageType.Error))
            {
                SendMessageToServer(new Message { MsgId = 4112, MsgType = MessageType.Ack, Content = reply.MsgId }, serverEndPoint);
            }
        }
        
        //TODO: [Send End message to  server]
        SendMessageToServer(new Message { MsgId = 91377, MsgType = MessageType.End, Content = "No Lookups anymore" }, serverEndPoint);
    }
    
    private static void SendMessageToServer(Message message, EndPoint serverEndPoint)
    {
        try
        {
            // Convert message object to JSON string
            string jsonMessage = JsonSerializer.Serialize(message);
            // Convert JSON string to bytes
            byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            // Send the bytes to the server
            clientSocket.SendTo(messageBytes, serverEndPoint);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"Sent message: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.WriteLine();
        } catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Error sending message: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(ex.Message);
            Console.WriteLine();
        }
    }

    private static Message ReceiveMessageFromServer(EndPoint serverEndPoint) {
        byte[] buffer = new byte[1024];
        // Put received bytes in the buffer
        int bytesReceived = clientSocket.ReceiveFrom(buffer, ref serverEndPoint);
        // Convert buffer to JSON string
        string jsonMessage = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
        // Convert JSON string to Message object
        Message message = JsonSerializer.Deserialize<Message>(jsonMessage);
        
        using (JsonDocument doc = JsonDocument.Parse(jsonMessage)) {
            if (doc.RootElement.TryGetProperty("Content", out JsonElement contentElement)) {
                try {
                    string contentJson = contentElement.GetRawText();
                    message.Content = JsonSerializer.Deserialize<DNSRecord>(contentJson);
                }
                catch (Exception e) {
                    
                }
            }
        }

        if (message.MsgType == MessageType.Error) {
            Console.ForegroundColor = ConsoleColor.Red;
        } else {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        
        Console.Write($"Received message: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.WriteLine();
        return message;
    }
}
