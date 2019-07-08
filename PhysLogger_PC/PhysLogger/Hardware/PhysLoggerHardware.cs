using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO.Ports;
using FivePointNine.Windows.IO;
using PhysLogger.LogControls;

namespace PhysLogger.Hardware
{
    public delegate void SetPointHandler(float x, float [] y, PlotLabel [] labels);
    public delegate void GainChangedHandler(int index, int gain);
    public delegate void SendCommandRequest(PacketCommandMini pc);
    public delegate void SignatureHandler(PhysLoggerHWSignature oldSignature, PhysLoggerHWSignature newSignature);
    public class LoggerHardware
    {
        protected virtual int[] ChannelDataSourceIndices { get; set; }
        public event EventHandler OnDisconnectRequested;
        protected virtual List<Maths.Function> ChannelCalibrationData { get; set; } = new List<Maths.Function>();
        protected List<ChannelOption> ChannelOptions;
        public event SignatureHandler OnSignatureUpdate;
        public event SendCommandRequest OnCommandSendRequest;
        public event valueChangeHandler OnSamplingRateChanged;
        public PhysLogger.Forms.PhysLoggerUpdaterConsole LConsole = new Forms.PhysLoggerUpdaterConsole();
        string messageTBC = "";
        public PhysLoggerHWSignature Signature { get; set; } = PhysLoggerHWSignature.Unknown;
        public virtual void HandleDataPacket(PacketCommandMini command, ref float f, ref float[] values, ref PlotLabel[] labels)
        { new NotImplementedException(); }
        public virtual void HandleMiscCommand(PacketCommandMini command)
        { new NotImplementedException(); }
        public virtual event SetPointHandler NewPointReceived;
        public void ParseCommand(PacketCommandMini command, SerialDataChannel channel)
        {
            if (command.PacketID == PhysLoggerPacketCommandID.GetHWSignatures)
            {
                PhysLoggerHWSignature newSignature = PhysLoggerHWSignature.Unknown;
                if (Signature != PhysLoggerHWSignature.Unknown)
                {
                    OnSignatureUpdate(Signature, newSignature);
                    return;
                }
                else
                {
                    var parts = command.PayLoadString.Split(new char[] { '=' });
                    if (parts[0] == "ver")
                    {
                        if (parts[1] == PhysLoggerHWSignature.PhysLogger1_0.ToString())
                        {
                            newSignature = PhysLoggerHWSignature.PhysLogger1_0;
                            new PacketCommandMini(PhysLoggerPacketCommandID.BeginFire, new byte[] { 1 }).SendCommand(channel);
                        }
                        else if (parts[1] == PhysLoggerHWSignature.PhysLogger1_1.ToString())
                        {
                            newSignature = PhysLoggerHWSignature.PhysLogger1_1;
                            new PacketCommandMini(PhysLoggerPacketCommandID.GetI2CInstruments, new byte[] { 1 }).SendCommand(channel);
                            new PacketCommandMini(PhysLoggerPacketCommandID.BeginFire, new byte[] { 1 }).SendCommand(channel);
                        }
                        else if (parts[1] == PhysLoggerHWSignature.PhysLogger1_2.ToString())
                        {
                            newSignature = PhysLoggerHWSignature.PhysLogger1_2;
                            new PacketCommandMini(PhysLoggerPacketCommandID.GetI2CInstruments, new byte[] { 1 }).SendCommand(channel);
                            new PacketCommandMini(PhysLoggerPacketCommandID.BeginFire, new byte[] { 1 }).SendCommand(channel);
                        }
                        else
                            return;
                        OnSignatureUpdate(Signature, newSignature);
                        return;
                    }
                }
            }
            if (Signature == PhysLoggerHWSignature.Unknown)
            {
                new PacketCommandMini(PhysLoggerPacketCommandID.GetHWSignatures).SendCommand(channel);
                return;
            }
            if (command.PacketID == PhysLoggerPacketCommandID.DataPacket)
            {
                float t = 0;
                float[] values = null;
                PlotLabel[] labels = null;
                HandleDataPacket(command, ref t, ref values, ref labels);
                if (values != null)
                    NewPointReceived(t, values, labels);
            }
            else if (command.PacketID == PhysLoggerPacketCommandID.i2cInstrumentSignature)
            {
                AddI2CInstrument(command.PayLoad[0], command.PayLoad[1]);
            }
            else if (command.PacketID == PhysLoggerPacketCommandID.ChangeSamplingTime)
            {
                var ts = BitConverter.ToUInt32(command.PayLoad, 0);
                OnSamplingRateChanged.Invoke(1 / (float)ts * 1000);
            }
            else
            {
                if (command.PacketID == PhysLoggerPacketCommandID.GetValue)
                {
                    if (command.PayLoad[0] == 0) // get float TBC
                    {
                        messageTBC += Encoding.UTF8.GetString(command.PayLoad, 1, command.PayLoadLength - 1);
                        return;
                    }
                    else if (command.PayLoad[0] == 1) // get float
                    {
                        string m = messageTBC + Encoding.UTF8.GetString(command.PayLoad, 1, command.PayLoadLength - 1);
                        float v = 0;
                        try
                        {
                            v = GetValueFromUser(m);
                        }
                        catch
                        { return; }
                        var com = new PacketCommandMini(PhysLoggerPacketCommandID.GetValue);
                        com.PayLoad = Encoding.UTF8.GetBytes(v.ToString());
                        CommandSendRequest(com);
                        messageTBC = "";
                        return;
                    }
                    else if (command.PayLoad[0] == 2) // get Console Key
                    {
                        if (!LConsole.IsActive)
                        {
                            try
                            {
                                LConsole.Show();
                            }
                            catch
                            {
                                LConsole = new Forms.PhysLoggerUpdaterConsole();
                                LConsole.Show();
                            }
                        }
                        LConsole.WriteLine(Encoding.UTF8.GetString(command.PayLoad, 1, command.PayLoadLength - 1));
                        string s = LConsole.ReadKey();
                        var com = new PacketCommandMini(PhysLoggerPacketCommandID.GetValue);
                        com.PayLoad = Encoding.UTF8.GetBytes(s);
                        if (s == "")
                            com.PayLoad = new byte[] { 0 };
                        CommandSendRequest(com);
                        return;
                    }
                    else
                    {
                        HandleMiscCommand(command);
                    }
                }
                if (command.PacketID == PhysLoggerPacketCommandID.SetValue)
                {
                    if (command.PayLoad[0] == 1)
                    {
                        HandleIncomingUserMessage(messageTBC + Encoding.UTF8.GetString(command.PayLoad, 1, command.PayLoadLength - 1), false);
                        messageTBC = "";
                    }
                    else if (command.PayLoad[0] == 2) // force console print
                    {
                        HandleIncomingUserMessage(messageTBC + Encoding.UTF8.GetString(command.PayLoad, 1, command.PayLoadLength - 1), true);
                        messageTBC = "";
                    }
                    else if (command.PayLoad[0] == 3) // force console println
                    {
                        HandleIncomingUserMessage(messageTBC + Encoding.UTF8.GetString(command.PayLoad, 1, command.PayLoadLength - 1) + "\r\n", true);
                        messageTBC = "";
                    }
                    else if (command.PayLoad[0] == 4) // ConsolMessageTBC
                    {
                        messageTBC += Encoding.UTF8.GetString(command.PayLoad, 1, command.PayLoadLength - 1);
                        return;
                    }
                    else if (command.PayLoad[0] == 5) // console may exit
                    {
                        if (LConsole.IsActive)
                            LConsole.SafeExit();
                        return;
                    }
                    else if (command.PayLoad[0] == 6) // Disconnect Request
                    {
                        RequestDisconnect();
                        return;
                    }
                    else
                        HandleMiscCommand(command);
                }
                HandleMiscCommand(command);
            }
        }
        public virtual void AddI2CInstrument(int instrumentID, int i2cAddress)
        { throw new NotImplementedException(); }
        public virtual void ChangeSamplingRate(float frequencyInHz)
        {
            throw new NotImplementedException();
        }
        public virtual float GetValueFromUser(string message)
        {
            if (LConsole.IsActive)
            {
                LConsole.Write(message);
                var read = LConsole.ReadLine();
                if (read.ToLower() == "exit" || read.ToLower() == "quit")
                {
                    LConsole.SafeExit();
                    throw new Exception();
                }
                try
                {
                    return Convert.ToSingle(read);
                }
                catch { }
                return 0;
            }
            else
            {
                var askF = PhysLogger.Forms.AskFloat.ShowDialog(message, 0);
                if (askF.dr == System.Windows.Forms.DialogResult.OK)
                    return askF.Value;
                else
                    return 0;
            }
        }
        public virtual void HandleIncomingUserMessage(string message, bool forceConsole)
        {
            if (forceConsole)
            {
                if (!LConsole.IsActive)
                    LConsole.Show();
            }
            if (LConsole.IsActive)
                LConsole.Write(message);
            else
                System.Windows.Forms.MessageBox.Show(message);
        }
        internal virtual ChannelOption UpdateChannelOptions(ChannelOption[] allOptions, int id)
        {
            throw new NotImplementedException();
        }
        public virtual List<ChannelOption> EnumerateChannelOptions()
        {
            throw new NotImplementedException();
        }
        public List<float> SupportedFrequencies { get; protected set; }

