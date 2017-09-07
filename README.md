# Ooogles - Object Oriented OpenGL-ES 2.0

Ooogles is a ultra-thin object oriented wrapper around OpenGL-ES 2.0 with near-zero impact on performance. It works for iOS and Android, but can also be used to create OpenGL-ES applications for Windows using [ANGLE](https://github.com/Microsoft/angle). 

This wrapper is still pretty low-level. It is not a "rendering engine" or high-level framework, so you still need OpenGL knowledge.

The main goals of Ooogles are to make it easier and less error-prone to use OpenGL-ES, offering error reporting in DEBUG builds and a better IntelliSense experience in the Visual Studio IDE. 

## Features

Ooogles has the following features:

- Encapsulates OpenGL "objects" into C# classes for a better organized and easier to use interface to OpenGL. 
- Provides type-safe access to most OpenGL constructs. 
- Is based on [OpenTK](https://opentk.github.io/) (for now), but provides a uniform API for iOS, Android and Windows, taking care of platform differences in the OpenTK API. 
- The wrapper is very thin. Most methods are inlined and wrap just a single OpenGL API. So the overhead of the wrapper is very low. 
- A few methods perform multiple OpenGL calls for convenience. For example, when compiling a shader, the wrapper not only calls the compile API, but also checks for compilation errors. In DEBUG mode, it will expose those errors through exceptions, and it will also log any warnings to the debug console. 
- OpenGL APIs that are not tied to an OpenGL "object" are wrapped in the static gl class. For example, the OpenGL API glClear is wrapped in the method gl.Clear. Not that, contrary to conventions, the name of this class is in lowercase. This is to avoid naming conflicts and confusion with the OpenGL GL class (it is also more in line with the original OpenGL APIs, which start with a lowercase gl). 
- When compiling with the DEBUG conditional define, every OpenGL call is checked for errors, and an exception is raised when an error occurs, indicating the type of error and in which method it occurred. When DEBUG is not defined, all error checking code is removed from the build so it has zero impact on performance. 
- Also, when compiling in DEBUG, warnings will be logged to the debug console if you forget to bind an object before you use it (or when a different object than the one you are using is currently bound). 
- The wrapper is well documented. Documentation can be found [on-line](https://neslib.github.io/Ooogles.Net/), as well as in the Ooogles.Net.Chm help file in this repository. Each type and method contains documentation from the official OpenGL-ES 2.0 reference, as well as custom documentation where needed. The documentation also shows which original OpenGL API call(s) are used in the implementation, to make it easier to find some method if you already know the API equivalent. 
- Comes with various samples that show how to use this wrapper. These samples work on Windows, iOS and Android. 

## Getting Started

Ooogles depends on the OpenTK that comes with Xamarin for iOS and Android. For Windows, to used OpenTK assembly is supplied in the Ooogles repository. 

There aren't any templates (yet) for creating Ooogles applications. For now, it is probably easiest to look at the sample applications and copy their structure and modify it to meet your requirements. 
