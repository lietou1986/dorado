@ECHO OFF

REM ======================================================================
REM
REM Batch File -- Created with manual
REM
REM NAME: 
REM
REM AUTHOR:  , 
REM DATE  : 2011-08-04
REM
REM COMMENT: 
REM
REM ======================================================================

TITLE XML配置映射类生成工具 Email:zhangjiangsong@vancl.com
MODE CON: COLS=60 LINES=30
REM COLOR 2e
COLOR 4f

:REPEAT
CLS
ECHO. 
ECHO===============XML配置映射类生成工具==============
ECHO. 
:INPUT
SET xsd=xsd.exe
SET configpath=
SET configdir=
SET configname=
SET configext=
SET xsdpath=
SET cspath=
ECHO 英雄们，开始拖入您的配置文件吧（Xml or Config）：
ECHO. 
SET /p configpath=
:APPLY
IF NOT DEFINED configpath (GOTO INVALID)
IF DEFINED configpath (GOTO CREATE) 
:OPENDIR
start %configdir%
GOTO SUCCESS
:CREATE
CALL :INIT %configpath%
IF %configext% EQU .config (GOTO COPYXML)
IF %configext% EQU .xml (GOTO CREATEXSD)
GOTO NONSUPPORT
:CREATEXSD
ECHO. 
ECHO 准备生成架构文件 。。。
ECHO. 
xsd.exe %configpath% /outputdir:%configdir%
if not exist %xsdpath%(
goto CREATEXSDFAIL
)
ECHO. 
GOTO CREATECS
:CREATECS
REM Color 69
ECHO 架构文件生成成功，正在准备生成配置对象文件。。。
ECHO. 
xsd.exe %xsdpath% /c /outputdir:%configdir%
ECHO. 
GOTO XSD
:INVALID 
ECHO 英雄们，请拖入您的配置文件：
ECHO. 
GOTO SELECT
:FAIL
REM Color 60
ECHO. 
ECHO 抱歉，生成配置对象失败!
ECHO. 
GOTO SELECT
:NONSUPPORT
ECHO. 
ECHO 抱歉，您提供的不是有效的配置文件!
ECHO. 
GOTO SELECT
:CREATEXSDFAIL
REM Color 60
ECHO. 
ECHO 抱歉，生成XML架构失败!
ECHO. 
GOTO SELECT
:CREATECSFAIL
REM Color 60
ECHO 抱歉，生成配置对象文件失败!
ECHO. 
GOTO SELECT
:SUCCESS
REM Color 4f
ECHO 恭喜您我的英雄，大功告成!
ECHO. 
GOTO SELECT
:COPYXML
if exist %configname%() else(
copy %configpath% %configname%
)
set configpath=%configname%
GOTO CREATEXSD
:SELECT
SET choice=
SET /p choice=Are you ready？[Y/N]:

IF NOT DEFINED choice (GOTO SELECT)
IF %choice% EQU Y (GOTO REPEAT) 
IF %choice% EQU y (GOTO REPEAT) 
IF %choice% EQU N (GOTO END)
IF %choice% EQU n (GOTO END)  
GOTO SELECT
:DELXSD
ECHO 准备清理架构文件。。。
ECHO. 
del %xsdpath%
ECHO 架构文件清理完毕！
ECHO. 
GOTO OPENDIR
:XSD
SET del=
SET /p del=要删除架构文件吗？[Y/N]:
ECHO. 
IF NOT DEFINED del (GOTO XSD)
IF %del% EQU Y (GOTO DELXSD) 
IF %del% EQU y (GOTO DELXSD) 
IF %del% NEQ Y (GOTO OPENDIR)
IF %del% NEQ y (GOTO OPENDIR)  
GOTO SELECT
:END
PAUSE
:INIT
set configdir=%~dp1 
set configname=%~dp1%~n1.xml
set configext=%~x1
set xsdpath=%~dp1%~n1.xsd 
set csdpath=%~dp1%~n1.cs
























