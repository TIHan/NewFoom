module internal Foom.Win32Internal

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop

(*
 * Window Messages
 *)

let WM_NULL                         = 0x0000
let WM_CREATE                       = 0x0001
let WM_DESTROY                      = 0x0002
let WM_MOVE                         = 0x0003
let WM_SIZE                         = 0x0005
let WM_ACTIVATE                     = 0x0006
(*
 * WM_ACTIVATE state values
 *)
let     WA_INACTIVE     = 0
let     WA_ACTIVE       = 1
let     WA_CLICKACTIVE  = 2

let WM_SETFOCUS                    = 0x0007
let WM_KILLFOCUS                   = 0x0008
let WM_ENABLE                      = 0x000A
let WM_SETREDRAW                   = 0x000B
let WM_SETTEXT                     = 0x000C
let WM_GETTEXT                     = 0x000D
let WM_GETTEXTLENGTH               = 0x000E
let WM_PAINT                       = 0x000F
let WM_CLOSE                       = 0x0010
let WM_QUIT                        = 0x0012
let WM_ERASEBKGND                  = 0x0014
let WM_SYSCOLORCHANGE              = 0x0015
let WM_SHOWWINDOW                  = 0x0018
let WM_WININICHANGE                = 0x001A
let WM_DEVMODECHANGE               = 0x001B
let WM_ACTIVATEAPP                 = 0x001C
let WM_FONTCHANGE                  = 0x001D
let WM_TIMECHANGE                  = 0x001E
let WM_CANCELMODE                  = 0x001F
let WM_SETCURSOR                   = 0x0020
let WM_MOUSEACTIVATE               = 0x0021
let WM_CHILDACTIVATE               = 0x0022
let WM_QUEUESYNC                   = 0x0023
let WM_GETMINMAXINFO               = 0x0024
let WM_CHAR                        = 0x0102
let WM_KEYDOWN                     = 0x0100
let WM_KEYUP                       = 0x0101
let WM_SYSKEYDOWN                  = 0x0104
let WM_SYSKEYUP                    = 0x0105
let WM_SYSCOMMAND                  = 0x0112

// Window Styles

let WS_OVERLAPPED       = 0x00000000u
let WS_POPUP            = 0x80000000u
let WS_CHILD            = 0x40000000u
let WS_MINIMIZE         = 0x20000000u
let WS_VISIBLE          = 0x10000000u
let WS_DISABLED         = 0x08000000u
let WS_CLIPSIBLINGS     = 0x04000000u
let WS_CLIPCHILDREN     = 0x02000000u
let WS_MAXIMIZE         = 0x01000000u
let WS_CAPTION          = 0x00C00000u   (* WS_BORDER | WS_DLGFRAME  *)
let WS_BORDER           = 0x00800000u
let WS_DLGFRAME         = 0x00400000u
let WS_VSCROLL          = 0x00200000u
let WS_HSCROLL          = 0x00100000u
let WS_SYSMENU          = 0x00080000u
let WS_THICKFRAME       = 0x00040000u
let WS_GROUP            = 0x00020000u
let WS_TABSTOP          = 0x00010000u
let WS_MINIMIZEBOX      = 0x00020000u
let WS_MAXIMIZEBOX      = 0x00010000u

let WS_OVERLAPPEDWINDOW = 
    WS_OVERLAPPED |||
    WS_CAPTION |||
    WS_SYSMENU |||
    WS_THICKFRAME |||
    WS_MINIMIZEBOX |||
    WS_MAXIMIZEBOX

let SC_CLOSE = 0xF060
let SC_KEYMENU = 0xF100

let PM_REMOVE = 0x0001u

let NULL = IntPtr.Zero

let WM_MOUSEMOVE = 0x0200
let WM_LBUTTONDOWN = 0x0201
let WM_LBUTTONUP = 0x0202
let WM_MBUTTONDOWN = 0x0207
let WM_MBUTTONUP = 0x0208
let WM_RBUTTONDOWN = 0x0204
let WM_RBUTTONUP = 0x0205

let MK_LBUTTON = 0x0001
let MK_MBUTTON = 0x0010
let MK_RBUTTON = 0x0002
    
type DWORD = uint32
type LPCWSTR = nativeptr<char>
type HWND = nativeint
type HMENU = nativeint
type HINSTANCE = nativeint
type LPVOID = nativeint
type UINT = uint32
type WNDPROC = nativeint
type HICON = nativeint
type HCURSOR = nativeint
type HBRUSH = nativeint
type ATOM = int
type BOOL = byte
type WPARAM = UINT
type LPARAM = nativeint
type LONG = uint64
type LRESULT = nativeint
type HANDLE = nativeint

