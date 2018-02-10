# Scripting
## Editor
- https://github.com/Microsoft/monaco-editor

Monaco is rich web-based code editor with support for, among others, TypeScript and JavaScript. Offers features such as navigation from references to definitions, diff visualisation, syntax checking and highlighting.

## Language
- https://github.com/Microsoft/TypeScript
- https://github.com/fabiandev/typescript-playground

TypeScript can be transpiled to JavaScript in browser, before saving. Both TypeScript source and generated JavaScript can be trasparently commited at once to avoid need for separate build step, together with their source map for easier debugging.

## Storage
- https://github.com/github-tools/github
- https://github.com/krispo/git-edit/blob/master/git-edit.js

GitHub API can be used to create forks, configure https access (Pages), retrieve script content and commit changes. 

## Engine interop
Custom engine build step may generate glue code between engine and scripts based on reflection and JS syntax tree.
Based on public engine interface, TypeScript class Engine will be generated. 
Based on public script functions, C# class Scripts will be generated (with method stubs for editor preview mode).
Both will be commited with each engine build together with WebAssembly binaries.
In the future monolithic "Engine" and "Scripts" interfaces may be split into, possibly corresponding to each other, object oriented classes (like Engine.Character.ts and Scripts.Character.cs).
