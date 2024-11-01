# Onyx API

It is a service that acts as an aggregator and search engine for process and acoustic data for various lines in production. It works with process data produced by OPC UA client and with acoustic testing data produced by ASx.

OPC client on TTL is updated to work with this API. Opal project acts (partially) as a front-end application for this API.

# Installation and use

## Build and configure

Make sure that code (shown below in section **API documentation**) is uncommented.

1. `Build` -> `Build solution` (Release mode)
2. `Build` -> `Publish Onyx`

Get the files from the location of the project in `.../bin/Release/net8.0`.

Publish is already configured for this project.

Open `appsettings.json` and configure the SQL Server connection string, Mongo DB and Jwt.

Copy the published version on USB drive or to a destination machine and run the application.

Check logs in `Logs/onyx<today's date>.log` to see if there are any issues with connecting to databases.

Onyx is configured with Kestrel web server for use on `0.0.0.0` and receiving http requests from outside the machine.

## API documentation

To open the Swagger, please comment the following lines in `Program.cs` and run the app in debug mode:

```cs
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5000);
});
```

## Infrastructure

### Mongo DB

MongoDB can be provided in 2 ways:

1. Local installation as service
2. Docker container

In default configuration you can find connection info for local installation.

Create a database `process_data` and a collection `process_data`.

After loading some files, create index `DUT.created_at` with TTL set to 1 week (in seconds).

It is needed for faster search when GET requests use creation date or serial number of the part.

### SQL Server

Install SQL Server Express and use the default details during installation.

Onyx is configured with automatic migrations to create database scheme and structure for managing the users and their access.
