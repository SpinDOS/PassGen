# nullable enable
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace PassGen.GlobalTool;

public sealed class WindowsCredentialsManager
{
    private static Lazy<WindowsCredentialsManager> _singleton =
        new Lazy<WindowsCredentialsManager>(() => new WindowsCredentialsManager(), true);

    public sealed class NotFoundException : Exception {};

    [SupportedOSPlatform(nameof(OSPlatform.Windows))]
    public static WindowsCredentialsManager Instance => _singleton.Value;

    public string GetPassword(string key)
    {
        if (!CredRead(key, CRED_TYPE.GENERIC, 0, out var credHandleRaw))
        {
            if (Marshal.GetLastWin32Error() == ERROR_NOT_FOUND)
                throw new NotFoundException();
            throw new Exception("Unable to Read Credential");
        }

        using var credHandle = new CredHandleWrapper(credHandleRaw);
        if (credHandle.IsInvalid)
            throw new Exception("Windows api returned invalid credentials handle");

        var credentialData = Marshal.PtrToStructure<CredentialData>(credHandle.RawHandle);
        return credentialData.CredentialBlob != IntPtr.Zero && credentialData.CredentialBlobSize >= 2
            ? Marshal.PtrToStringUni(credentialData.CredentialBlob, (int)credentialData.CredentialBlobSize / 2)
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

    private const string DllNameAdvapi = "advapi32.dll";
    private const int ERROR_NOT_FOUND = 1168;

    private enum CRED_PERSIST : uint
    {
        CRED_PERSIST_SESSION = 1,
        CRED_PERSIST_LOCAL_MACHINE = 2,
        CRED_PERSIST_ENTERPRISE = 3
    }

    private enum CRED_TYPE
    {
        GENERIC = 1,
        DOMAIN_PASSWORD = 2,
        DOMAIN_CERTIFICATE = 3,
        DOMAIN_VISIBLE_PASSWORD = 4,
        MAXIMUM = 5
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct CredentialData
    {
        public uint Flags;
        public CRED_TYPE Type;
        public string TargetName;
        public string Comment;
        public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
        public uint CredentialBlobSize;
        public IntPtr CredentialBlob;
        public CRED_PERSIST Persist;
        public uint AttributeCount;
        public IntPtr Attributes;
        public string TargetAlias;
        public string UserName;
    }

    [DllImport(DllNameAdvapi, EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32 | DllImportSearchPath.AssemblyDirectory)]
    private static extern bool CredRead(string target, CRED_TYPE type, int reservedFlag, out IntPtr userCredential);

    [DllImport(DllNameAdvapi, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32 | DllImportSearchPath.AssemblyDirectory)]
    private static extern void CredFree([In] IntPtr buffer);
}