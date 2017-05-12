@echo 正在安装WindowService
@Set Path=C:\Windows\Microsoft.NET\Framework\v4.0.30319;
@Set svn_dir=%~dp0
installutil %svn_dir%Dorado.VWS.ClientHost.exe

@echo 正在启动ServiceWindowService
C:\WINDOWS\system32\net.exe start "Dorado.VWS.ClientHost"

pause
@echo 成功！