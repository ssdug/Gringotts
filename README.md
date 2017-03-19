# Gringotts Financial [![Build status](https://ci.appveyor.com/api/projects/status/r26bfloq6gl2b7r8?svg=true)](https://ci.appveyor.com/project/NotMyself/gringotts)

![Gringotts](/docs/images/gringotts_wide.jpg?raw=true "Gringotts")

## TOC

- [About](#about)
- [Getting Started](#getting-started)
- [External Dependencies](#external-dependencies)
- [Platform Dependencies](#platform-dependencies)
- [Features](#features)
- [Architecture](#architecture)


### About

The [Ministry of Magic](http://harrypotter.wikia.com/wiki/British_Ministry_of_Magic) hereby agrees to engage [Gringotts Wizarding Bank of England](http://harrypotter.wikia.com/wiki/Gringotts_Wizarding_Bank) for their unapparelled services in managing funds for all of its branches that provide services to the Magical Community.

The Ministry shall entrust the skilled Goblins of Gringotts to care for the wizarding gold for it's educational institution, [Hogwarts School of Witchcraft and Wizardry](http://harrypotter.wikia.com/wiki/Hogwarts_School_of_Witchcraft_and_Wizardry), to
see that student accounts are safe and secure.

In addition, Gringotts will manage the funds used for the running of [Azkaban Wizard Prison](http://harrypotter.wikia.com/wiki/Azkaban) to ensure that individuals who've broken magical law are kept safely away from
the Magical Community and are property cared for during their incarceration.

Gringotts shall also manage the gold used for all magical health and welfare serves
provided by the Ministry including [St. Mungo's Hospital for Magical Maladies and
Injuries](http://harrypotter.wikia.com/wiki/St_Mungo's_Hospital_for_Magical_Maladies_and_Injuries), the [Ariana Dumbledore](http://harrypotter.wikia.com/wiki/Ariana_Dumbledore) Mental Health Institution, the [Argus Flitch](http://harrypotter.wikia.com/wiki/Argus_Filch) Home for
Squibs and the [Tom Riddle](http://harrypotter.wikia.com/wiki/Tom_Riddle) Memorial Orphanage.

### Getting Started

1. Clone the repository: `git clone https://github.com/NotMyself/Gringotts.git`.
1. Change directory into the cloned repository `cd Gringotts`.
1. Open the `Gringotts.sln` in Visual Studio.
1. Open the Pacakge Manager Console
1. Run the command `Update-Database`
1. Hit `F5` to start the web application.

**Note:** This application will run mostly out of the box. It has direct dependencies on Active Directory, so some functions will throw an error attempting to validate something in AD. Gringotts assures us this will be fixed in a future version.

### External Dependencies

#### .NET

- [Autofac](https://autofac.org/) - Dependency Intection framework used for service and plugin registration
- [ImageResizer](https://imageresizing.net/) - Image Manipulation library used to resize profile images
- [MediatR](https://github.com/jbogard/MediatR) - Simple Mediator implementation used to process Commands, Queries & Notifications
- [NLog](http://nlog-project.org/) - NLog makes it easy to produce and manage high-quality logs for your application
- [Glimpse](http://getglimpse.com/) - Providing real time diagnostics & insights

#### JavaScript, HTML, CSS

- [jQuery](https://jquery.com/) - jQuery is a fast, small, and feature-rich JavaScript library
- [lodash](https://lodash.com/) - A modern JavaScript utility library delivering modularity, performance & extras
- [mustache.js](https://mustache.github.io/) - Logic-less client side templates
- [Bootstrap](http://getbootstrap.com/) - Bootstrap is the most popular HTML, CSS, and JS framework
- [FontAwesome](http://fontawesome.io/) - Font Awesome gives you scalable vector icons that can instantly be customized

### Platform Dependencies

- [ASP.NET MVC](https://www.asp.net/mvc)
- [Entity Framework](https://msdn.com/data/ef)
- [MS Test](https://www.visualstudio.com/en-us/docs/test/developer-testing/index)

### Features

- [Accounts](docs/mockups/Account/readme.md)
- [Clients](docs/mockups/Client/readme.md)
- [Organizations](docs/mockups/Organization/readme.md)
- [Vendors](docs/mockups/Vendor/readme.md)
- [Dashboard](docs/mockups/Dashboard/readme.md)

### Architecture

- [Logging](docs/logging.md)
- [Data Access](docs/dataaccess.md)
- [Diagnostics](docs/diagnostics.md)
- [Multitenancy](docs/multitenancy.md)
- [Dependency Injection](docs/dependencyinjection.md)
- [Command/Query Seperation](docs/commandqueryseperation.md)
