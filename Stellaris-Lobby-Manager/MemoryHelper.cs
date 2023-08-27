using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris_Lobby_Manager
{
    internal class MemoryHelper
    {
        public static T Read<T>(Process process, IntPtr address) where T : unmanaged
        {
            Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);
            byte[] buffer = new byte[Marshal.SizeOf(outputType)];

            IntPtr bytesRead;
            ReadProcessMemory(process.Handle, address, buffer, buffer.Length, 0);

            T result;
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), outputType);
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public static object Read(Process process, IntPtr address, Type type)
        {
            byte[] buffer = new byte[Marshal.SizeOf(type)];

            IntPtr bytesRead;
            ReadProcessMemory(process.Handle, address, buffer, buffer.Length, 0);

            object result;
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                result = Convert.ChangeType(Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type), type);
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public static string ReadString(Process process, IntPtr address)
        {
            byte[] buffer = new byte[256];

            ReadProcessMemory(process.Handle, address, buffer, buffer.Length, 0);

            return Encoding.UTF8.GetString(buffer).Split('\0')[0];
        }

        public static byte[] ReadUnmanaged(Process process, IntPtr address, int size)
        {
            byte[] buffer = new byte[size];

            ReadProcessMemory(process.Handle, address, buffer, buffer.Length, 0);

            return buffer;
        }

        // public static bool Write<T>(Process process, IntPtr address, T value) where T : struct
        public static bool Write(Process? process, IntPtr address, object value)
        {
            if (process == null)
                return false;

            Type outputType = value.GetType();
            byte[] buffer = new byte[Marshal.SizeOf(outputType)];
        
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);
            }
            finally
            {
                handle.Free();
            }
        
            return WriteProcessMemory(process.Handle, address, buffer, buffer.Length, 0);
        }

        public static bool WriteUnmanaged(Process process, IntPtr address, byte[] value)
        {
            return WriteProcessMemory(process.Handle, address, value, value.Length, 0);
        }


        public static IntPtr GetModuleBaseAddress(Process process, string moduleName)
        {
            ProcessModule? module = process.Modules.Cast<ProcessModule>().FirstOrDefault(m => m.ModuleName == moduleName);
            return module == null ? IntPtr.Zero : module.BaseAddress;
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);


        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        protected static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

        [StructLayout(LayoutKind.Sequential)]
        protected struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public uint RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }
        List<MEMORY_BASIC_INFORMATION> MemReg { get; set; }

        public void MemInfo(IntPtr pHandle)
        {
            IntPtr Addy = new IntPtr();
            while (true)
            {
                MEMORY_BASIC_INFORMATION MemInfo = new MEMORY_BASIC_INFORMATION();
                int MemDump = VirtualQueryEx(pHandle, Addy, out MemInfo, Marshal.SizeOf(MemInfo));
                if (MemDump == 0) break;
                if ((MemInfo.State & 0x1000) != 0 && (MemInfo.Protect & 0x100) == 0)
                    MemReg.Add(MemInfo);
                Addy = new IntPtr(MemInfo.BaseAddress.ToInt32() + MemInfo.RegionSize);
            }
        }
        public IntPtr _Scan(byte[] sIn, byte[] sFor)
        {
            int[] sBytes = new int[256]; int Pool = 0;
            int End = sFor.Length - 1;
            for (int i = 0; i < 256; i++)
                sBytes[i] = sFor.Length;
            for (int i = 0; i < End; i++)
                sBytes[sFor[i]] = End - i;
            while (Pool <= sIn.Length - sFor.Length)
            {
                for (int i = End; sIn[Pool + i] == sFor[i]; i--)
                    if (i == 0) return new IntPtr(Pool);
                Pool += sBytes[sIn[Pool + End]];
            }
            return IntPtr.Zero;
        }
        public IntPtr AobScan(string ProcessName, byte[] Pattern)
        {
            Process[] P = Process.GetProcessesByName(ProcessName);
            if (P.Length == 0) return IntPtr.Zero;
            MemReg = new List<MEMORY_BASIC_INFORMATION>();
            MemInfo(P[0].Handle);
            for (int i = 0; i < MemReg.Count; i++)
            {
                byte[] buff = new byte[MemReg[i].RegionSize];
                ReadProcessMemory(P[0].Handle, MemReg[i].BaseAddress, buff, (int)MemReg[i].RegionSize, 0);

                IntPtr Result = _Scan(buff, Pattern);
                if (Result != IntPtr.Zero)
                    return new IntPtr(MemReg[i].BaseAddress.ToInt32() + Result.ToInt32());
            }
            return IntPtr.Zero;
        }

    }
}
