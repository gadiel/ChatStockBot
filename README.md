# ChatStockBot - Jobsity .NET Challenge

This is a chat application written in C# .NET Core 2.0, using a ASP.NET Core Web Application implementing SignalR for realtime comunication and a Decoupled ChatBot Microservice that communicates with the WebApp using RabbitMQ. 
# Features

  - User Registration and Login using an email
  - Chatroom for several users
  - Stock Updates with command \stock=stock_symbol

### Installation

ChatStockBot requires Visual Studio 2017 With .NET Core 2.0 Installed. It also needs [RabbitMQs](https://www.rabbitmq.com/install-windows.html) v3.7.17, RabbitMQ requires [ERLang OTP](https://www.erlang.org/downloads) v22.

ChatStockBot needs at least SQL Server Express for it's data persistence needs.

Nuget Package Manager will automatically restore all the dependencies required for each project when opening the project or atempting to compile it.

### Migrations

ChatStockBot uses .NET Core Entity Framework code first approach for it's Data Access Layer, currently I have the connection string configured to use SQL Server Express `LocalDB`. The Connection string needs to be updated if a different version of SQL Server is used.

In order to execute the migration for the table creation, run the following command in the Package Manager Console, located at Tools > Nuget Package Manager > Package Manager Console
Make sure the Default Project is `ChatLogicLayer`

```sh
$ Update-Database 
```

Migrations can also be run with the `dotnet` cli command, preferably in a `PowerShell` terminal
```sh
$ cd {ChatLogicLayer Project folder inside the Solution}
$ dotnet ef database update
```

### Running the App

In order to run the ChatBotBroker and the WebApp I recommend configuring the solution to run `Multiple Startup Projects` in order to configure this right click the solution and go to `Properties` inside the dialog select CommonProperties > Startup Project. Select Multiple startup projects and set ChatBotBroker and ChatLogicLayer action to `Start`

### Projects

| Project | Description | README |
| ------ | ------ | ------ |
| ChatLogicLayer | Web Application | [ChatLogicLayer/README.md][PlDb] |
| ChatBotBroker | Console App with the Bot Broker Service | [ChatBotBroker/README.md][PlGh] |
| ChatBotBroker.Test | xUnit Test Project | [ChatBotBroker.Test/README.md][PlGd] |
| ChatBot.Models | Class Library | [ChatBot.Models/README.md][PlOd] |

### TODOs

 - Add Repository Pattern for the EF DbContext
 - Add Signed SSL
