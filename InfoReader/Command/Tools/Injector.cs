using System;
using System.Runtime.InteropServices;
using System.Text;
using InfoReaderPlugin.Plugin.Command.CommandMethods;
using osuTools;

namespace InfoReaderPlugin.Plugin.Command.Tools
{
    internal static class Win32Functions
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr OpenProcess(
            ProcessAccessFlags processAccess,
            bool bInheritHandle,
            int processId
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr processHandle, IntPtr baseAddress, IntPtr buffer, int size,ref int  realLength);
        [DllImport("kernel32.dll")]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes, uint dwStackSize, IntPtr
                lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
            int dwSize, AllocationType dwFreeType);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr LoadLibraryA([MarshalAs(UnmanagedType.LPStr)] string lpFileName);
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr GetProcAddress(IntPtr dllHandle,[MarshalAs(UnmanagedType.LPStr)] string lpFuncName);
        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleA", SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string moduleName);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern UInt32 WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);

    }
    public class Injector
    {
        private readonly int _pid;
        private readonly string _dllName;
        private IntPtr _hProcess,_remoteMemory,_k32Handle,_loadDllPtr;
        private bool _readiedForInject;
        public Injector(int pid, string dllName)
        {
            _pid = pid;
            _dllName = dllName;
        }

        bool TryOpenProcess(int pid, out IntPtr handle)
        {
            handle = IntPtr.Zero;
            IntPtr i = Win32Functions.OpenProcess(ProcessAccessFlags.All, false, pid);
            if (i != IntPtr.Zero)
                handle = i;
            return handle != IntPtr.Zero;
        }

        bool TryGetModuleHandle(string moduleName, out IntPtr moduleHandle)
        {
            moduleHandle = IntPtr.Zero;
            IntPtr h = Win32Functions.GetModuleHandle(moduleName);
            if (h != IntPtr.Zero)
                moduleHandle = h;
            return moduleHandle != IntPtr.Zero;

        }
        bool TryGetProcAddress(IntPtr hModule, string funcName, out IntPtr funcPtr)
        {
            funcPtr = IntPtr.Zero;
            if(hModule != IntPtr.Zero)
            {
                IntPtr p = Win32Functions.GetProcAddress(hModule, funcName);
                if (p != IntPtr.Zero)
                    funcPtr = p;
                return funcPtr != IntPtr.Zero;
            }
            return false;
        }

        void ReadyForProcess()
        {
            if (!TryOpenProcess(_pid, out _hProcess))
            {
                _readiedForInject = false;
                return;
            }

            _remoteMemory = Win32Functions.VirtualAllocEx(_hProcess, IntPtr.Zero, (uint)_dllName.ToBytes(Encoding.ASCII).Length + 5,
                AllocationType.Commit, MemoryProtection.ReadWrite);
            if (_remoteMemory == IntPtr.Zero)
            {
                _readiedForInject = false;
                return;
            }

            byte[] toWriteBytes = _dllName.ToBytes(Encoding.ASCII);
            IntPtr pBuffer = Marshal.AllocHGlobal(toWriteBytes.Length + 5);
            Marshal.Copy(toWriteBytes,0,pBuffer,toWriteBytes.Length);
            int wroteSize = 0;
            bool suc = Win32Functions.WriteProcessMemory(_hProcess, _remoteMemory, pBuffer, toWriteBytes.Length,ref wroteSize);
            if (suc && wroteSize != toWriteBytes.Length)
            {
                _readiedForInject = false;
                return;
            }
            if (!TryGetModuleHandle("kernel32", out _k32Handle))
            {
                _readiedForInject = false;
                return;
            }

            if (!TryGetProcAddress(_k32Handle, "LoadLibraryA", out  _loadDllPtr))
            {
                _readiedForInject = false;
                return;
            }

            _readiedForInject = true;

        }
        public bool Inject()
        {
            ReadyForProcess();
            if (_readiedForInject)
            {
                IntPtr hThread = Win32Functions.CreateRemoteThread(_hProcess, IntPtr.Zero, 0, _loadDllPtr,
                    _remoteMemory, 0,
                    IntPtr.Zero);

                if (hThread != IntPtr.Zero)
                {
                    Win32Functions.WaitForSingleObject(hThread, 0xffffffff);
                    Clean();
                    Win32Functions.CloseHandle(hThread);
                    return true;
                }

                Clean();
                return false;
            }

            return false;
        }

        void Clean()
        {
            Win32Functions.VirtualFreeEx(_hProcess, _remoteMemory,  _dllName.ToBytes(Encoding.ASCII).Length + 5,
                AllocationType.Decommit);

        }

    }
}
