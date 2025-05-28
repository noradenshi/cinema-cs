# cinema-cs

Cinema ticket service.

## Techstack

- .ASP
- Postgres (docker-compose)

## How to run

```bash
docker-compose up -d
dotnet run
```

## Database

### User Table

| Column   | Type     | Description       |
|----------|----------|-------------------|
| id       | int      | Primary key       |
| name     | string   | First name        |
| surname  | string   | Last name         |
| phone    | string   | Phone number      |
| email    | string   | Email address     |
| password | string   | Hashed password   |

---

### Ticket Table

| Column       | Type     | Description                        |
|--------------|----------|------------------------------------|
| screening_id | int      | (PK) Foreign key to Screening      |
| seat_id      | int      | (PK) Foreign key to Seat           |
| price        | decimal  | Ticket price                       |
| owner_id     | int      | Foreign key to User                |

---

### Movie Table

| Column      | Type     | Description             |
|-------------|----------|-------------------------|
| id          | int      | Primary key             |
| title       | string   | Movie title             |
| description | string   | Movie description       |
| length      | int      | Duration in minutes     |

---

### Room Table

| Column   | Type     | Description        |
|----------|----------|--------------------|
| id       | int      | Primary key        |
| capacity | int      | Number of seats    |

---

### Seat Table

| Column  | Type     | Description                 |
|---------|----------|-----------------------------|
| id      | int      | Primary key                 |
| room_id | int      | Foreign key to Room         |
| row     | string   | Row label (e.g., "A", "B")  |
| number  | int      | Seat number within the row  |

---

### Screening Table

| Column   | Type     | Description               |
|----------|----------|---------------------------|
| id       | int      | Primary key               |
| movie_id | int      | Foreign key to Movie      |
| room_id  | int      | Foreign key to Room       |
| date     | datetime | Screening date and time   |
