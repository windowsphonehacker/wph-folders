using System;
using System.Net;
using System.Windows;
using System.Runtime.InteropServices;
using System.Text;

using ComBridge = Microsoft.Phone.InteropServices.ComBridge;

namespace XapHandler
{
#if false
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct AppInfo
	{
		public String PlatformVersion;
		public String ProductId;
		public String Title;
		public String Version;
		public String Genre;
		public String Author;
		public String Description;
		[MarshalAs(UnmanagedType.ByValArray, ArraySubType=UnmanagedType.LPWStr, SizeConst=30)]
		public String[] Capabilities;
		public int CapabilityCount;
	}
#endif

	[ComImport, ClassInterface(ClassInterfaceType.None), Guid("EE3552C5-B9EF-4309-B099-23256B3D154C")]
	public class CXapHandler
	{
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("7C9699DE-77A7-4809-8EC0-ABDCBCCF8BE2")]
	public interface IXapHandler
	{

		[return: MarshalAs(UnmanagedType.Bool)]
		bool PrepareInstall (String path);
		[return: MarshalAs(UnmanagedType.Bool)]
		bool InstallApp (bool update);
		void CancelInstall ();
		[return: MarshalAs(UnmanagedType.Bool)]
		bool CheckFileAssociation (
			[Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder association,
			[Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder command);
		void SetFileAssociation ();
	}

	public static class XapDeployerInterop
	{
		internal static IXapHandler deployer;

		static XapDeployerInterop ()
		{
			uint reg = ComBridge.RegisterComDll("ComXapHandler.dll", new Guid("EE3552C5-B9EF-4309-B099-23256B3D154C"));
			if (reg != 0x0)
			{
				throw new ExternalException("Failure to register COM DLL (result: " + reg + ")");
			}
			deployer = new CXapHandler() as IXapHandler;
		}


		public static void Initialize ()
		{
			return;
		}

		public static bool ReadyIsAppInstalled (String path)
		{
			return deployer.PrepareInstall(path);
		}

		public static bool FinishInstall (bool update)
		{
			return deployer.InstallApp(update);
		}

		public static void CancelInstall ()
		{
			deployer.CancelInstall();
		}

		public static bool CheckFileAssociation (out String progID, out String command)
		{
			StringBuilder pid = new StringBuilder(), cmd = new StringBuilder();
			bool result = deployer.CheckFileAssociation(pid, cmd);
			progID = pid.ToString();
			command = cmd.ToString();
			return result;
		}

		public static void SetFileAssociation ()
		{
			deployer.SetFileAssociation();
		}
	}
}
