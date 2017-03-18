# Gringotts Financial 

![Gringotts](/docs/images/gringotts.jpg?raw=true "Gringotts")

## TOC

 - [About](#about)
 - [Getting Started](#getting-started)
 - [External Dependencies](#external-dependencies)

### About




### Getting Started

1. Clone the repository: `git clone https://github.com/NotMyself/Gringotts.git`.
1. Change directory into the cloned repository `cd Gringotts`.
1. Open the `Gringotts.sln` in Visual Studio.
1. Open the Sql Server Object Explorer View, View > Sql Server Object Explorer.
1. Under the `(localdb)\MSSQLLocalDB` node, right click Databases and select `Add New Database`.
1. Set Database Name to `Gringotts` and click `OK`.
1. Open the Pacakge Manager Console
1. Run the command `Update-Database`
1. Hit `F5` to start the web application.

### External Dependencies

1. [Autofac](https://autofac.org/) - Dependency Intection framework used for service and plugin registration