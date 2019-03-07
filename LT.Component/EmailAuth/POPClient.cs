using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LT.Component.EmailAuth
{
    /// <summary>
    /// POPClient
    /// </summary>
    public class POPClient
    {
        private const string RESPONSE_OK = "+OK";
        private TcpClient clientSocket;
        private StreamReader reader;
        private StreamWriter writer;
        private int _receiveTimeOut = 6000;
        private int _sendTimeOut = 6000;
        private int _receiveBufferSize = 4090;
        private int _sendBufferSize = 4090;
        private int _waitForResponseInterval = 200;
        private string _lastCommandResponse;
        private bool _connected = true;


        public bool Connected
        {
            get { return _connected; }
        }

        /// <summary>
        /// wait for response interval
        /// </summary>
        public int WaitForResponseInterval
        {
            get { return _waitForResponseInterval; }
            set { _waitForResponseInterval = value; }
        }


        /// <summary>
        /// Receive timeout for the connection to the SMTP server in milliseconds.
        /// The default value is 60000 milliseconds.
        /// </summary>
        public int ReceiveTimeOut
        {
            get { return _receiveTimeOut; }
            set { _receiveTimeOut = value; }
        }

        /// <summary>
        /// Send timeout for the connection to the SMTP server in milliseconds.
        /// The default value is 60000 milliseconds.
        /// </summary>
        public int SendTimeOut
        {
            get { return _sendTimeOut; }
            set { _sendTimeOut = value; }
        }

        /// <summary>
        /// Receive buffer size
        /// </summary>
        public int ReceiveBufferSize
        {
            get { return _receiveBufferSize; }
            set { _receiveBufferSize = value; }
        }

        /// <summary>
        /// Send buffer size
        /// </summary>
        public int SendBufferSize
        {
            get { return _sendBufferSize; }
            set { _sendBufferSize = value; }
        }

        private void WaitForResponse(ref StreamReader rdReader, int intInterval)
        {
            if (intInterval == 0)
            {
                intInterval = WaitForResponseInterval;
            }
            //while(rdReader.Peek()==-1 || !rdReader.BaseStream.CanRead)
            while (!rdReader.BaseStream.CanRead)
            {
                Thread.Sleep(intInterval);
            }
        }

        private void WaitForResponse(ref StreamReader rdReader)
        {
            DateTime dtStart = DateTime.Now;
            TimeSpan tsSpan;
            while (!rdReader.BaseStream.CanRead)
            {
                tsSpan = DateTime.Now.Subtract(dtStart);
                if (tsSpan.Milliseconds > _receiveTimeOut)
                {
                    break;
                }
                Thread.Sleep(_waitForResponseInterval);
            }
        }

        private void WaitForResponse(ref StreamWriter wrWriter, int intInterval)
        {
            if (intInterval == 0)
            {
                intInterval = WaitForResponseInterval;
            }
            while (!wrWriter.BaseStream.CanWrite)
            {
                Thread.Sleep(intInterval);
            }
        }

        /// <summary>
        /// Tests a string to see if it's a "+OK" string
        /// </summary>
        /// <param name="strResponse">string to examine</param>
        /// <returns>true if response is an "+OK" string</returns>
        private bool IsOkResponse(string strResponse)
        {
            return (strResponse.Substring(0, 3) == RESPONSE_OK);
        }

        /// <summary>
        /// Sends a command to the POP server.
        /// </summary>
        /// <param name="strCommand">command to send to server</param>
        /// <returns>true if server responded "+OK"</returns>
        private bool SendCommand(string strCommand)
        {
            _lastCommandResponse = "";
            try
            {
                if (writer.BaseStream.CanWrite)
                {
                    writer.WriteLine(strCommand);
                    writer.Flush();

                    WaitForResponse(ref reader);
                    _lastCommandResponse = reader.ReadLine();

                    return IsOkResponse(_lastCommandResponse);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// connect to remote server
        /// </summary>
        /// <param name="strHost">POP3 host</param>
        /// <param name="intPort">POP3 port</param>
        public bool Connect(string strHost, int intPort)
        {
            clientSocket = new TcpClient();
            clientSocket.ReceiveTimeout = _receiveTimeOut;
            clientSocket.SendTimeout = _sendTimeOut;
            clientSocket.ReceiveBufferSize = _receiveBufferSize;
            clientSocket.SendBufferSize = _sendBufferSize;

            try
            {
                clientSocket.Connect(strHost, intPort);

                reader = new StreamReader(clientSocket.GetStream(), Encoding.Default, true);
                writer = new StreamWriter(clientSocket.GetStream());
                writer.AutoFlush = true;

                WaitForResponse(ref reader, WaitForResponseInterval);

                string strResponse = reader.ReadLine();

                if (IsOkResponse(strResponse))
                {
                    _connected = true;
                    return true;
                }
                Disconnect();
                return false;
            }
            catch
            {
                Disconnect();
                return false;
            }


        }

        /// <summary>
        /// Disconnect from POP3 server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                clientSocket.ReceiveTimeout = 500;
                clientSocket.SendTimeout = 500;
                SendCommand("QUIT");
                clientSocket.ReceiveTimeout = _receiveTimeOut;
                clientSocket.SendTimeout = _sendTimeOut;
                reader.Close();
                writer.Close();
                clientSocket.GetStream().Close();
                clientSocket.Close();
            }
            catch (Exception)
            {
            }
            finally
            {
                reader = null;
                writer = null;
                clientSocket = null;
            }
        }

        /// <summary>
        /// release me
        /// </summary>
        ~POPClient()
        {
            Disconnect();
        }

        /// <summary>
        /// verify user and password
        /// </summary>
        /// <param name="strlogin">user name</param>
        /// <param name="strPassword">strPassword</param>
        public bool Authenticate(string strlogin, string strPassword)
        {
            if (AuthenticateUsingUSER(strlogin, strPassword))
            {
                return true;
            }
            if (AuthenticateUsingAPOP(strlogin, strPassword))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// verify user and password
        /// </summary>
        /// <param name="strlogin">user name</param>
        /// <param name="strPassword">password</param>
        private bool AuthenticateUsingUSER(string strlogin, string strPassword)
        {
            if (!SendCommand("USER " + strlogin))
            {
                return false;
            }

            WaitForResponse(ref writer, WaitForResponseInterval);

            if (!SendCommand("PASS " + strPassword))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// verify user and password using APOP
        /// </summary>
        /// <param name="strlogin">user name</param>
        /// <param name="strPassword">password</param>
        private bool AuthenticateUsingAPOP(string strlogin, string strPassword)
        {
            if (!SendCommand("APOP " + strlogin + " " + MyMD5.GetMD5HashHex(strPassword)))
            {
                return false;
            }
            return true;
        }
    }
}

