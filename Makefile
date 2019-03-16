.PHONY: run
run:
	cd archive && dotnet run .

.PHONY: migrate
migrate:
	cd archive && dotnet ef migrations ${NAME}

.PHONY: update
update:
	cd archive && dotnet ef database update