        protected virtual void SignatureUpdates(PhysLoggerHWSignature oldSignature, PhysLoggerHWSignature newSignature)
        {
            OnSignatureUpdate?.Invoke(oldSignature, newSignature);
        }
        protected virtual void SamplingRateChanged(float value)
        {
            OnSamplingRateChanged?.Invoke(value);
        }
        protected virtual void CommandSendRequest(PacketCommandMini com)
        {
            OnCommandSendRequest?.Invoke(com);
        }

        protected void RequestDisconnect()
        {
            OnDisconnectRequested?.Invoke(this, null);
        }
        public virtual void UpdateFirmware(PhysLogger.Forms.PhysLoggerUpdaterConsole ucf, string comPort, string forcedSource)
        {
            if (forcedSource == "" || forcedSource == null)
                throw new NotImplementedException();
            RequestDisconnect();
            ucf.WriteLine("PhysLogger Update Console");
            string fNameSeed = "";
            string verString = "";
            fNameSeed = System.IO.Path.Combine("firmware", System.IO.Path.GetFileNameWithoutExtension(forcedSource));
            verString = System.IO.Path.GetFileNameWithoutExtension(fNameSeed);
            if (fNameSeed != "")
            {
                if (System.IO.File.Exists(forcedSource))
                {
                    ucf.WriteLine();
                    ucf.Write("You are about to update the PhysLogger firmare to ");
                    ucf.Write(verString);
                    ucf.WriteLine(". This will update the firmare on the PhysLogger hardware which might raise conflicts if not done properly.");
                    ucf.WriteLine("If you are not sure what you are doing, you might want to quit it now.\r\n");
                    if (System.IO.File.Exists(fNameSeed + "_preInstall.txt"))
                    {
                        ucf.WriteLine();
                        ucf.Write(System.IO.File.ReadAllText(fNameSeed + "_preInstall.txt"));
                        ucf.WriteLine();
                    }
                    ucf.Write("\r\nDo you still want to continue with the update (Y/n)? ");
                    if (ucf.ReadLine() != "Y")
                    {
                        return;
                    }
                    string root = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\avrdude";


                    ArduinoSketchUploader.UploaderMain uploader = new ArduinoSketchUploader.UploaderMain();
                    uploader.OnProgressUpdate += delegate (string a)
                    {
                        ucf.Write(a);
                    }
                    ;
                    for (int i = 1; i <= 3; i++)
                    {
                        if (uploader.Upload(fNameSeed + ".hex", ArduinoUploader.Hardware.ArduinoModel.Mega2560, comPort))
                            break;
                        ucf.WriteLine("Upload failed. Retrying (" + i + "/3)...");
                        System.Threading.Thread.Sleep(3000);
                    }
                    return;
                    //p.OutputDataReceived += (s, ea) => ucf.Write(ea.Data);
                }
            }
            ucf.WriteLine("The firmware update process has failed.");
            ucf.SafeExit();
        }

