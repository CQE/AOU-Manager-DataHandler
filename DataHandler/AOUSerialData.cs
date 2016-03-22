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
    public class AOUSerialData: AOUData
    {
        private SerialDevice serialPort = null;

        private CancellationTokenSource ReadCancellationTokenSource;

        private DataWriter dataWriteObject = null;
        private DataReader dataReaderObject = null;

        private string textToSend = "";
        private string receivedText = "";

        private AOUSettings.SerialSetting setting;

        public AOUSerialData(AOUSettings.SerialSetting serialSetting) : base()
        {
            setting = serialSetting;
        }

        ~AOUSerialData()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
                serialPort = null;
            }
        }

        public override void Connect()
        {
            base.Connect();
            InitComPort(setting.ComPort, setting.BaudRate);
        }

        public override void Disconnect()
        {
            base.Disconnect();
            CancelReadTask();
        }

        public override bool SendData(string data)
        {
            textToSend += data;
            Send();
            return true;
        }

        public override void UpdateData()
        {
            base.GetTextDataList();
        }

        protected override string GetTextData()
        {
            string text = receivedText;
            receivedText = "";
            return text;
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

            AddDataLogText("ProdId:" + serialPort.UsbProductId + ", " + "vendorId:" + serialPort.UsbVendorId);
            AddDataLogText("Baudrate:" + serialPort.BaudRate + ", Parity:" + serialPort.Parity + ", Databits:" + serialPort.DataBits + ", Stopbits:" + serialPort.StopBits + ", No Handshake");
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
                        AddDataLogText("Found serial device: " + dev.Name + " on " + serialPort.PortName);
                        ConfigureSerialPort(baudRate);
                        break;
                    }
                }

                if (serialPort != null)
                { 
                    ReadCancellationTokenSource = new CancellationTokenSource();
                    Listen();
                    Connected = true;
                    AddDataLogText("Listening to " + serialPort.PortName);
                }
                else
                {
                    AddDataLogText("Can not connect to Serial Device " + portName);
                    Connected = false;
                }
            }
            catch (Exception ex)
            {
                AddDataLogText("Connection exception: " + ex.Message);
            }
        }

        /**************************************************/
        // Sending Data
        /**************************************************/

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
                    AddDataLogText("Sending data, Not connected to serial port");
                }
            }
            catch (Exception ex)
            {
                AddDataLogText("Error sending data: " + ex.Message);
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
                    AddDataLogText("Reading task was cancelled, closing device and cleaning up");
                    if (serialPort != null)
                    {
                        serialPort.Dispose();
                    }
                    serialPort = null;
                }
                else
                {
                    AddDataLogText("Cancel error: " + ex.Message);
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
