FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base

COPY bin/Debug/net8.0/publish .

ENTRYPOINT["dotnet", "ParallelDfs.dll"]