#!/usr/bin/env bash
shopt -s globstar

cd $( dirname "${BASH_SOURCE[0]}" )/..
pwd
echo
rm -r engine/Temp
rm -r engine/obj
rm -r engine/Library
rm -r engine/UnityPackageManager
rm    engine/Assets/**/*.meta
rm    engine/ProjectSettings/AudioManager.asset
rm    engine/ProjectSettings/ClusterInputManager.asset
rm    engine/ProjectSettings/DynamicsManager.asset
rm    engine/ProjectSettings/EditorBuildSettings.asset
rm    engine/ProjectSettings/InputManager.asset
rm    engine/ProjectSettings/NavMeshAreas.asset
rm    engine/ProjectSettings/NetworkManager.asset
rm    engine/ProjectSettings/Physics2DSettings.asset
rm    engine/ProjectSettings/TagManager.asset
rm    engine/ProjectSettings/TimeManager.asset
rm    engine/ProjectSettings/UnityConnectSettings.asset
rm    engine/Assembly-CSharp.csproj
rm    engine/engine.sln

rm -r engine/.vscode
rm -r engine/.idea
rm    engine/Assembly-CSharp-Editor-firstpass.csproj
rm    engine/Assets/Plugins/Editor/JetBrains
echo
echo finished
sleep 1
