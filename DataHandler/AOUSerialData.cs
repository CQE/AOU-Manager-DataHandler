using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using System.Threading;
using System.Threading.Tasks;

namespace DataHandler
{
    public class AOUSerialData
    {
        public bool Connected { get; private set; }
 
        private SerialDevice serialPort = null;

        private CancellationTokenSource ReadCancellationTokenSource;

        private DataWriter dataWriteObject = null;
        private DataReader dataReaderObject = null;

        private string textToSend = "";
        private string receivedText = "";

        private string logstr = "";
        private string errstr = "";

        public bool IsDataAvailable()
        {
            return receivedText.Length > 0;
        }

        public bool IsError()
        {
            return errstr.Length > 0;
        }

        public string GetTextData()
        {
            string text = receivedText;
            receivedText = "";

            return text;
        }

        public string GetLogText()
        {
            string text = logstr;
            logstr = "";

            return text;
        }

        public string GetErrText()
        {
            return errstr;
        }

        public AOUSerialData(string comportSettings)
        {
            logstr = "";
            errstr = "";
            Connected = false;
            string comPort = "COM1";
            uint baudRate = 9600; // 9600, 14400, 19200, 28800, 38400, 56000, 57600, 115200...
            uint baud2 = 9600;
            if (comportSettings.Length > 3)
            { 
                string[] parameters = comportSettings.Split(',');
                if (parameters.Length > 0 && parameters[0].Length > 0)
                {
                    comPort = parameters[0].ToUpper();
                }
                if (parameters.Length > 1 && uint.TryParse(parameters[1], out baud2))
                {
                    baudRate = baud2;
                }
            }

            InitComPort(comPort, baudRate);
        }

        ~AOUSerialData()
        {
            CancelReadTask();
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
            }
        }

        private void ConfigureSerialPort(uint baudrate)
        {
            serialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            serialPort.BaudRate = baudrate;
            serialPort.Parity = SerialParity.None;
            serialPort.StopBits = SerialStopBitCount.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = SerialHandshake.None;

            logstr += "ProdId:" + serialPort.UsbProductId + ", ";
            logstr += "vendorId:" + serialPort.UsbVendorId + "\r\n";
            logstr += "Baudrate:" + serialPort.BaudRate + ", Parity:" + serialPort.Parity + ", Databits:" + serialPort.DataBits + ", Stopbits:" + serialPort.StopBits + ", No Handshake\r\n";
        }

        private async void InitComPort(string portName, uint baudRate)
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);

                for (int i = 0; i < dis.Count; i++)
                {
                    var dev = dis[i];
                    var entry = dis[i]; // DeviceInformation
                    serialPort = await SerialDevice.FromIdAsync(entry.Id);
                    if (serialPort != null && serialPort.PortName == portName)
                    {
                        logstr += "Found serial device: " + dev.Name + " on " + serialPort.PortName + "\r\n";
                        ConfigureSerialPort(baudRate);
                        break;
                    }
                }

                if (serialPort != null)
                { 
                    ReadCancellationTokenSource = new CancellationTokenSource();
                    Listen();
                    Connected = true;
                    logstr += "Listening to " + serialPort.PortName + "\r\n";
                }
                else
                {
                    logstr += "Can not connect to Serial Device " + portName + "\r\n";
                    Connected = false;
                }
            }
            catch (Exception ex)
            {
                logstr += "Connection exception: " + ex.Message + "\r\n";
            }
        }

        /**************************************************/
        // Sending Data
        /**************************************************/
        public bool SendData(string text_data)
        {
            textToSend += text_data;
            Send();
            return true;
        }

        private async Task WriteAsync()
        {
            Task<UInt32> storeAsyncTask;
            if (textToSend.Length > 0)
            {
                dataWriteObject.WriteString(textToSend);
                storeAsyncTask = dataWriteObject.StoreAsync().AsTask();
                UInt32 bytesWritten = await storeAsyncTask;
                textToSend = ""; 
            }
        }

        private async void Send()
        {
            try
            {
                if (Connected)
                {
                    dataWriteObject = new DataWriter(serialPort.OutputStream);
                    await WriteAsync();
                }
                else
                {
                    logstr += "Sending data, Not connected to serial port\r\n";
                }
            }
            catch (Exception ex)
            {
                logstr += "Error sending data: " + ex.Message + "\r\n";
            }
            finally // Cleanup
            {
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }

        /**************************************************/
        // Reading Data
        /**************************************************/
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }

        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;

            uint ReadBufferLength = 1024;

            // Throw Cancel exception if Canceled
            cancellationToken.ThrowIfCancellationRequested();

            dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
            loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                receivedText += dataReaderObject.ReadString(bytesRead);
            }
        }

        private async void Listen()
        {
            try
            {
                if (serialPort != null)
                {
                    dataReaderObject = new DataReader(serialPort.InputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(ReadCancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "TaskCanceledException")
                {
                    logstr += "Reading task was cancelled, closing device and cleaning up\r\n";
                    if (serialPort != null)
                    {
                        serialPort.Dispose();
                    }
                    serialPort = null;
                }
                else
                {
                    logstr += "Cancel error: " + ex.Message + "\r\n";
                }
            }
            finally
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }

    }
}
