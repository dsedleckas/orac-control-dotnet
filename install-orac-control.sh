#!/bin/bash

DOTNET_VERSION=3.0.100-preview5-011568
DOTNET_FILE=dotnet-sdk-3.0.100-preview5-011568-linux-arm.tar.gz
REPO_NAME=orac-control-dotnet
REPO_URL=https://github.com/dsedleckas/$REPO_NAME.git
DOTNET_DIR=/usr/local/dotnet
APP_DIR=/usr/local/orac-control/

cd $HOME

echo "Looking for .NET Core " $DOTNET_VERSION

if ! $DOTNET_DIR/dotnet --version | grep -q "$DOTNET_VERSION"; then
	if [ ! -f "$HOME/$DOTNET_FILE" ]; then
    	echo "Downloading" $DOTNET_FILE
		wget -P $HOME https://download.visualstudio.microsoft.com/download/pr/176a9b0c-bf87-4ddc-856e-9a0a71d37fa5/a6fc47f80927ba2f0abde74d1185fc0b/$DOTNET_FILE
		echo ".NET Core 3.0 file download OK" 
	else
		echo "$DOTNET_FILE in place"
	fi
	mkdir -p $HOME/dotnet
	tar -zxvf $HOME/$DOTNET_FILE -C $HOME/dotnet
	sudo cp -R $HOME/dotnet/. $DOTNET_DIR/
	echo .NET Core $DOTNET_VERSION ready
else
	echo .NET Core 3.0 SDK $DOTNET_VERSION already installed
fi

if [ -z `which git` ]; then
	echo "Installing git..."
	sudo apt-get update
	sudo apt-get install -y git
fi

if [ ! -d "$REPO_NAME" ]; then
	echo Cloning repository from $REPO_URL
	git clone $REPO_URL
	cd $REPO_NAME
else
	echo Updating $REPO_NAME repository with latest stuff
	cd $REPO_NAME
	git pull
fi

echo "Build started..."
$DOTNET_DIR/dotnet publish -c Release -o app ./OracControl/OracControl.csproj
sudo cp -R app/. $APP_DIR
sudo install -v -m 644 orac-control.service /usr/lib/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable orac-control.service
sudo systemctl restart orac-control.service