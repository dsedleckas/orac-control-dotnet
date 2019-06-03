
# Osc Client application for ORAC 2.0
Osc Client for the virtual modular synth ORAC: https://github.com/TheTechnobear/Orac. Further description assumes some knowledge and experience with ORAC.

## Installation
### Building from source
You will need to build and run client from source.
Download .NET SDK for your platform (Linux/macOS/Windows) [here](https://dotnet.microsoft.com/download/dotnet-core/3.0).

Check dotnet version:
```bash
patch@patchbox:~/orac-control-dotnet $ dotnet --version
3.0.100-preview5-011568
```
Clone, build and run:
```bash
git clone https://github.com/dsedleckas/orac-control-dotnet
cd orac-control-dotnet
dotnet publish -c Release -o app ./OracControl/OracControl.csproj
cd app
dotnet OracControl.dll --urls="http://*:5000"
```
Open `localhost:5000` in browser, or remote machine ip if running on remote machine.

### Configuration
Keyboard mappings, default IPs and ports are provided by default - check `appsettings.json`. 
On-screen actions can also be disabled for cleaner look. 

## Usage
### Parameters
Parameter values can be changed by clicking and dragging up-down on the pie-control with the mouse.

### Default keyboard mappings
`q`,`w` - prev/next module
`a`,`s` - prev/next page
`i`,`j`,`o` - menu up, down, activate
