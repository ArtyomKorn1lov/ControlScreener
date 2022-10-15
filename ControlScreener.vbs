Dim WshShell
Set WshShell = CreateObject("WScript.Shell")
WshShell.CurrentDirectory="C:\ControlScreener\"
WshShell.Run "ControlScreener.exe", 0
Set WshShell = Nothing