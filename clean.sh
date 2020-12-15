#!/bin/bash

BUILD=""

if [ -z "$1" ]
then
	echo "Type not specified, falling back to Debug"
	BUILD="Debug"
else
	BUILD=$1
fi

dotnet clean -c $BUILD
echo "Now deleting assets "
rm -rf bin/$BUILD/netcoreapp3.1/assets/
echo "Completely done!"
