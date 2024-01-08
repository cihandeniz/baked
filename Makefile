.PHONY: build run test

build:
	dotnet build
run:
	@ \
	echo "(1) Blueprints.Service (Development)" ; \
	echo "(2) Blueprints.Service (Production)" ; \
	echo "(3) Do.Docs" ; \
	echo "" ; \
	echo "Please select 1-2: " ; \
	read app ; \
	if test $$app -eq "1" ; then \
		dotnet run --project test/blueprints/Do.Test.Blueprints.Service.Application ; \
	fi ; \
	if test $$app -eq "2" ; then \
		docker compose up --build ; \
	fi ; \
	if test $$app -eq "3" ; then \
		cd ./docs ; \
		make run ; \
	fi
test:
	dotnet test

# dotnet test -c Release --collect:"XPlat Code Coverage" --logger trx --results-directory .coverage --settings test/runsettings.xml
# reportgenerator -reports:.coverage\0d84daea-0041-4f8d-a93c-51d3d348fa69\coverage.cobertura.xml;.coverage\d606db4f-8ea5-4e9f-a304-f37b22a1f34b\coverage.cobertura.xml -targetdir:.coverage/report