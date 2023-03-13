const commands = [
    {
        name : "dotnet new --list",
        desc : "show the list of possible templates for create a dotnet project"
    },
    {
        name : "dotnet new webapi",
        desc : "create the template for a API"
    },
    {
        name : "dotnet watch run",
        desc : "run the current project"
    },
    {
        name: "dotnet ef -h",
        desc: "see the commands for manage the database"
    },
    {
        name: "dotnet ef migrations add 'migration_name'",
        desc: "create the new model migration to the current database"
    },
    {
        name: "dotnet ef migrations remove",
        desc: "remove the current database migrations"
    },
    {
        name: "dotnet ef database update",
        desc: "connect to the dabase and update according the migrations"
    },
    {
        name: "dotnet add package Swashbuckle.AspNetCore.Filters",
        desc: "its a package for add security definitions to swagger and for the filters"
    }
]