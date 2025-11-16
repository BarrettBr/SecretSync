SOLUTION=SecretSync.sln
SERVER_PROJECT=src/SecretSync.Server/SecretSync.Server.csproj

.PHONY: build run-server all clean restore watch
all: build run-server

restore:
	@dotnet restore $(SOLUTION)

build:
	@dotnet build $(SOLUTION)

run-server:
	@dotnet run --project $(SERVER_PROJECT)

watch:
	@dotnet watch --project $(SERVER_PROJECT) run

clean:
	@dotnet clean $(SOLUTION)
