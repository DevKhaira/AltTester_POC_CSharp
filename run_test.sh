#!/bin/bash

DEVICE_ID="3A271FDJH00466"
APK_PATH="./apk/ALL.apk"
PROJECT_PATH="TestALL.csproj"
PACKAGE_NAME="com.homagames.studio.allinhole"


# # Uninstall the old APK if it exists
# if adb -s "$DEVICE_ID" shell pm list packages | grep "$PACKAGE_NAME"; then
#     echo "Uninstalling existing app..."
#     adb -s "$DEVICE_ID" uninstall "$PACKAGE_NAME"
# fi

# # Install the APK
# echo "Installing APK..."
# adb -s "$DEVICE_ID" install "$APK_PATH"

# # Launch the app
# echo "Launching the Unity app..."
adb reverse tcp:13000 tcp:13000
adb shell monkey -p $PACKAGE_NAME -c android.intent.category.LAUNCHER 1

sleep 5  # Wait for the app to start

# Run NUnit tests
echo "Running NUnit tests in TestALLCSharp.csproj..."
dotnet test "$PROJECT_PATH"

echo "Test execution complete!"
