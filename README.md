# Gringotts Financial [![Build status](https://ci.appveyor.com/api/projects/status/r26bfloq6gl2b7r8?svg=true)](https://ci.appveyor.com/project/NotMyself/gringotts)

![Gringotts](/docs/images/gringotts_wide.jpg?raw=true "Gringotts")

## TOC

 - [Contract for Magical Financial Management](#contract-for-magical-financial-management)
 - [Getting Started](#getting-started)
 - [External Dependencies](#external-dependencies)


### Contract for Magical Financial Management

The Ministry of Magic hereby agrees to engage Gringotts Wizarding Bank of England for 
their unapparelled services in managing funds for all of its branches that provide 
services to the Magical Community.

The Ministry shall entrust the skilled Goblins of Gringotts to care for the wizarding
gold for it’s educational institution, Hogwarts School of Witchcraft and Wizardry, to
see that student accounts are safe and secure. 

In addition, Gringotts will manage the funds used for the running of Azkaban Wizard
Prison to ensure that individuals who’ve broken magical law are kept safely away from
the Magical Community and are property cared for during their incarceration. 

Gringotts shall also manage the gold used for all magical health and welfare serves
provided by the Ministry including St. Mungo’s Hospital for Magical Maladies and
Injuries, the Ariana Dumbledore Mental Health Institution, the Argus Flitch Home for
Squibs and the Tom Riddle Memorial Orphanage. 




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
1. [ImageResizer](https://imageresizing.net/)
1. [MediatR](https://github.com/jbogard/MediatR)
1. [NLog](http://nlog-project.org/)
1. [Glimpse](http://getglimpse.com/)
1. [jQuery](https://jquery.com/)
1. [lodash](https://lodash.com/)
1. [mustache.js](https://mustache.github.io/)
1. [Bootstrap](http://getbootstrap.com/)
1. [FontAwesome](http://fontawesome.io/)