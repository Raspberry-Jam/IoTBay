# IoTBay
### ISD Autumn 2025 class assignment
This program is made using the .NET SDK and C# language, leveraging the ASP.NET Core Framework, along with EntityFramework Core and Npgsql for PostgreSQL integration.

## Instructions
### Requirements
- Docker
- .NET SDK [9.0] (only for development and debugging)

### Running the program
To run this program, all you need is Docker. The .NET SDK and all libraries are handled in-container by the Dockerfile.

To get started, clone the repo and run `cd IoTBay/IoTBay`. From here, run `docker compose -f compose.yaml -f compose.release.yaml up`. This will build and run the Release 
target of the program, and host it on port 8080. Ensure your Docker networking settings allow access to this port from 
the host machine, and then using any modern browser, navigate to `http://localhost:8080`. If all is running correctly, 
you should see two containers running, one named `iotbay-final`, which is the ASP.NET web application, and one named 
`postgres`, which is our PostgreSQL database.

To restart with a fresh image (this will delete any data in the database), first run `docker compose down`, and then 
repeat the earlier Docker Compose command. If you wish to restart stopped containers without deleting them (thereby 
retaining database data), skip the `docker compose down` command and simply repeat the earlier command.

### Development
If you wish to modify and debug the code, you should install .NET SDK (9.0) on your host machine. The preferred IDE and 
debugger for this project is JetBrains Rider, but if you have a Visual Studio or VS Code setup for C#, those should work
fine too.

First, clone the repo, then `cd` into it, and run `dotnet restore IoTBay.sln`. This should configure the project on your
machine and retrieve all the necessary library packages using NuGet. To get started from the terminal, run 
`docker compose -f compose.yaml -f compose.dev.yaml up` (add `--build` any time you need to explicitly rebuild the 
project). If you are using JetBrains Rider, create a Docker Compose build configuration, add the `compose.yaml` and 
`compose.dev.yaml` files. Leaving Fast Mode transformations enabled typically yields better build and startup times, and
better debugger compatibility with Rider's built-in debugging tools. This configuration should allow you to add 
breakpoints at any point, and perform hot reloads to the code to change things during runtime (this has limited support 
within ASP.NET Core).

## How It Works
This application is developed in C# using .NET 9.0. The database schema is implemented on PostgreSQL. Deploying the
database and application is handled by Docker, so the host machine does not need to install .NET SDK or PostgreSQL 
Server to get started, only Docker.

The IoTBay application interfaces with the PostgreSQL database using a framework called EntityFramework Core, which is 
an intelligent library and toolset which can generate all the boilerplate code necessary to model the database schema in 
the project, which for this project is in the Models folder, and automatically generates the SQL queries sent to the database during runtime based on the interactions 
with its DbContext instance. Any database-related model is in the Models/Entities folder, and any other type of model is
typically stored somewhere else with an appropriately named folder. This database's schema-specific DbContext is the 
AppDbContext class (Models/AppDbContext.cs).

ASP.NET Core is the web server framework that the front end is built on, providing tools for rendering "Razor" pages, 
which are written in CSHTML, a fusion of HTML and C#, that is then rendered on the server side into pure HTML before 
being sent to the browser. ASP.NET Core allows the user to combine partial CSHTML views into one page, through powerful 
templating and layout tooling.

In this project, a CSHTML page generally uses a View Model, which is a mostly pure data class (sometimes with validation
logic), that gives the View (CSHTML page) a data structure to model itself around. This model can be instantiated and 
modified, then submitted to a Controller Action through a GET or POST method, typically with a form.

Given that this project is based on the ASP.NET MVC structure, in this context a Controller is a class that handles the 
business logic for a collection of Views (CSHTML pages) that are all grouped into one folder. The naming convention 
between this folder and the controller matter. For example, the UserController handles the business for all the views in 
the Views/User folder.

In the Controller class, each method is an Action, which is an HTTP API endpoint that is used to handle the business 
logic for that particular action. An action can be defined multiple times for different access methods, such as HTTP GET
and HTTP POST. Typically they will return a View, optionally with some model data, or a redirect to another action.

## Group Contributions

| Name            | Student ID  |                 Key Contributions |
|-----------------|-------------|----------------------------------:|
| Ruoyo Bo        | 24641007    |   Payment Management (Feature 04) |
| Jiaqian Cai     | 25504883    |  Shipment Management (Feature 05) |
| James Chivers   | 24547198    |      User Management (Feature 01) |
| Reece Malanos   | 14255448    | Catalogue Management (Feature 02) |
| Mikhail Zhelnin | 14524159    | Suppliers Management (Feature 09) |
| Caroline Zhou   | 25607742    |     Order Management (Feature 03) |