[<Struct>]
type WNDCLASSEXW =

    val mutable cbSize : UINT
    val mutable style : UINT
    val mutable lpfnWndProc : WNDPROC
    val mutable cbClsExtra : int
    val mutable cbWndExtra : int
    val mutable hInstance : HINSTANCE
    val mutable hIcon : HICON
    val mutable hCursor : HCURSOR
    val mutable hbrBackground : HBRUSH
    val mutable lpszMenuName : LPCWSTR
    val mutable lpszClassName : LPCWSTR
    val mutable hIconSm : HICON

[<DllImport("user32.dll")>]
extern ATOM RegisterClassExW(WNDCLASSEXW& wndClassEx)

[<DllImport("user32.dll")>]
extern nativeint CreateWindowExW(
    DWORD dwExStyle,
    LPCWSTR lpClassName,
    char* lpWindowName,
    DWORD dwStyle,
    int X,
    int Y,
    int nWidth,
    int nHeight,
    HWND hWndParent,
    HMENU hMenu,
    HINSTANCE hInstance,
    LPVOID lpParam
)

[<Struct>]
type POINT =

    val mutable x : uint32
    val mutable y : uint32
    val mutable _x: uint32
    val mutable _y: uint32

type LPPOINT = nativeptr<POINT>

[<Struct>]
type MSG =

    val mutable hwnd : HWND
    val mutable message : UINT
    val mutable wParam : WPARAM
    val mutable lParam : LPARAM
    val mutable time : DWORD
    val mutable pt : POINT

type LPMSG = nativeptr<MSG>

[<Struct>]
type RECT =
    val mutable left: uint32
    val mutable top: uint32
    val mutable right: uint32
    val mutable bottom: uint32

[<DllImport("user32.dll")>]
extern BOOL ShowWindow(HWND hWnd, int nCmdShow)

[<DllImport("user32.dll")>]
extern BOOL UpdateWindow(HWND hWnd)

[<DllImport("user32.dll")>]
extern BOOL CloseWindow(HWND hWnd)

[<DllImport("user32.dll")>]
extern HCURSOR LoadCursor(HINSTANCE hInstance, int lpCursorName)

[<DllImport("kernel32.dll")>]
extern UINT GetLastError();

[<DllImport("user32.dll")>]
extern nativeint DefWindowProc(nativeint hWnd, uint32 msg, nativeint wParam, nativeint lParam)

[<DllImport("user32.dll")>]
extern BOOL GetMessage(MSG* lpMsg, HWND hWnd, UINT wMsgFilterMin, UINT uMsgFilterMax)

[<DllImport("user32.dll")>]
extern BOOL TranslateMessage(MSG* lpMsg)

[<DllImport("user32.dll")>]
extern LRESULT DispatchMessage(MSG* lpmsg)

[<DllImport("user32.dll")>]
extern BOOL PeekMessage(LPMSG lpMsg, HWND hWnd, UINT wMsgFilterMin, UINT wMsgFilterMax, UINT wRemoveMsg)

[<DllImport("kernel32.dll")>]
extern DWORD WaitForSingleObjectEx(HANDLE hHandle, DWORD dwMilliseconds, BOOL bAlertable)

[<Struct>]
type SECURITY_ATTRIBUTES =

    val mutable nLength : DWORD
    val mutable lpSecurityDescriptor : LPVOID
    val mutable bInheritHandle : BOOL

type LPSECURITY_ATTRIBUTES = nativeptr<SECURITY_ATTRIBUTES>

[<DllImport("kernel32.dll")>]
extern HANDLE CreateEventW(LPSECURITY_ATTRIBUTES lpEventAttributes, BOOL bManualReset, BOOL bInitialState, LPCWSTR lpName)

[<DllImport("kernel32.dll")>]
extern nativeint GetConsoleWindow()

[<UnmanagedFunctionPointer(CallingConvention.Cdecl)>]
type WndProcDelegate = delegate of HWND * UINT * nativeint * nativeint -> nativeint

[<DllImport("user32.dll")>]
extern BOOL GetCursorPos(LPPOINT lpPoint)

[<DllImport("user32.dll")>]
extern BOOL ScreenToClient(HWND hWnd, LPPOINT lpPoint)

[<DllImport("user32.dll")>]
extern BOOL ClipCursor(RECT* lpRect)

[<DllImport("user32.dll")>]
extern int ShowCursor(BOOL bShow)

[<DllImport("user32.dll")>]
extern BOOL GetWindowRect(HWND hWnd, RECT* lpRect)

[<DllImport("user32.dll")>]
extern BOOL SetCursorPos(int x, int y)

[<DllImport("user32.dll")>]
extern nativeint SetWindowPos(HWND hWnd, HWND hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);