# Plc interface
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/Vectron/PlcInterface/blob/main/LICENSE.txt)
[![Build status](https://github.com/Vectron/PlcInterface/actions/workflows/BuildTestDeploy.yml/badge.svg)](https://github.com/Vectron/PlcInterface/actions)
[![NuGet](https://img.shields.io/nuget/v/PlcInterface.svg)](https://www.nuget.org/packages/PlcInterface.Abstraction)

An abstraction for communicating with PLC over different protocols.
The abstraction can be used with Microsoft.Extensions.DependencyInjection.Abstractions

Important interfaces:
    IPlcConnection: Open and close connection to the plc.
    IReadWrite: For reading and writing variables to the PLC.
    IMonitor: For monitoring variables in the PLC, and get a notification when they change.
    

# Plc interface ADS
Implementation for the TwinCAT Ads interface.
[![NuGet](https://img.shields.io/nuget/v/PlcInterface.svg)](https://www.nuget.org/packages/PlcInterface.Ads)

# Plc interface OPC
Implementation for the OPC interface.
[![NuGet](https://img.shields.io/nuget/v/PlcInterface.svg)](https://www.nuget.org/packages/PlcInterface.Opc)

## Authors
- [@Vectron](https://www.github.com/Vectron)
