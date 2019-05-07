using System;
using ArduinoUploader;
using CommandLine;
using NLog;
using PhysLogger;

namespace ArduinoSketchUploader
{
    internal class HashProgress : Progress<double>
    {
        Action<string> action;
        public HashProgress(Action<string> handler)
        {
            action = handler;
        }
        double last = 0;
        double current = 0;
        protected override void OnReport(double value)
        {
            if (value == 1)
                ;
            if (value > last + 0.05 || value == 1)
            {
                last = value;
                current = value;
                action(ProgLine());
            }

        }

        internal string ProgLine()
        {
            string s = "[";
            for (double i = 0; i <= 1; i += 0.05D)
            {
                if (i <= current)
                    s += "#";
                else
                    s += " ";
            }
            s += "] " + Math.Round(current * 100, 2) + "%";
            return s;
        }
    }

    internal class ArduinoConsoleStyleUploader : IArduinoUploaderLogger
    {
        public event StringHandler OnProgressUpdate;
        //private static readonly Logger Logger = LogManager.GetLogger("ArduinoSketchUploader");
        
        public ArduinoConsoleStyleUploader()
        {
        }
        public void Error(string message, Exception exception)
        {
            OnProgressUpdate?.Invoke(message+"\r\n");
            OnProgressUpdate?.Invoke(exception.ToString() + "\r\n");
        }

        public void Warn(string message)
        {
            OnProgressUpdate?.Invoke(message+"\r\n");
        }

        public void Info(string message)
        {
            OnProgressUpdate?.Invoke(message+"\r\n");
        }

        public void Debug(string message)
        {
        }

        public void Trace(string message)
        {
        }
        public void ReadyToExit(string message)
        {
            OnProgressUpdate?.Invoke(message + "\r\n");
        }

        internal void Info(object p)
        {
        }
    }
    public delegate void StringHandler(string args);

    /// <summary>
    /// The ArduinoSketchUploader can upload a compiled (Intel) HEX file directly to an attached Arduino.
    /// </summary>
    public class UploaderMain
    {
        public event StringHandler OnProgressUpdate;
        public bool Upload(string file, ArduinoUploader.Hardware.ArduinoModel controller, string port)
        {
            var logger = new ArduinoConsoleStyleUploader();
            logger.OnProgressUpdate += Logger_OnProgressUpdate;
            //commandLineOptions.PortName = "COM55";
            //commandLineOptions.FileName = @"C:\Users\umar.hassan\GoogleDrive\LUMS\PhysLogger\PhysLogger\bin\Debug\firmware\PhysLoggerV1.1.hex";
            //commandLineOptions.ArduinoModel = ArduinoUploader.Hardware.ArduinoModel.Mega2560;
            //if (!Parser.Default.ParseArguments(args, commandLineOptions))
            //{ System.Windows.Forms.MessageBox.Show("No args"); return; }
            

            var options = new ArduinoSketchUploaderOptions
            {
                PortName = port,
                FileName = file,
                ArduinoModel = controller
            };

            var progress = new HashProgress(
                p => logger.Info(p));

            try
            {
                var uploader = new ArduinoUploader.ArduinoSketchUploader(options, logger, progress);
                try
                {
                    uploader.UploadSketch();
                    logger.ReadyToExit("The process completed");

                    return true;
                }
                catch (ArduinoUploaderException)
                {
                    logger.ReadyToExit("An error occured during firmware upload. Kindly retry or Kindly retry or if the problem persists, contact Qosain.");

                    return false;
                }
                catch (Exception ex)
                {
                    logger.Error($"Unexpected exception: {ex.Message}!", ex);
                    logger.ReadyToExit("An error occured during firmware upload. " + ex.Message + " Kindly retry or Kindly retry or if the problem persists, contact Qosain.");

                    return false;
                }
            }
            catch
            {
                logger.ReadyToExit("An error occured during firmware upload. Kindly retry or if the problem persists, contact Qosain.");

                return false;

            }
        }

        private void Logger_OnProgressUpdate(string args)
        {
            OnProgressUpdate?.Invoke(args);
        }

        private enum StatusCodes
        {
            Success,
            ArduinoUploaderException,
            GeneralRuntimeException
        }
    }
}