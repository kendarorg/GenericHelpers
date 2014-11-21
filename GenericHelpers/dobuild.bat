@echo off
call dobuild_env.bat

call dobuild_single GenericHelpers 4.0 net40
call dobuild_single GenericHelpers 4.5 net45
call dobuild_nuget GenericHelpers

pause
