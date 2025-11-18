# zgo
project generator inspected by UnrealEngine UnrealHeadTool/UnrealBuildTool, rust Cargo and dotnet CLI.

support ide 
- [ ] VisualStudio
  - [ ] c#
  - [ ] c/c++
- [ ] VisualStudioCode
- [ ] Xcode
- [ ] AndroidStudio
- [ ] CMake

support language
- [ ] c#
- [ ] c/c++

toolchain
- [ ] project
  - [ ] new: create init.zgo project file
  - [ ] setup: set toolchain and environment
  - [ ] generate: create solution project file
  - [ ] build: build project file
  - [ ] publish: make publish
  - [ ] clean: clean project files
  - [ ] run: build and execute bin
- [ ] dotnet
  - [ ] install: install sdk, app runtime and workload
  - [ ] nuget: nuget package download
  - [ ] exec: exec command use downloaded dotnet sdk
- [ ] cmake
  - [ ] install

cross app
- [ ] rust + c#
- [ ] c/c++ + c#



# Dependencies
- Toml: [Tommy](https://github.com/dezhidki/Tommy)