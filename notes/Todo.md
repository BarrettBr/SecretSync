## Todo:

### Documentation:

- [Create man page](https://www.linuxhowtos.org/System/creatingman.htm) for command

### Automatic Packaging and Installation:

- The below is handled via ci/cd ations (Maybe github actions? Feel like there must be a better way)
  - Linux: rpm & deb packages, can use [nfpm](https://github.com/goreleaser/nfpm)
  - Windows: msi installation files, can use [WiX framework](https://www.firegiant.com/wixtoolset/) or look into default [Windows Instructions](https://learn.microsoft.com/en-us/intune/configmgr/develop/apps/how-to-create-the-windows-installer-file-msi)
  - Package source files to give out source code

### Structure out skeleton

- DONE - Build out Program.cs and start accepting API requests, can build a quick echo project as a marker of success
  - Ensure layers can talk so make sure an API request comes in goes down the chain hits each printing off a ping message and then exiting
- Partial - Build out application/basic interfaces, dont need functionality to all be there but basic "thing" that is there

### Building out endpoints

- After the basic skeleton is done build out the basic endpoints connecting them from call -> application layer
