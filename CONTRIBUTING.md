# Overview
In order to work with and contribute to this project it is important that you understand the project structure, and how to work with and build the various pieces.  This project is slightly more complex than a typical KSP plugin, so please read this carefully.

## Project Structure
There are three main components to this project; The core engine solver, the KSP mod/plugin, and the Unity project.

* **Core Engine Solver**: Located in `./Core`.  This is the meat of the project and is what does the actual work.  It is designed as a stand-alone C# Class Library that has no dependency on KSP or Unity.  This is used by both the Unity and KSP code.
* **KSP Mod**: Located in `./KSP`. This the code neccesary to bring the engine solver into KSP and interact with it as a PartModule.  This is your classic KSP mod code and has dependencies on the Core as well as KSP assemblies.
* **Unity Tools** Located in `./Unity`.  These are custom inspectors, and other tools for use inside the Unity Editor.  These tools offer to main features; Ease of testing the solver, and ease of importing and generating the datasets the solver uses.  This has a depdnecy on the Core as well as obviously Unity.

## Building
To build the project you can either build the Core by itself if desired, or more often you want to build both the Core and the KSP mod at the same time.  You can do this with the `EngineDesignerKSPFull.sln` solution.  This will build the Core and KSP libraries, as well as copy the Core DLL into the Unity side, and copy both the Core and KSP DLLs to `./GameData/EngineDesigner/Plugins`.
**NOTE** That building the KSP code requires the KSP assemblies which are not distributed with this project for legal reasons.  You will need to provide them from your copy of the game. By default the project expexts them to be in `./bin`.  If you want to put them elsewhere please **do not** change the project files to repoint but instead us a local ReferencePath either in Visual Studio. msbuild, or Rider.

The first time you compile you may need to manually create `./GameData/Plugins`

The Unity side is not "compiled".  Simply open the unity project in the Unity Editor using **2019.2.2f1** (unityhub://2019.2.2f1/ab112815d860) and point to the `./Unity` folder.

## KSP Configs
### Creating configs
Tools inside the Unity project make exporting ModuleManager style configs for KSP super easy.  Simply select a Propellant Mixture dataset and in the inspector click the `Export Config` button.  
This will create a .CFG file in `Assets/Data/Configs` with the same name as the original dataset asset.

### Using configs
Simply copy the .CFG files you need from `./Unity/Assets/Data/Configs` to `./GameData/EngineDesinger/`

## CEARUN Chemical Datasets
EngineDesigner is not a complete engine solver.  It does not include a chemical solver, but instead relies on pre-calculated chemical data from CEARUN.
The Unity project includes tools to make importing this data simple.

<Add instructions for running, importing, and using the data here>
