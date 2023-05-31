# Retrosynthesis-Route-MR-Visualizer

# How to setup

## Requirements
- Unity: 2021.3.26f1 (LTS)
- Visual Studio 2022: 17.6.2
- Mixed Reality Toolkit2: 2.8.3

## Install plugin

### Mixed Reality Toolkit

Follow the official instruction. https://learn.microsoft.com/en-us/training/modules/learn-mrtk-tutorials/1-5-exercise-configure-resources?tabs=openxr

## Data preparation

This application supports the output data of [ASKCOS Tree Builder](https://askcos.mit.edu/retro/network/).
Example chemical reaction network files are in `Assets/Resources/[small_network.json,medium_network.json,large_network.json]`.
If you want to use your own results, please add the file in `Assets/Resources/` and specify the file in `NetworkManager` object.

In addition, you must preprocess the file using https://github.com/sishida21/parse_ASKCOS_json and must check the mol and png files are generated in the `Assets/Resources/[images,molfiles]`, respectively.

## Build and Deploy

To build the application, please refer to the following link [Build and deploy to the HoloLens](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/build-and-deploy-to-hololens).
