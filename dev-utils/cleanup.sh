#!/usr/bin/env bash
shopt -s globstar

cd $( dirname "${BASH_SOURCE[0]}" )/..
pwd
echo
rm -r engine/Temp
rm -r engine/Library/metadata
rm -r engine/Library/ShaderCache
rm -r engine/Library/ScriptAssemblies
rm -r engine/Library/obj
rm    engine/Library/assetDatabase3
rm    engine/Library/CurrentLayout.dwlt
rm    engine/Library/CurrentMaximizeLayout.dwlt
rm    engine/Library/expandedItems
rm    engine/Library/LastSceneManagerSetup.txt
rm    engine/Library/*.log
rm    engine/Assets/**/*.meta
echo
echo finished
sleep 1
