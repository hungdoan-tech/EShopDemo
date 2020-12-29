### Time-zone eShop
This is a server side rendering typed webapp in monolithic architecture based on MVC model

Development Environment: .Net Core 

Framework: ASP.Net Core 3.0

The default DBMS in this project is Microsoft SQL Server.

The connection string configuration for database connection is stored at appsetting.json in root directory.
Anyone wanna start to run this project in development enviroment just scaffold a new migration and apply it from a command prompt at this project's directory to generate the database:

> dotnet ef migrations add [migration name]

> dotnet ef database update

The default admin user has an account:  email:  admin@gmail.com 	 password: Admin123*

The rest seed users information can be seen in ApplicationDbContext.cs in DataAccess folder.