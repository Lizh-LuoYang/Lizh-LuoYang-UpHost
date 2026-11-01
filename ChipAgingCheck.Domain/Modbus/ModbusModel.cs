using ChipAgingCheck.Domain.Interface;
using NModbus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace ChipAgingCheck.Domain.Modbus
{
    public class ModbusModel : IModbus
    {
        private IModbusMaster _master;
        private ModbusParam _param;
        private TcpClient _tcpClient;
        private object _lock = new object();
      public ModbusModel(ModbusParam param)
        {
        
        _param = param;
        }

        public bool Connect()
        {
            if (_master != null)
            {
                return true;
            }
            try
            {
                lock (_lock)
                {
                    var client = new TcpClient();
                    client.ReceiveTimeout = _param.TimeoutMs;
                    client.SendTimeout = _param.TimeoutMs;
                    client.NoDelay = true;


                    client.Connect(_param.IP, _param.Port);

                    var factory = new ModbusFactory();
                    var master = factory.CreateMaster(client);
                    master.Transport.ReadTimeout = _param.TimeoutMs;
                    master.Transport.WriteTimeout = _param.TimeoutMs;
                    master.Transport.Retries = 0;

                    _tcpClient = client;
                    _master = master;
                }
                return true;

            }
            catch (Exception)
            {
                // 连不上不抛异常，返回 false 让上层退避重试
                return false;
            }


        }
        private void CloseNoLock()
        {
            try { _master?.Dispose(); } catch { }
            try { _tcpClient?.Dispose(); } catch { }

            _master = null;
            _tcpClient = null;
        }
        public void DisConnect()
        {
            lock (_lock)
            {
                CloseNoLock();
            }
        }

        public void Dispose() => DisConnect();

        public ushort[] ReadHoldingRegiseter(ushort startAddress, ushort count)
        {
            IModbusMaster master;
            lock (_lock)
            {

                master = _master ?? throw new InvalidOperationException("未连接，不能读寄存器");
            }
            ushort[] reg = master.ReadHoldingRegisters(_param.SlaveId, startAddress, count);
            if (reg.Length == 0)
            {
                throw new IOException("读寄存器 " + startAddress + " 没有返回数据");
            }
            if (reg.Length < count)                                                   // 回包短了 = 故障
            {
                string message = "读寄存器 " + startAddress + " 想读 " + count + " 个，只回来 " + reg.Length + " 个";
                throw new IOException(message);
            }
            return reg;
        }


    }
}
