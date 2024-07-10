using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace PassGen.GlobalTool;

public sealed class OSXKeyChain
{
    private const string libSystem = "/usr/lib/libSystem.dylib";
    private const string SecurityFramework = "/System/Library/Frameworks/Security.framework/Security";
    private const string CoreFoundationFramework = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
    private const uint kCFStringEncodingUTF8 = 0x08000100;
    private const int errSecItemNotFound = -25300;

    private readonly IntPtr securityHandle;
    private readonly IntPtr kSecClass;
    private readonly IntPtr kSecClassGenericPassword;
    private readonly IntPtr kSecAttrLabel;
    private readonly IntPtr kSecAttrAccount;
    private readonly IntPtr kSecValueData;
    private readonly IntPtr kSecReturnAttributes;
    private readonly IntPtr kSecReturnData;

    private readonly IntPtr cfHandle;
    private readonly IntPtr kCFBooleanTrue;
    private readonly IntPtr kCFTypeDictionaryKeyCallBacks;
    private readonly IntPtr kCFTypeDictionaryValueCallBacks;

    private static Lazy<OSXKeyChain> _singleton = new Lazy<OSXKeyChain>(() => new OSXKeyChain(), true);

    public sealed class NotFoundException : Exception {};

    [SupportedOSPlatform(nameof(OSPlatform.OSX))]
    public static OSXKeyChain Instance => _singleton.Value;

    public unsafe (string username, string password) Query(string label)
    {
        var dict = CreateQueryDict(label);
        try {
            IntPtr data;
            var result = SecItemCopyMatching(dict, out data);
            if (result == errSecItemNotFound)
                throw new NotFoundException();
            if (result != 0)
                throw new Exception($"SecItemCopyMatching failed with {result}");
            var cfUsername = CFDictionaryGetValue(data, kSecAttrAccount);
            var cfPasswordData = CFDictionaryGetValue(data, kSecValueData);
            var cfPassword = CFStringCreateFromExternalRepresentation(IntPtr.Zero, cfPasswordData, kCFStringEncodingUTF8);
            try {
                var username = cfUsername == IntPtr.Zero ? string.Empty : GetCFString(cfUsername);
                var password = cfPassword == IntPtr.Zero ? string.Empty : GetCFString(cfPassword);
                return (username, password);
            } finally {
                CFRelease(cfPassword);
            }
        } finally {
            CFRelease(dict);
        }
    }

    private OSXKeyChain()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            throw new PlatformNotSupportedException();

        securityHandle = dlopen(SecurityFramework, 0);
        if (securityHandle == IntPtr.Zero)
            throw new Exception($"Failed to dlopen {SecurityFramework}");

        cfHandle = dlopen(CoreFoundationFramework, 0);
        if (cfHandle == IntPtr.Zero)
            throw new Exception($"Failed to dlopen {CoreFoundationFramework}");

        kSecClass = GetConstant(securityHandle, "kSecClass");
        kSecClassGenericPassword = GetConstant(securityHandle, "kSecClassGenericPassword");
        kSecAttrLabel = GetConstant(securityHandle, "kSecAttrLabel");
        kSecAttrAccount = GetConstant(securityHandle, "kSecAttrAccount");
        kSecValueData = GetConstant(securityHandle, "kSecValueData");
        kSecReturnAttributes = GetConstant(securityHandle, "kSecReturnAttributes");
        kSecReturnData = GetConstant(securityHandle, "kSecReturnData");

        kCFBooleanTrue = GetConstant(cfHandle, "kCFBooleanTrue");
        kCFTypeDictionaryKeyCallBacks = GetConstant(cfHandle, "kCFTypeDictionaryKeyCallBacks", false);
        kCFTypeDictionaryValueCallBacks = GetConstant(cfHandle, "kCFTypeDictionaryValueCallBacks", false);
    }

    private unsafe IntPtr CreateQueryDict(string label)
    {
        var cfLabel = CFStringCreateWithCharacters(IntPtr.Zero, label, (IntPtr)label.Length);
        try {
            var keys = stackalloc IntPtr[4];
            var values = stackalloc IntPtr[4];
            keys[0] = kSecClass;
            values[0] = kSecClassGenericPassword;
            keys[1] = kSecAttrLabel;
            values[1] = cfLabel;
            keys[2] = kSecReturnAttributes;
            values[2] = kCFBooleanTrue;
            keys[3] = kSecReturnData;
            values[3] = kCFBooleanTrue;
            return CFDictionaryCreate(IntPtr.Zero, keys, values, 4, kCFTypeDictionaryKeyCallBacks, kCFTypeDictionaryValueCallBacks);
        } finally {
            CFRelease(cfLabel);
        }
    }

    private static IntPtr GetConstant(IntPtr handle, string symbol, bool deref = true)
    {
        var ptr = dlsym(handle, symbol);
        if (ptr == IntPtr.Zero)
            throw new EntryPointNotFoundException(symbol);
        return deref ? Marshal.ReadIntPtr(ptr) : ptr;
    }

    private static string GetCFString(IntPtr handle)
    {
        var len = (int)CFStringGetLength(handle);
        var buf = Marshal.AllocHGlobal(len * 4 + 1);
        try {
            CFStringGetCharacters(handle, new CFRange { Location = (IntPtr)0, Length = (IntPtr)len }, buf);
            return Marshal.PtrToStringUni(buf, len);
        } finally {
            Marshal.FreeHGlobal(buf);
        }
    }

	#region CoreFoundation
    [DllImport(CoreFoundationFramework, CharSet = CharSet.Unicode)]
    private static extern IntPtr CFStringCreateWithCharacters(IntPtr/*CFAllocatorRef*/ allocator, string str, IntPtr count);

    [DllImport(CoreFoundationFramework)]
    private static unsafe extern IntPtr/*CFDictionaryRef*/ CFDictionaryCreate(IntPtr/*CFAllocatorRef*/ allocator, IntPtr* keys, IntPtr* values, long numValues, IntPtr/*const CFDictionaryKeyCallBacks**/ keyCallBacks, IntPtr/*const CFDictionaryValueCallBacks**/ valueCallBacks);

    [DllImport(CoreFoundationFramework)]
    private static extern IntPtr/*const void**/ CFDictionaryGetValue(IntPtr/*CFDictionaryRef*/ theDict, IntPtr/*const void**/ key);

    [DllImport(CoreFoundationFramework)]
    private static extern IntPtr/*CFStringRef*/ CFStringCreateFromExternalRepresentation(IntPtr/*CFAllocatorRef*/ alloc, IntPtr/*CFDataRef*/ data, uint encoding);

    [DllImport(CoreFoundationFramework)]
    private static extern IntPtr CFStringGetLength(IntPtr handle);

    [StructLayout(LayoutKind.Sequential)]
    private struct CFRange
    {
        public IntPtr Location;
        public IntPtr Length;
    }

    [DllImport(CoreFoundationFramework)]
    private static extern IntPtr CFStringGetCharacters(IntPtr handle, CFRange range, IntPtr buffer);

    [DllImport (CoreFoundationFramework)]
    private static extern void CFRelease(IntPtr obj);
    #endregion

    #region Keychain
    [DllImport(SecurityFramework)]
    private static extern int/*OSStatus*/ SecItemCopyMatching(IntPtr/*CFDictionaryRef*/ query, out IntPtr/*CFTypeRef _Nullable* */ result);
    #endregion

    #region dlfcn
    [DllImport (libSystem)]
    private static extern IntPtr dlopen(string path, int mode);

    [DllImport (libSystem)]
    private static extern IntPtr dlsym(IntPtr handle, string symbol);
    #endregion
}
