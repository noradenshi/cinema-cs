# cinema-cs

Cinema ticket service.

### Techstack

- Avalonia UI
- Postgres (docker-compose)

### How to run

```bash
docker-compose up
dotnet run
```

### Tables

User:
id
name
surname
phone
password

Ticket:
id
screening_id
seat_id
price
owner_id  nullable

Movie:
id
title
description
length

Room:
id
capacity

Seat:
id
room_id
row
number 

Screening:
id
movie_id
room_id
date
