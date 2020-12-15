#!/bin/bash

BUILD=""

if [ -z "$1" ]
then
	echo "Build type not specified, falling back to Debug"
	BUILD="Debug"
else
	BUILD=$1
fi

dotnet build -c $BUILD
echo "Now copying assets over.... "
cp -rf assets/ bin/$BUILD/netcoreapp3.1/
echo "Completely done!"
