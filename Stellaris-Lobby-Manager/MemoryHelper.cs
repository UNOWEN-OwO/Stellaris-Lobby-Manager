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
            ReadProcessMemory(process.Handle, address, buffer, buffer.Length, out bytesRead);

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
            ReadProcessMemory(process.Handle, address, buffer, buffer.Length, out bytesRead);

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

            ReadProcessMemory(process.Handle, address, buffer, buffer.Length, out _);

            return Encoding.UTF8.GetString(buffer).Split('\0')[0];
        }

        public static byte[] ReadUnmanaged(Process process, IntPtr address, int size)
        {
            byte[] buffer = new byte[size];

            IntPtr bytesRead;
            ReadProcessMemory(process.Handle, address, buffer, buffer.Length, out bytesRead);

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
        
            return WriteProcessMemory(process.Handle, address, buffer, buffer.Length, out _);
        }

        public static bool WriteUnmanaged(Process process, IntPtr address, byte[] value)
        {
            return WriteProcessMemory(process.Handle, address, value, value.Length, out _);
        }


        public static IntPtr GetModuleBaseAddress(Process process, string moduleName)
        {
            ProcessModule? module = process.Modules.Cast<ProcessModule>().FirstOrDefault(m => m.ModuleName == moduleName);
            return module == null ? IntPtr.Zero : module.BaseAddress;
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out IntPtr lpNumberOfBytesWritten);


        [DllImport("kernel32.dll")]
        static extern uint GetLastError();
    }
}
