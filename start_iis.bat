@echo off
title IIS Express - WebApp CD45 (DREAMH)
echo ========================================================
echo   KHOI DONG IIS EXPRESS CHO DU AN BVTL - WEBAPP CD45
echo   URL HTTP : http://localhost:49856/
echo   URL HTTPS: https://localhost:44374/
echo ========================================================
"C:\Program Files\IIS Express\iisexpress.exe" /config:"D:\Projects\BVTL-X\.vs\WebApp\config\applicationhost.config" /site:"WebApp"
pause
