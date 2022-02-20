# CQRS & EventSourcing

[![Build status](https://github.com/tautvydasversockas/cqrs-eventsourcing/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/tautvydasversockas/cqrs-event-sourcing/actions/workflows/dotnet.yml)

This is a simple application built using `CQRS` and `EventSourcing` patterns. 

You can find more information about `EventSourcing` at [thehonestcoder.com](https://thehonestcoder.com/the-promised-land-of-event-sourcing).

## Atchitecture

![image](https://user-images.githubusercontent.com/12632820/154841826-63091662-5c8f-4b61-88d6-e00384c75aa9.png)

## Testing

Aggregate tests are written using `given-when-then` pattern:
- `Given` - a set of existing events is provided
- `When` - a command is provided
- `Then` - a set of resulting events is provided

Tests specifications are generated from tests by running `Accounts.SpecGenerator`.

## Running the application

- Run `docker-compose up` to start the application.
- Connect to EventStoreDB admin panel (http://localhost:2113/) and create a persistent subscription:
  - Stream name: `$ce-account`
  - Group name: `account-view`
- Run `docker-compose restart` to restart the application.

## Support

<a href="https://www.buymeacoffee.com/tautvydasverso"> 
    <img align="left" src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" height="50" width="210"  alt="tautvydasverso" />
</a>