        public virtual void SignatureReceived(PhysLoggerHWSignature newSignature)
        {
            throw new NotImplementedException();
        }
    }
    public class PhysLoggerPacketCommandID : FivePointNine.Windows.IO.PacketCommandID
    {
        //#define GetHWSignatures 1  // IO
        //#define ChangeChannelType 2 // I
        //#define ChangeChannelGain 3 // I
        //#define DataPacket 2 // O
        //#define BeginFire 4 //I
        //#define ChangeSamplingTime 5 // IO
        //#define GetI2CInstruments 6 // I
        //#define i2cInstrumentSignature 3 // O
        //#define ShowWait 4	// O
        //#define EndWait 6 // O
        public const byte GetHWSignatures = 1;
        public const byte ChangeChannelType = 2;
        public const byte ChangeChannelRange = 3;
        public const byte DataPacket = 2;
        public const byte BeginFire = 4;
        public const byte ChangeSamplingTime = 5;
        public const byte GetI2CInstruments = 6;
        public const byte i2cInstrumentSignature = 3;
        public const byte SetValue = 4;
        public const byte GetValue = 7;
    }
    public enum PhysLoggerHWSignature
    {
        Unknown,
        PhysLogger1_0,
        PhysLogger1_1,
        PhysLogger1_0_Virtual,
        PhysLogger1_2
    }
}
