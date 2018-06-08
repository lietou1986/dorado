@echo 正在停止ServiceWindowService
C:\WINDOWS\system32\net.exe stop "Dorado.VWS.ClientHost"

@echo 正在卸载WindowService

@Set Path=C:\Windows\Microsoft.NET\Framework\v4.0.30319;
@Set svn_dir=%~dp0
installutil %svn_dir%Dorado.VWS.ClientHost.exe /u

pause
@echo 成功！