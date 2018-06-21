# Virtual Reality Chemical Space
![VRCS User](http://viz.gdb.tools/images/vrcs_user.jpg)
[**Download Windows Binary**](http://viz.gdb.tools/bin/vrcs_21062018.zip)
## Introduction
The recent general availability of low-cost virtual reality headsets and accompanying 3D engine support presents an opportunity to bring the concept of chemical space into virtual environments. While virtual reality applications represent a category of widespread tools other fields, their use in the visualization and exploration of abstract data such as chemical spaces has been experimental. Based on our previous work on [interactive 2D maps of chemical spaces](http://gdb.tools/tools), followed by interactive web-based 3D visualizations ([WebDrugCS](http://gdbtools.unibe.ch:8080/webDrugCS/), [WebMolCS](http://gdbtools.unibe.ch:8080/webDrugCS/)), and culminating in the [interactive web-based 3D visualization of extremely large chemical spaces](http://faerun.gdb.tools), virtual reality chemical spaces are a natural extension of these concepts. As 2D and 3D embeddings and projections of high-dimensional chemical fingerprint spaces were shown to be valuable tools in chemical space visualization and exploration, [existing pipelines](http://doc.gdb.tools/fun) of data mining and preparation can be extended to be used in virtual reality applications. Here we present an application based on the [Unity engine](https://unity3d.com/) and the [virtual reality toolkit (VRTK)](http://vrtk.io), allowing for the interactive exploration of chemical space populated by [Drugbank](http://www.drugbank.ca) compounds in virtual reality

## Requirements
- Oculus Rift or HTC Vive VR headeset including respective controllers and lighthouses as well as supporting hardware
- Windows 10 (not tested on previous versions of Windows)
- A minimum of 8 GB of RAM

## Installation
You can download the precompiled windows binary here. Extract the contents of the zip folder and run `vrcs.exe`.

## Usage
Keyboard: <kbd>F</kbd> toggles the visiblity of the floor grid, while <kbd>B</kbd> toggles the visiblity of the skybox (the space themed background).
![VRCS Controls](http://viz.gdb.tools/images/vrcs_controls.jpg)

### Usage (Without a VR Headset)
If you do not have a VR headset, you will be able to use VRCS in a limitad fashion using your mouse and keyboard. The movement in the plane is controlled by <kbd>A</kbd><kbd>W</kbd><kbd>S</kbd><kbd>D</kbd> while the rotation of the simulated headset is controlled using the mouse.

## Building VRCS
In order to build VRCS, the latest release of [Unity3d](https://unity3d.com/) is required.