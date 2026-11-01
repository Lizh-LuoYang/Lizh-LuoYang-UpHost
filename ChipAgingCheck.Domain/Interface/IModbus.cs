using ChipAgingCheck.Domain.Modbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipAgingCheck.Domain.Interface
{
    public interface IModbus
    {
        bool Connect();
        void DisConnect();
        ushort[] ReadHoldingRegiseter(ushort startAddress, ushort count);
        void Dispose();

    }
}
