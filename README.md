# a2-s3740446-s3734938
Assignment 2 Web Development Technologies 2020 Flexible Semester

Ryan Cassidy - s3740446

Vineet Bugtani - s3734938

Equal contribution by both assignment partners

Repository for the NWBA Online Banking Application Project.

This project was made in ASP.net core in Visual Studio while being hosted on a Microsoft Sql Server.
It is composed of a base project written in C# as MVC pattern and a seperate class library containing a generic repository pattern. 

This project utilizes EF core to manage and validate business model objects.
The repository is utilized via the wrapper objects contained under Data/RepositoryWrapper in the project files. 

In order to run this project the solution must be opened in Visual Studio and the project needs to reference the dll included in this github repository. RepositoryDLL contains both the DLL and the project solution for viewing the repository pattern.

Business objects include the Account, Billpay, Login, Billpayservice and Repository wrapper.

The Account model object handles ATM transactions and the payment of bills through methods within the objects.

The Billpay model object allows for updating when bills are modified.

The Login model handles validation users changing their passwords.

The Billpay hosted service is run every 15 seconds to automatically check for due bills and pay them.

The repository wrapper allows for generic calls of EF Core to interact with the database. This with a wrapper allows for future implementation of methods for simplified DB exchange. 

All of these model objects were chosen as the data required for these tasks is contained within them so it would seem logical that they would control and validate them in order to complete business operations. This also keeps controllers away from model logic.

Code references: 

Paging and some controller examples taken and modified from RMIT Web Development Technologies tutorials

Background Task implementation learnt from Microsoft Docs:
https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-3.1&tabs=visual-studio

DbContext Scoping learnt from StackOverflow user alsami:
https://stackoverflow.com/questions/51572637/access-dbcontext-service-from-background-task

Generic Repository example from code-maze:
https://code-maze.com/net-core-web-development-part4/
