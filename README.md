# Easy-Volume-Control
A simple command line based volume controller that uses C# to modify audio source volumes and retrieve details while also providing a service to use with external programs

# Usage

## Parameters:
### Basics:
| Option      | Description |
| ----------- | ----------- |
| `-device`   |Retrieve data on current default device|
| `-sources`   |Retrieve and List all current audio sources the corresponding data|  
| `-setVol ProcessID,Volume`   |Set volume for given process ID and volume|  
| `-setMaster Volume`   | Set volume for current default device to given volume |  
| `-setMute ProcessID,State`   | Set mute for given process ID with state `0` or `1` [True/False]         |  
| `-setMuteMaster State`   | Set mute for current default device with state `0` or `1` [True/False]       |  

### Output Modifiers:
| Option      | Description |
| ----------- | ----------- |
| `-silent / -q`   |Prevent Warning and Success outputs (Only display data)|
| `-clear`   |Clear any compile time warnings from console|   
| `-json`   |Output all data in json format for easy read and use in external programs|

## Volume Control Service
The volume control service is meant to make volume control in external programs alot simpler and easier allowing developers to design their own volume control software with the help of this service.

### Getting Started:
| Option      | Description |
| ----------- | ----------- |
| `-startService`   |Start service to create a callback.json file and start running|

In order to use the service you will need to first run the exe with the `-startService` command  
```cmd 
volume -startService
```
OR for JSON output
```cmd 
volume -startService -silent -json
```
Once the service has started running you will recieve a output saying its running or if started with `-json` a JSON object `{"service":true}`  
This will also create a file called `callbacks.json` in the same folder which follows this structure
```json
{
    "serviceID": 12345,
    "stop": false,
    "fade": false,
    "calls": "",
    "device": {
        "name": "device name",
        "type": "device type",
        "interface": "interface",
        "device": 1,
        "icon": 3,
        "iconPath": "path/to/icon/.dll/file",
        "id": "id",
        "realID": "{real}.{id}",
        "isMuted": false,
        "volume": 100,
        "state": 1
    },
    "sources": [
        {
            "name": "Source Name",
            "device": "Source Device",
            "processID": 1234,
            "isSystem": false,
            "isMuted": false,
            "state": 1,
            "volume": 100,
            "iconPath": "path/to/icon (If available)",
            "exePath": "path/to/exe",
            "iconBytes": "string base64 icon bytes"
        },
    ]
}
```
### Setting audio source attributes:
This allows for easy access to device and source details as they will be constantly updated, on top of that you can issue set commands using the `calls` attribute.
For example settings `calls` to `"-setVol 1234,50"` will set the source with process ID `1234` volume to `50` and then reset calls to a empty string.

### Stopping service:
By setting the attribute `stop` to `true` will tell the service to shutdown, right before the shutdown `stop` will be reset back to `false` so its ready for the next start.

### Detailed Breakdown of each attribute
| Option      | Description |
| ----------- | ----------- |
| `serviceID`   |Current/Last Service Process ID (Used to close previous service if one is running)|
| `stop`   |Set to `true` to have service close|
| `fade`   |Set to `true` to allow audio transition instead of instant change|
| `calls`   |Use any of the previously listed commands to modify audio source values|
| `device`   |JSON object with all data for currently set default audio output device|
| `sources`   |JSON Array with JSON Objects of sources that are currently outputting (or at least telling windows they are outputting) audio|


## Credits and Tools Used:
This project makes use xenolightning's [AudioSwitcher](https://github.com/xenolightning/AudioSwitcher) .NET Library as well as some basic C# Code 
