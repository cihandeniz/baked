.PHONY: format build test run
FILE ?= file_name

format:
	@ \
	dotnet format --verbosity normal ; \
	cd test/recipe/admin ; npm run lint -- --fix ; cd ../../.. ; \
	cd docs/.theme ; npm run lint -- --fix ; cd ../..
fix:
	@ \
	cd src/recipe/admin ; npx eslint $(TARGET) --fix ; cd ../../.. ; \
	cd test/recipe/admin ; npx eslint $(TARGET) --fix ; cd ../../..
install:
	@ \
	cd src/recipe/admin ; npm ci ; cd ../../.. ; \
	cd test/recipe/admin ; npm ci ; cd ../../..
build:
	@ \
	cd src/recipe/admin ; npm run build ; cd ../../.. ; \
	dotnet build
test:
	@ \
	dotnet test --logger quackers ; \
	cd test/recipe/admin ; SILENT=1 npm run test -- --grep-invert @visual ; cd ../../..
coverage:
	@ \
	rm -rdf .coverage ; \
	dotnet test -c Release --collect:"XPlat Code Coverage" --logger trx --results-directory .coverage --settings test/runsettings.xml ; \
	dotnet reportgenerator -reports:.coverage/*/coverage.cobertura.xml -targetdir:.coverage/html ; \
	open .coverage/html/index.html
run:
	@ \
	echo "(1) Recipe.Service (Development)" ; \
	echo "(2) Recipe.Admin (Development)" ; \
	echo "(3) Recipe.* (Production)" ; \
	echo "(4) Docs" ; \
	echo "" ; \
	echo "Please select 1-4: " ; \
	read app ; \
	if test $$app -eq "1" ; then \
		dotnet run --project test/recipe/Baked.Test.Recipe.Service.Application ; \
	fi ; \
	if test $$app -eq "2" ; then \
		cd test/recipe/admin ; \
		npm run dev ; \
		cd ../../.. ; \
	fi ; \
	if test $$app -eq "3" ; then \
		docker compose up --build ; \
	fi ; \
	if test $$app -eq "4" ; then \
		cd ./docs ; \
		make run ; \
	fi
