using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NModbus")]
[assembly: AssemblyProduct("NModbus")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyCopyright("Copyright © 2006 Scott Alexander, 2015 Dmitry Turin")]
[assembly: AssemblyDescription("NModbus is a C# implementation of the Modbus protocol. " +
           "Provides connectivity to Modbus slave compatible devices and applications. " +
           "Supports ASCII, RTU, TCP, and UDP protocols. ")]

[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(false)]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion("3.0.0.0")]
[assembly: AssemblyFileVersion("3.0.0.0")]
[assembly: AssemblyInformationalVersion("3.0.0")]

#if !SIGNED
[assembly: InternalsVisibleTo("NModbus.UnitTests")]
[assembly: InternalsVisibleTo("NModbus.IntegrationTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
#endif
