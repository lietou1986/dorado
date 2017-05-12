mkdir c:\clientservice2
cd /D c:\clientservice2

sc stop ClientServiceV2
ping -n 5 127.0.0.1>nul
echo open 10.251.141.84 >ftp.txt 
echo vwsftp2>>ftp.txt 
echo vwsftp2>>ftp.txt 
echo binary >>ftp.txt
echo prompt>>ftp.txt
echo mget * >>ftp.txt 
echo bye >>ftp.txt
ftp -s:ftp.txt
del ftp.txt

C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\InstallUtil.exe Vancl.IC.VWS.ClientService2.exe
sc start ClientServiceV2
