@echo off
echo Sending shutdown warning to players...

curl http://localhost:127.0.0.1/shutdown-warning

timeout /t 30 /nobreak > NUL

echo Stopping server process...
taskkill /IM VintageStoryServer.exe /F