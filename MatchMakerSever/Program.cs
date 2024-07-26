//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;

//class MatchMakerSever
//{
//    static void Main()
//    {
//        StartMatchmakerServer(5000);
//    }

//    static void StartMatchmakerServer(int port)
//    {
//        UdpClient udpClient = new UdpClient(port);
//        Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
//        Dictionary<string, IPEndPoint> rooms = new Dictionary<string, IPEndPoint>();


//        Console.WriteLine($"Matchmaking server started on port {port}...");
//        //HOST:QIEWR(Random 5 Charactor)
//        //MATCH:QIEWR:
//        while (true)
//        {
//            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
//            byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
//            string receivedMessage = Encoding.ASCII.GetString(receivedBytes);

//            Console.WriteLine($"Received message from {remoteEndPoint.Address}:{remoteEndPoint.Port}: {receivedMessage}");

//            if (receivedMessage.StartsWith("HOST"))
//            {
//                Console.WriteLine("HOST");
//                string clientId = receivedMessage.Split(':')[1];
//                string message = receivedMessage.Split(':')[2];

//                clients[clientId] = remoteEndPoint;
//                rooms[message] = remoteEndPoint;

//                Console.WriteLine($"Registered new client: {clientId}");


//                PrintKeys(rooms);

//            }
//            else if (receivedMessage.StartsWith("MATCH"))
//            {
//                Console.WriteLine("MATCH");
//                IPEndPoint client = remoteEndPoint;
//                string roomId = receivedMessage.Split(':')[1];
//                string clientId = receivedMessage.Split(':')[2];
//                Console.WriteLine($"Matching {clientId} To {roomId}");

//                PrintKeys(rooms);

//                if (rooms.ContainsKey(roomId))
//                {
//                    Console.WriteLine("Contained");
//                    byte[] clientMessage = Encoding.ASCII.GetBytes($"PEER:{rooms[roomId].Address}:{rooms[roomId].Port}");

//                    udpClient.Send(clientMessage, clientMessage.Length, client);
//                    Console.WriteLine($"Matched {clientId} To {roomId}");
//                }
//            }
//            else if (receivedMessage.StartsWith("CLOSE"))
//            {
//                Console.WriteLine("CLOSE");


//                PrintKeys(rooms);

//                string roomId = receivedMessage.Split(':')[1];
//                if(rooms.ContainsKey(roomId))
//                {
//                    Console.WriteLine("Contained");
//                    rooms.Remove(roomId);
//                }
//            }
//        }
//    }

//    private static void PrintKeys<T>(Dictionary<string, T> dictionary)
//    {
//        foreach (string key in dictionary.Keys)
//        {
//            string roomLists = "";
//            Console.WriteLine($"key : {key}");
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

class MatchMakerSever
{
    static void Main()
    {
        StartMatchmakerServer(5000);
    }

    static void StartMatchmakerServer(int port)
    {
        UdpClient udpClient = new UdpClient(port);
        Dictionary<string, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        Dictionary<string, IPEndPoint> rooms = new Dictionary<string, IPEndPoint>();
        Dictionary<string, string> ids = new Dictionary<string, string>();


        Console.WriteLine($"Matchmaking server started on port {port}...");
        //HOST:QIEWR(Random 5 Charactor)
        //MATCH:QIEWR:
        while (true)
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
            string receivedMessage = Encoding.ASCII.GetString(receivedBytes);

            Console.WriteLine($"Received message from {remoteEndPoint.Address}:{remoteEndPoint.Port}: {receivedMessage}");

            if (receivedMessage.StartsWith("HOST"))
            {
                Console.WriteLine("HOST");
                string clientId = receivedMessage.Split(':')[1];
                string ip = receivedMessage.Split(':')[2];
                string id = receivedMessage.Split(':')[3];

                IPEndPoint clientLocal = new IPEndPoint(IPAddress.Parse(ip), 7777);
                clients[clientId] = clientLocal;
                rooms[clientId] = clientLocal;
                ids[clientId] = id;

                Console.WriteLine($"Registered new client: {clientId}");


                PrintKeys(rooms);

            }
            else if (receivedMessage.StartsWith("MATCH"))
            {
                Console.WriteLine("MATCH");
                IPEndPoint client = remoteEndPoint;
                string roomId = receivedMessage.Split(':')[1];
                string clientId = receivedMessage.Split(':')[2];
                Console.WriteLine($"Matching {clientId} To {roomId}");

                PrintKeys(rooms);

                if (rooms.ContainsKey(roomId))
                {
                    Console.WriteLine("Contained");
                    byte[] clientMessage = Encoding.ASCII.GetBytes($"PEER:{rooms[roomId].Address}:{rooms[roomId].Port}");

                    udpClient.Send(clientMessage, clientMessage.Length, client);
                    Console.WriteLine($"Matched {clientId} To {roomId}");
                }
            }
            else if (receivedMessage.StartsWith("CLOSE"))
            {
                Console.WriteLine("CLOSE");


                PrintKeys(rooms);

                string roomId = receivedMessage.Split(':')[1];
                if (rooms.ContainsKey(roomId))
                {
                    Console.WriteLine("Contained");
                    rooms.Remove(roomId);
                }
            }
            else if (receivedMessage.StartsWith("LIST"))
            {
                Console.WriteLine("LIST");

                IPEndPoint client = remoteEndPoint;
                PrintKeys(rooms);

                string roomId = "LIST:";
                foreach ( string itemRoomId in rooms.Keys )
                {
                    roomId += $"{itemRoomId}.{ids[itemRoomId]},";
                }
                byte[] clientMessage = Encoding.ASCII.GetBytes(roomId);

                udpClient.Send(clientMessage, clientMessage.Length, client);
            }
        }
    }

    private static void PrintKeys<T>(Dictionary<string, T> dictionary)
    {
        foreach (string key in dictionary.Keys)
        {
            string roomLists = "";
            Console.WriteLine($"key : {key}");
        }
    }
}
