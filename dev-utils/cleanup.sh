#!/usr/bin/env bash
shopt -s globstar

cd $( dirname "${BASH_SOURCE[0]}" )/..
pwd
echo
rm -r Engine/Temp
rm -r Engine/obj
rm -r Engine/Library
rm -r Engine/UnityPackageManager
rm    Engine/Assets/**/*.meta
rm    Engine/ProjectSettings/AudioManager.asset
rm    Engine/ProjectSettings/ClusterInputManager.asset
rm    Engine/ProjectSettings/DynamicsManager.asset
rm    Engine/ProjectSettings/EditorBuildSettings.asset
rm    Engine/ProjectSettings/InputManager.asset
rm    Engine/ProjectSettings/NavMeshAreas.asset
rm    Engine/ProjectSettings/NetworkManager.asset
rm    Engine/ProjectSettings/Physics2DSettings.asset
rm    Engine/ProjectSettings/TagManager.asset
rm    Engine/ProjectSettings/TimeManager.asset
rm    Engine/ProjectSettings/UnityConnectSettings.asset
rm    Engine/Assembly-CSharp.csproj
rm    Engine/Assembly-CSharp-Editor.csproj
rm    Engine/Assembly-CSharp-Editor-firstpass.csproj
rm    Engine/engine.sln
rm    Engine/Assets/*.generated.*
rm    Engine/Assets/Plugins/*.generated.*

rm -r Engine/.vscode
rm -r Engine/.idea
rm -r Engine/Assets/Plugins/Editor/JetBrains

rm    Engine/Debug.log
echo
echo finished
sleep 1
