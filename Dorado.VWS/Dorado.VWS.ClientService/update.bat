cd /d %~dp0
rem net stop Dorado.VWS.ClientHost
rem ftp -s:ftp.txt
rem del ftp.txt

rem xcopy /e /y \\172.30.11.70\D$\Source\Platform\Batch\Dorado.VWS.ClientHost .
rem xcopy /e /y \\172.17.1.85\shareforder\ClientService . >log

rem C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe Dorado.VWS.ClientHost2.exe
rem net start Dorado.VWS.ClientHost

start ClientUpdate.exe
