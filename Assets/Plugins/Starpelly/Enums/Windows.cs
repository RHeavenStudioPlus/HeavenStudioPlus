using Starpelly.Common;
using System;
using System.Runtime.InteropServices;

namespace Starpelly.Enums.Windows
{
	public enum WindowsCursor : int
	{
		StandardArrowAndSmallHourglass = 32650,
		StandardArrow = 32512,
		Crosshair = 32515,
		Hand = 32649,
		ArrowAndQuestionMark = 32651,
		IBeam = 32513,
		/// <summary>
		/// Obsolete for applications marked version 4.0 or later. 
		/// </summary>
		[System.Obsolete]
		Icon = 32641,
		SlashedCircle = 32648,
		/// <summary>
		/// Obsolete for applications marked version 4.0 or later. Use FourPointedArrowPointingNorthSouthEastAndWest
		/// </summary>
		[System.Obsolete]
		Size = 32640,
		FourPointedArrowPointingNorthSouthEastAndWest = 32646,
		DoublePointedArrowPointingNortheastAndSouthwest = 32643,
		DoublePointedArrowPointingNorthAndSouth = 32645,
		DoublePointedArrowPointingNorthwestAndSoutheast = 32642,
		DoublePointedArrowPointingWestAndEast = 32644,
		VerticalArrow = 32516,
		Hourglass = 32514
	}

	[Flags]
	public enum SendInputEventType : uint
	{
		InputMouse,
		InputKeyboard,
		InputHardware
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MOUSEINPUT
	{
		public int dx;
		public int dy;
		public uint mouseData;
		public MouseEventFlags dwFlags;
		public uint time;
		public IntPtr dwExtraInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct KEYBOARDINPUT
	{
		public ushort wVk;
		public ushort wScan;
		public uint dwFlags;
		public uint time;
		public IntPtr dwExtraInfo;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct HARDWAREINPUT
	{
		public int uMsg;
		public short wParamL;
		public short wParamH;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct MOUSEANDKEYBOARDINPUT
	{
		[FieldOffset(0)]
		public MOUSEINPUT mi;

		[FieldOffset(0)]
		public KEYBOARDINPUT ki;

		[FieldOffset(0)]
		public HARDWAREINPUT hi;
	}

	[Flags]
	public enum MouseEventFlags : uint
	{
		MOUSEEVENT_MOVE = 0x0001,
		MOUSEEVENT_LEFTDOWN = 0x0002,
		MOUSEEVENT_LEFTUP = 0x0004,
		MOUSEEVENT_RIGHTDOWN = 0x0008,
		MOUSEEVENT_RIGHTUP = 0x0010,
		MOUSEEVENT_MIDDLEDOWN = 0x0020,
		MOUSEEVENT_MIDDLEUP = 0x0040,
		MOUSEEVENT_XDOWN = 0x0080,
		MOUSEEVENT_XUP = 0x0100,
		MOUSEEVENT_WHEEL = 0x0800,
		MOUSEEVENT_VIRTUALDESK = 0x4000,
		MOUSEEVENT_ABSOLUTE = 0x8000
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct INPUT
	{
		public SendInputEventType type;
		public MOUSEANDKEYBOARDINPUT mkhi;
	}
}