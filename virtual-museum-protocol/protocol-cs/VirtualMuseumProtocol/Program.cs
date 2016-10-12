using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Security.Principal;
using System.Security.Permissions;
using System.Diagnostics;
using System.IO;

namespace VirtualMuseumProtocol {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 1) return;
            string exePath = args[0].Replace ("/","\\");
			
			RegistryKey rootKey = Registry.ClassesRoot.CreateSubKey ("VirtualMuseum");
			rootKey.SetValue ("URL Protocol", "", RegistryValueKind.String);
			rootKey.Close();
			
			RegistryKey iconKey = Registry.ClassesRoot.CreateSubKey ("VirtualMuseum\\DefaultIcon");
			iconKey.SetValue("", exePath + ",0", RegistryValueKind.String);
			iconKey.Close();
			
			RegistryKey commandKey = Registry.ClassesRoot.CreateSubKey("VirtualMuseum\\shell\\open\\command");
			commandKey.SetValue ("", "\"" + exePath + "\" -url \"%1\"");
			commandKey.Close ();
        }
    }
}
