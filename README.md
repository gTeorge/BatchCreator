# ScriptRunner

ScriptRunner is a tool for creating batch files for consecutive execution of multiple scripts with a specified delay between them.

## Installation

Install [Dotnet Core 3.1 or newer.](https://dotnet.microsoft.com/download)
No other installation is needed. Just copy files.

## Usage

**Set column**
Sets/unsets all items

**Select folder**
Loads filenames from a selected folder.

**Create Batch**
Creates a batch file from items that are set.

**Save**
Saves current filenames to a JSON file.

**Load**
Loads a previously saved files

**Command**
Command that will be using each file as a parameter.

**Delay**
Delay in seconds between each file.

**Filter ext**
Filters the view to show only files with a corresponding file extension.

## Config.json
You can set there what should be loaded when app is open.
```json
{
  "Command": "C:\\\\GIT_MRS_REQ_2\\\\scripts\\pt_launcher.bat",
  "DelaySec": 600,
  "Scripts": null,
  "FileExtensions": ["all", ".bat", ".md"]
}
```

## Planned
Buttons for hide and remove file from list.