﻿
using MiniChess.Model.Enums;
using MiniChess.Model.Players;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniChess.Model.Connection
{
    public class Server 
    {
        public int Port { get; private set; }
        public string Ip { get; private set; }
        private TcpClient _Client;
        private NetworkStream _stream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private string _username;
        private string _password;
        public Server(string ip, int port, string username, string password)
        {
            Ip = ip;
            Port = port;
            _Client = new TcpClient();
            _username = username;
            _password = password;
        }
        public void StartConnection()
        {
            _Client.Connect(Ip, Port);
            _stream = _Client.GetStream();
            _reader = new StreamReader(_stream);
            _writer = new StreamWriter(_stream);
            _writer.AutoFlush = true;
            Read();
        }
        private void Send(string s)
        {
            if (!_stream.CanWrite)
            {
                throw new Exception("cant write");
            }
            _writer.Write(s + _writer.NewLine);
            _writer.Flush();
        }
        private string Read()
        {
            string result = "";
            int i = 0;
            byte[] buffer;
            buffer = new byte[4096];
            i = _stream.Read(buffer, 0, buffer.Length);
            _stream.Flush();
            result += System.Text.Encoding.UTF8.GetString(buffer);
            
            Console.WriteLine(result);
            return result;
        }
        public void Login()
        {
            Send("me " + _username + " " + _password);
            Read();
        }
        public string OfferGame(Colors Color = Colors.NONE)
        {
            if (Color != Colors.NONE)
            {
                Send("offer " + (char)Color);
            }
            else
            {
                Send("offer");
            }
            Read();
            return Read();
        }
        public string AcceptGame(int gameId, Colors Color = Colors.NONE)
        {
            if (Color != Colors.NONE)
            {
                Send("accept " + gameId + (char)Color);
            }
            else
            {
                Send("accept "+ gameId);
            }
            return Read();
        }

        public void SendMove(string p)
        {
            Send(p);
        }

        public string WaitForMove(int iteration = 0)
        {
            string str = Read();
            string move = str.Split('\n')[0];
            if (move[0] == '!')
            {
                return move.Split(' ')[1];
            }
            else if (iteration < 20)
            {
                return WaitForMove(++iteration);
            }
            else
            {
                return null;
            }
        }

        public string GetResult()
        {
            string str = Read();
            string result = str.Split('\n').FirstOrDefault(x => x[0] == '=');
            if (string.IsNullOrEmpty(result))
            {
                return GetResult();
            }
            else
            {
                return result;
            }
        }

        public Move Move(GameState state)
        {
            string s = WaitForMove();
            if (!string.IsNullOrEmpty(s))
            {
                Move m = new Move(s);
                return m;
            }
            else
            {
                return null;
            }
        }
    }
}
