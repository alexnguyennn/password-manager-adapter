all: pw-cli

pw-cli:
	dotnet publish -c Release

install: pw-cli
	mkdir -p /opt/pw-cli
	cp -rf src/Cli/bin/Release/netcoreapp2.2/publish/. /opt/pw-cli
	chmod 755 /opt/pw-cli
	cp -f scripts/pw-dmenu /usr/bin
	chmod 755 /usr/bin/pw-dmenu

uninstall:
	rm -rf /opt/pw-cli
	rm -rf /usr/bin/pw-dmenu
