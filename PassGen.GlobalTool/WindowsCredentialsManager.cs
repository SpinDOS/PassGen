# nullable enable
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace PassGen.GlobalTool;

public sealed class WindowsCredentialsManager
{
    private const string DllNameAdvapi = "Advapi32.dll";
    private const int CredentialTypeGeneric = 1;
    private const int ErrorCodeNotFound = 1168;

    private static Lazy<WindowsCredentialsManager> _singleton =
        new Lazy<WindowsCredentialsManager>(() => new WindowsCredentialsManager(), true);

    public sealed class NotFoundException : Exception {};

    [SupportedOSPlatform(nameof(OSPlatform.Windows))]
    public static WindowsCredentialsManager Instance => _singleton.Value;

    public string GetPassword(string key)
    {
        if (!CredRead(key, CredentialTypeGeneric, 0, out var credHandleRaw))
        {
            if (Marshal.GetLastWin32Error() == ErrorCodeNotFound)
                throw new NotFoundException();
            throw new Exception("Unable to Read Credential");
        }

        using var credHandle = new CredHandleWrapper(credHandleRaw);
        if (credHandle.IsInvalid)
            throw new Exception("Windows api returned invalid credentials handle");

        var credentials = Marshal.PtrToStructure<NativeCredential>(credHandle.RawHandle);
        return credentials.CredentialBlobSize >= 2
            ? Marshal.PtrToStringUni(credentials.CredentialBlob, (int)credentials.CredentialBlobSize / 2)
            : string.Empty;
    }

    private WindowsCredentialsManager() {}

    private sealed class CredHandleWrapper : CriticalHandleZeroOrMinusOneIsInvalid
    {
        public CredHandleWrapper(IntPtr handleRaw) => SetHandle(handleRaw);

        public IntPtr RawHandle => handle;

        protected override bool ReleaseHandle()
        {
            if (IsInvalid)
                return false;
            CredFree(handle);
            SetHandleAsInvalid();
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NativeCredential
    {
        public UInt32 Flags;
        public UInt32 Type;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public UInt32 CredentialBlobSize;
        public IntPtr CredentialBlob;
        public UInt32 Persist;
        public UInt32 AttributeCount;
        public IntPtr Attributes;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetAlias;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string UserName;
    }

    [DllImport(DllNameAdvapi, EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CredRead([MarshalAs(UnmanagedType.LPWStr)]string target, uint type, int reservedFlag, out IntPtr CredentialPtr);

    [DllImport(DllNameAdvapi, EntryPoint = "CredFree", SetLastError = true)]
    private static extern bool CredFree([In] IntPtr cred);
}