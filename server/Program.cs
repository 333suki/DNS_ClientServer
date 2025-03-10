﻿using System;
using System.Data;
using System.Data.SqlTypes;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using LibData;

// ReceiveFrom();
class Program {
    static void Main(string[] args) {
        ServerUDP.start();
    }
}

public class Setting {
    public int ServerPortNumber { get; set; }
    public string? ServerIPAddress { get; set; }
    public int ClientPortNumber { get; set; }
    public string? ClientIPAddress { get; set; }
}


class ServerUDP
{
    static string configFile = @"../Setting.json";
    static string configContent = File.ReadAllText(configFile);
    static Setting? setting = JsonSerializer.Deserialize<Setting>(configContent);

    // TODO: [Read the JSON file and return the list of DNSRecords]


    public static void start() {
        int port = 9000; // Port to listen on

        // Create a UDP socket
        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Bind to any available IP on the specified port
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, port);
        serverSocket.Bind(serverEndPoint);

        Console.WriteLine($"Server is listening on port {port}...");

        // Buffer to receive data
        byte[] buffer = new byte[1024];
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (true) {
            // Receive data from client
            int receivedBytes = serverSocket.ReceiveFrom(buffer, ref remoteEndPoint);
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

            Console.WriteLine($"Received: {receivedMessage} from {remoteEndPoint}");
        }

        // TODO: [Create a socket and endpoints and bind it to the server IP address and port number]



        // TODO:[Receive and print a received Message from the client]




        // TODO:[Receive and print Hello]



        // TODO:[Send Welcome to the client]


        // TODO:[Receive and print DNSLookup]


        // TODO:[Query the DNSRecord in Json file]

        // TODO:[If found Send DNSLookupReply containing the DNSRecord]



        // TODO:[If not found Send Error]


        // TODO:[Receive Ack about correct DNSLookupReply from the client]


        // TODO:[If no further requests receieved send End to the client]

    }
}
