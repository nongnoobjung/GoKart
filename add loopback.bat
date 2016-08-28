@echo off
rem Loginservers
netsh int ip add addr 1 address=64.94.106.161 mask=255.255.255.255 st=ac
netsh int ip add addr 1 address=64.94.106.162 mask=255.255.255.255 st=ac
netsh int ip add addr 1 address=64.94.106.163 mask=255.255.255.255 st=ac
netsh int ip add addr 1 address=64.94.106.164 mask=255.255.255.255 st=ac
netsh int ip add addr 1 address=64.94.106.165 mask=255.255.255.255 st=ac

rem Webserver
netsh int ip add addr 1 address=63.251.217.214 mask=255.255.255.255 st=ac

pause
