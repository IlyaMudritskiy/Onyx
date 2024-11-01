# Onyx API

It is a service that acts as an aggregator and search engine for process and acoustic data for various lines in production. It works with process data produced by OPC UA client and with acoustic testing data produced by ASx.

OPC client on TTL is updated to work with this API. Opal project acts (partially) as a front-end application for this API.

# Installation and use

## Build and configure

Go to `Build` -> `Publish Onyx`.

Publish is already configured for this project.

Open `appsettings.json` and configure the SQL Server connection string, Mongo DB and Jwt.

Change the key in `Jwt` -> `Key` for security.

Copy the published version on USB drive or to a destination machine and run the application.

Check logs in `Logs/onyx<today's date>.log` to see if there are any issues with connecting to databases.

Onyx is configured with Kestrel web server for use on `0.0.0.0` and receiving http requests from outside the machine.

## Infrastructure

### Mongo DB

MongoDB can be provided in 2 ways:

1. Local installation as service
2. Docker container

In default configuration you can find connection info for local installation.

Create a database `process_data` and a collection `process_data`.

After loading some files, create 2 indexes:

1. DUT.serial_nr
2. DUT.created_at

It is needed for faster search when GET requests use creation date or serial number of the part.

### SQL Server

Install SQL Server Express and use the default details during installation.

Onyx is configured with automatic migrations to create database scheme and structure for managing the users and their access.


# API endpoints

## Auth

### Register

Allows an end user to create a new accout (or user) in API.

URL: `[POST]` `/api/Auth/Register`

Body example:

```json
{
  "username": "string",
  "email": "user@example.com",
  "password": "string",
  "roles": [
    "string"
  ]
}
```

Password requirements:

- 

Available roles:

1. Reader
   - Can use only `GET` methods and endpoints and can only read data.
2. Writer
   - Can use only `POST` methods and endpoints and can only create new data.

### Login

### Ping

## ProcessData

## AcousticData